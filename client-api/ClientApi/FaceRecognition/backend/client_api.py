"""供 ClientApi 自动托管的身份识别服务；按配置执行单帧反欺骗，不加载摄像头或双目模块。"""

import base64
import binascii
import ctypes
import hmac
import io
import logging
import os
import threading
from pathlib import Path

from flask import Flask, jsonify, request
from PIL import Image, UnidentifiedImageError
from werkzeug.exceptions import HTTPException

from backend.engine import ROOT, FaceEngine, FaceInputError, EngineUnavailable, decode_image
from backend.matching import match_identity
from backend.store import FaceStore
from backend.settings import FaceSettings


def business_id(value: object) -> str:
    """接受正数 Int32 的文本 ID，规范化前导零；0 保留为未匹配结果。"""
    if not isinstance(value, str) or not value.isascii() or not value.isdecimal() or len(value) > 40:
        raise ValueError("用户 ID 必须为正整数文本。")
    number = int(value)
    if not 1 <= number <= 2_147_483_647:
        raise ValueError("用户 ID 必须在 1～2147483647 范围内。")
    return str(number)


def image_frame(value: object):
    """兼容原接口的 Base64 和数据 URL；验证常见图片格式后统一为 JPEG。"""
    if not isinstance(value, str) or len(value) > 6_000_000:
        raise FaceInputError("图片为空或过大。")
    encoded = value
    if value.startswith("data:"):
        header, separator, encoded = value.partition(",")
        if not separator or header not in {"data:image/jpeg;base64", "data:image/png;base64", "data:image/bmp;base64"}:
            raise FaceInputError("只支持 JPEG、PNG 或 BMP 图像。")
    try:
        raw = base64.b64decode(encoded, validate=True)
        with Image.open(io.BytesIO(raw)) as image:
            if image.format not in {"JPEG", "PNG", "BMP"}:
                raise FaceInputError("只支持 JPEG、PNG 或 BMP 图像。")
            if not (80 <= image.width <= 4096 and 80 <= image.height <= 4096) or image.width * image.height > 8_500_000:
                raise FaceInputError("图像尺寸不符合要求。")
            if image.format != "JPEG":
                buffer = io.BytesIO()
                image.convert("RGB").save(buffer, "JPEG", quality=95)
                raw = buffer.getvalue()
    except (binascii.Error, UnidentifiedImageError, OSError, Image.DecompressionBombError, ValueError) as error:
        raise FaceInputError("图像损坏，请重新采集。") from error
    return decode_image("data:image/jpeg;base64," + base64.b64encode(raw).decode("ascii"))


def create_app(database: Path | None = None, engine=None, token: str | None = None) -> Flask:
    """创建独立业务人脸库；令牌由父进程提供，测试可注入模型替身。"""
    secret = token or os.environ.get("CLIENT_FACE_TOKEN")
    if not secret:
        raise RuntimeError("必须由 ClientApi 提供内部访问令牌。")
    app = Flask(__name__)
    app.config["MAX_CONTENT_LENGTH"] = 6_000_000
    store = FaceStore(database or Path(os.environ.get("CLIENT_FACE_DATA_DIR", str(ROOT / "data" / "client-api"))) / "faces.sqlite3")
    recognizer = engine if engine is not None else FaceEngine()
    app.config["FACE_SETTINGS"] = FaceSettings()

    @app.before_request
    def internal_only():
        """拒绝浏览器来源与未认证调用；只供父进程在回环接口访问。"""
        if request.headers.get("Origin") or request.headers.get("Sec-Fetch-Site") == "cross-site":
            return jsonify(error="不允许浏览器直接访问内部识别服务。"), 403
        if not hmac.compare_digest(request.headers.get("Authorization", ""), "Bearer " + secret):
            return jsonify(error="内部服务认证失败。"), 401

    @app.after_request
    def no_cache(response):
        """禁止缓存身份与识别结果。"""
        response.headers["Cache-Control"] = "no-store"
        return response

    @app.errorhandler(FaceInputError)
    def invalid_image(error):
        """将图像质量或格式错误转为输入失败。"""
        return jsonify(error=str(error)), 422

    @app.errorhandler(EngineUnavailable)
    def unavailable(error):
        """模型故障或繁忙保持失败，不能返回默认身份。"""
        return jsonify(error=str(error)), 503

    @app.errorhandler(ValueError)
    def invalid_value(error):
        """用户 ID、样本上限与模型版本不符合要求时明确失败。"""
        return jsonify(error=str(error)), 400

    @app.errorhandler(HTTPException)
    def http_error(error):
        """保持 HTTP 错误码并统一返回 JSON。"""
        return jsonify(error=error.name), error.code

    @app.errorhandler(Exception)
    def unexpected(error):
        """在服务边界记录未知故障，响应不包含内部路径或样本。"""
        app.logger.exception("内部身份服务请求失败：%s %s", request.method, request.path)
        return jsonify(error="识别服务处理失败，请查看日志。"), 500

    def body():
        """只接受 JSON 对象。"""
        from werkzeug.exceptions import BadRequest
        value = request.get_json()
        if not isinstance(value, dict):
            raise BadRequest()
        return value

    def request_settings(value: dict) -> FaceSettings:
        """应用父进程当前配置快照；验证失败时不执行模型推理或写库。"""
        settings = FaceSettings.from_payload(value.get("settings", {}))
        app.config["FACE_SETTINGS"] = settings
        return settings

    def check_liveness(frame):
        """检查原始帧的反欺骗结论；身份特征仍由原 opencv 流程提取。"""
        result = recognizer.inspect_liveness(frame)
        if result.get("is_real") is not True:
            raise FaceInputError("未通过活体检查，请使用真人现场采集的图像。")

    @app.get("/api/status")
    def status():
        """返回身份模型状态和最近一次已验证请求的活体总开关。"""
        return jsonify(service="client-api-face", ready=recognizer.ready,
                       liveness_enabled=app.config["FACE_SETTINGS"].liveness_enabled)

    @app.post("/api/warmup")
    def warmup():
        """预加载身份模型；活体启用时预加载反欺骗模型，缺少权重明确失败。"""
        settings = request_settings(body())
        recognizer.warmup()
        if settings.liveness_enabled:
            recognizer.warmup_anti_spoofing()
        return jsonify(ready=recognizer.ready)

    @app.post("/api/enroll")
    def enroll():
        """用业务用户 ID 作为独立人脸库的身份键，同一 ID 追加样本。"""
        value = body()
        settings = request_settings(value)
        user_id = business_id(value.get("user_id"))
        frame, raw = image_frame(value.get("image"))
        if settings.liveness_enabled and settings.check_on_enroll:
            check_liveness(frame)
        embedding, _ = recognizer.represent(frame)
        person = store.add(user_id, embedding, raw, max_samples=settings.max_samples)
        return jsonify(user_id=int(user_id), sample_count=person["sample_count"]), 201

    @app.post("/api/recognize")
    def recognize():
        """复用原识别规则；只有唯一匹配才返回业务用户 ID。"""
        value = body()
        settings = request_settings(value)
        frame, _ = image_frame(value.get("image"))
        if settings.liveness_enabled and settings.check_on_match:
            check_liveness(frame)
        samples = store.samples()
        embedding = recognizer.represent(frame)[0] if samples else [1.0]
        result = match_identity(embedding, samples, settings.threshold, settings.margin)
        return jsonify(status=result["status"], user_id=int(result["name"]) if result["status"] == "matched" else None)

    @app.delete("/api/people/<user_id>")
    def delete(user_id):
        """按业务用户 ID 删除身份及全部样本。"""
        name = business_id(user_id)
        person = next((entry for entry in store.people() if entry["name"] == name), None)
        if person is None or not store.delete(person["id"]):
            return jsonify(error="该用户没有已录入的人脸。"), 404
        return jsonify(deleted=True)

    return app


def watch_parent(parent_pid: int):
    """Windows 父进程异常退出时终止子服务，防止识别进程残留。"""
    from ctypes import wintypes
    kernel = ctypes.WinDLL("kernel32", use_last_error=True)
    kernel.OpenProcess.argtypes = [wintypes.DWORD, wintypes.BOOL, wintypes.DWORD]
    kernel.OpenProcess.restype = wintypes.HANDLE
    kernel.WaitForSingleObject.argtypes = [wintypes.HANDLE, wintypes.DWORD]
    kernel.CloseHandle.argtypes = [wintypes.HANDLE]
    handle = kernel.OpenProcess(0x00100000, False, parent_pid)
    if not handle:
        os._exit(1)
    try:
        if kernel.WaitForSingleObject(handle, 0xFFFFFFFF) == 0:
            os._exit(0)
    finally:
        kernel.CloseHandle(handle)


if __name__ == "__main__":
    from waitress import serve
    logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s: %(message)s")
    parent = int(os.environ["CLIENT_FACE_PARENT_PID"])
    if os.name == "nt":
        threading.Thread(target=watch_parent, args=(parent,), daemon=True).start()
    serve(create_app(), host="127.0.0.1", port=int(os.environ["CLIENT_FACE_PORT"]), threads=4,
          max_request_body_size=6_000_000)
