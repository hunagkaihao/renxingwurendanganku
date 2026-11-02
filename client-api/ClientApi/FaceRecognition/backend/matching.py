"""按身份汇总样本距离，并对陌生人和相似候选明确拒绝匹配。"""

import math
from collections.abc import Iterable, Sequence


def unit_vector(vector: Sequence[float]) -> list[float]:
    """校验并归一化非零有限向量；无效模型输出不能参与身份匹配。"""
    if not vector or not all(math.isfinite(value) for value in vector):
        raise ValueError("人脸特征包含无效数值。")
    length = math.sqrt(sum(value * value for value in vector))
    if not math.isfinite(length) or length <= 0:
        raise ValueError("人脸特征不能为零向量。")
    return [value / length for value in vector]


def match_identity(
    embedding: Sequence[float],
    samples: Iterable[tuple[str, str, Sequence[float]]],
    threshold: float,
    margin: float,
) -> dict:
    """返回 1:N 匹配结果；阈值限制距离，margin 限制不同身份之间的最小差距。

    同一身份有多个样本时取最小余弦距离。没有唯一可信候选时，不返回身份。
    """
    query = unit_vector(embedding)
    distances: dict[str, tuple[float, str]] = {}
    for person_id, name, sample in samples:
        if len(sample) != len(query):
            raise ValueError("人脸特征维度不一致，请重新录入。")
        reference = unit_vector(sample)
        distance = max(0.0, min(2.0, 1 - sum(a * b for a, b in zip(query, reference))))
        if person_id not in distances or distance < distances[person_id][0]:
            distances[person_id] = (distance, name)
    ranked = sorted(distances.items(), key=lambda entry: entry[1][0])
    result = {
        "status": "empty", "person_id": None, "name": None,
        "distance": None, "threshold": threshold, "margin": margin,
    }
    if not ranked:
        return result
    person_id, (distance, name) = ranked[0]
    result["distance"] = round(distance, 6)
    if distance > threshold:
        result["status"] = "unknown"
    elif len(ranked) > 1 and ranked[1][1][0] - distance < margin:
        result["status"] = "ambiguous"
    else:
        result.update(status="matched", person_id=person_id, name=name)
    return result
