"""使用 SQLite 原子保存身份、人脸样本与特征；照片不对外提供访问接口。"""

import json
import sqlite3
import uuid
from contextlib import contextmanager
from pathlib import Path

from backend.matching import unit_vector

MAX_SAMPLES = 10  # 每个身份最多保存十张样本，供本地小规模验证使用。
PIPELINE = "deepface-0.0.101:SFace:opencv:align:base:cosine"


class FaceStore:
    """管理单个本地人脸库，每次操作使用独立连接并在退出时关闭。"""

    def __init__(self, path: Path):
        """创建数据库目录和表结构，保留已经录入的数据。"""
        self.path = path
        path.parent.mkdir(parents=True, exist_ok=True)
        with self.connect() as connection:
            connection.executescript("""
                CREATE TABLE IF NOT EXISTS people (
                    id TEXT PRIMARY KEY,
                    name TEXT NOT NULL UNIQUE,
                    created_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
                );
                CREATE TABLE IF NOT EXISTS samples (
                    id TEXT PRIMARY KEY,
                    person_id TEXT NOT NULL REFERENCES people(id) ON DELETE CASCADE,
                    pipeline TEXT NOT NULL,
                    embedding TEXT NOT NULL,
                    image BLOB NOT NULL,
                    created_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
                );
            """)

    @contextmanager
    def connect(self):
        """开启带事务和外键约束的连接，异常时回滚。"""
        connection = sqlite3.connect(self.path, timeout=10)
        connection.row_factory = sqlite3.Row
        connection.execute("PRAGMA foreign_keys = ON")
        connection.execute("PRAGMA secure_delete = ON")
        try:
            with connection:
                yield connection
        finally:
            connection.close()

    def people(self) -> list[dict]:
        """列出身份和样本数量，时间采用 UTC ISO 8601 格式。"""
        with self.connect() as connection:
            return [dict(row) for row in connection.execute("""
                SELECT p.id, p.name, p.created_at, COUNT(s.id) AS sample_count
                FROM people p LEFT JOIN samples s ON s.person_id = p.id
                GROUP BY p.id ORDER BY p.created_at DESC, p.id
            """)]

    def add(self, name: str, embedding: list[float], image: bytes, *, max_samples: int = MAX_SAMPLES) -> dict:
        """同名视为同一身份并追加样本；姓名唯一性和样本上限在事务中验证。"""
        unit_vector(embedding)
        with self.connect() as connection:
            connection.execute("BEGIN IMMEDIATE")
            row = connection.execute("SELECT id FROM people WHERE name = ?", (name,)).fetchone()
            person_id = row["id"] if row else uuid.uuid4().hex
            count = connection.execute(
                "SELECT COUNT(*) FROM samples WHERE person_id = ?", (person_id,)
            ).fetchone()[0]
            if count >= max_samples:
                raise ValueError(f"该身份样本已达到上限 {max_samples} 张，请调整上限或删除身份后重新录入。")
            if row is None:
                connection.execute("INSERT INTO people (id, name) VALUES (?, ?)", (person_id, name))
            connection.execute(
                "INSERT INTO samples (id, person_id, pipeline, embedding, image) VALUES (?, ?, ?, ?, ?)",
                (uuid.uuid4().hex, person_id, PIPELINE, json.dumps(embedding, allow_nan=False), image),
            )
        return {"id": person_id, "name": name, "sample_count": count + 1}

    def samples(self) -> list[tuple[str, str, list[float]]]:
        """只读取与当前模型流程一致的特征，防止跨模型错误比对。"""
        with self.connect() as connection:
            rows = connection.execute("""
                SELECT p.id, p.name, s.embedding, s.pipeline FROM samples s
                JOIN people p ON p.id = s.person_id
            """).fetchall()
        if any(row["pipeline"] != PIPELINE for row in rows):
            raise ValueError("人脸库模型版本不一致，请删除旧身份并重新录入。")
        return [(row["id"], row["name"], json.loads(row["embedding"])) for row in rows]

    def delete(self, person_id: str) -> bool:
        """删除指定身份及其全部照片和特征，返回身份是否存在。"""
        with self.connect() as connection:
            return connection.execute("DELETE FROM people WHERE id = ?", (person_id,)).rowcount > 0
