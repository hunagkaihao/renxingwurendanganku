"""封装 DeepFace 模型与图像输入边界，模型按需加载且推理串行执行。"""

import base64
import binascii
import io
import logging
import math
import os
import threading
from pathlib import Path

import cv2
import numpy as np
from PIL import Image, UnidentifiedImageError

ROOT = Path(__file__).resolve().parents[1]
MODEL_HOME = Path(os.environ.get("CLIENT_FACE_MODELS_DIR", str(ROOT / "data" / "models")))  # 身份与活体权重目录。
os.environ["DEEPFACE_HOME"] = str(MODEL_HOME)
os.environ.setdefault("TF_CPP_MIN_LOG_LEVEL", "2")
os.environ.setdefault("TF_USE_LEGACY_KERAS", "1")
MODEL = "SFace"
PAD_DETECTOR = "yunet"  # 反欺骗的一致性核对使用稳定关键点，避免 Haar 眼部误检造成错误旋转。
THRESHOLD = 0.593  # DeepFace 0.0.101 的 SFace / cosine 默认阈值，尚未针对本设备校准。
MARGIN = 0.05  # 不同身份的候选距离差不足时拒绝匹配，作为演示的保守规则。


class FaceInputError(ValueError):
    """图像可解析但不满足单人脸录入或识别要求。"""


class EngineUnavailable(RuntimeError):
    """模型加载失败或推理繁忙，调用端可以在问题解决后重试。"""


def decode_image(value: object) -> tuple[np.ndarray, bytes]:
    """解析浏览器 JPEG 数据 URL；拒绝文件路径、远程 URL 和超大图像。"""
    prefix = "data:image/jpeg;base64,"
    if not isinstance(value, str) or not value.startswith(prefix):
        raise FaceInputError("请使用摄像头采集的 JPEG 图像。")
    if len(value) > 6_000_000:
        raise FaceInputError("图像过大，请降低采集分辨率。")
    try:
        raw = base64.b64decode(value[len(prefix):], validate=True)
        with Image.open(io.BytesIO(raw)) as image:
            if image.format != "JPEG" or not (80 <= image.width <= 4096 and 80 <= image.height <= 4096):
                raise FaceInputError("图像必须为 JPEG，宽高范围为 80～4096 像素。")
            if image.width * image.height > 8_500_000:
                raise FaceInputError("图像像素数过大。")
            image.verify()
    except (binascii.Error, UnidentifiedImageError, OSError, Image.DecompressionBombError) as error:
        raise FaceInputError("图像损坏，请重新采集。") from error
    frame = cv2.imdecode(np.frombuffer(raw, dtype=np.uint8), cv2.IMREAD_COLOR)
    if frame is None:
        raise FaceInputError("无法解码图像，请重新采集。")
    return frame, raw


class FaceEngine:
    """延迟初始化 SFace；同一进程只允许一个模型加载或推理操作。"""

    def __init__(self):
        """初始化模型状态；应用启动不依赖模型权重已经下载。"""
        self.lock = threading.Lock()
        self.ready = False
        self.error = None
        self.api = None
        self.anti_spoofing_ready = False  # 单独记录反欺骗模型状态，不混同身份模型就绪。
        self.anti_spoofing_error = None
        self.anti_spoofing_model = None

    def _load(self):
        """在持有锁时加载 DeepFace 和模型，失败保留可诊断的异常日志。"""
        if self.ready:
            return
        try:
            from deepface import DeepFace

            MODEL_HOME.mkdir(parents=True, exist_ok=True)
            DeepFace.build_model(MODEL)
            self.api = DeepFace
            self.ready = True
            self.error = None
        except Exception as error:
            self.error = "模型加载失败，请检查网络或查看 output/server.err.log。"
            logging.getLogger(__name__).exception("加载 DeepFace SFace 模型失败")
            raise EngineUnavailable(self.error) from error

    def warmup(self):
        """预加载模型；重复调用复用内存中的模型实例。"""
        if not self.lock.acquire(blocking=False):
            raise EngineUnavailable("模型正在加载或识别，请稍后重试。")
        try:
            self._load()
        finally:
            self.lock.release()

    def represent(self, frame: np.ndarray) -> tuple[list[float], dict]:
        """提取唯一一张人脸的特征，拒绝无人脸、多人、过小或明显模糊的图像。"""
        if not self.lock.acquire(blocking=False):
            raise EngineUnavailable("模型正在加载或识别，请稍后重试。")
        try:
            self._load()
            crop, box, _ = self._single_face(frame)
            result = self.api.represent(
                img_path=crop, model_name=MODEL, detector_backend="skip",
                enforce_detection=True, align=False, normalization="base",
            )
            return result[0]["embedding"], box
        finally:
            self.lock.release()

    def _single_face(self, frame, *, detector_backend="opencv"):
        """持有引擎锁时检测唯一且清晰的人脸，供身份与反欺骗流程共用。"""
        faces = self.api.extract_faces(
            img_path=frame, detector_backend=detector_backend, enforce_detection=False,
            align=True, color_face="bgr", normalize_face=False,
        )
        faces = [face for face in faces if face["confidence"] > 0]
        if not faces:
            raise FaceInputError("未检测到人脸，请正对摄像头并改善照明。")
        if len(faces) != 1:
            raise FaceInputError("检测到多张人脸，请保持只有一人入镜。")
        face = faces[0]
        area = face["facial_area"]
        if min(area["w"], area["h"]) < 80:
            raise FaceInputError("人脸过小，请靠近摄像头。")
        crop = face["face"].astype(np.uint8)
        gray = cv2.cvtColor(crop, cv2.COLOR_BGR2GRAY)
        brightness = float(gray.mean())
        sharpness = float(cv2.Laplacian(gray, cv2.CV_64F).var())
        if brightness < 35:
            raise FaceInputError("人脸画面过暗，请补充正面照明。")
        if sharpness < 20:
            raise FaceInputError("人脸画面模糊，请保持稳定后重试。")
        quality = {"brightness_mean": round(brightness, 1), "sharpness_variance": round(sharpness, 1),
                   "frame_width": int(frame.shape[1]), "frame_height": int(frame.shape[0]),
                   "eye_angle_degrees": None, "eyes_detected": False, "detector_backend": detector_backend}
        left_eye, right_eye = area.get("left_eye"), area.get("right_eye")
        if left_eye is not None and right_eye is not None:
            dx = float(right_eye[0])-float(left_eye[0])
            dy = float(right_eye[1])-float(left_eye[1])
            if math.hypot(dx, dy) > 0:
                quality["eyes_detected"] = True
                quality["eye_angle_degrees"] = round((math.degrees(math.atan2(dy, dx))+90) % 180-90, 1)
        if detector_backend == PAD_DETECTOR and not quality["eyes_detected"]:
            raise FaceInputError("未能稳定定位双眼，请让完整人脸正对镜头后重试。")
        return crop, {key: int(area[key]) for key in ("x", "y", "w", "h")}, quality

    def _load_anti_spoofing(self):
        """在引擎锁内加载 DeepFace 的 TensorFlow MiniFASNet，失败不产生默认真人结果。"""
        if self.anti_spoofing_ready:
            return
        weights = MODEL_HOME / ".deepface" / "weights"
        required = ("face_detection_yunet_2023mar.onnx", "2.7_80x80_MiniFASNetV2.h5", "4_0_0_80x80_MiniFASNetV1SE.h5")
        if any(not (weights / name).is_file() for name in required):
            raise EngineUnavailable("缺少活体权重，请准备 YuNet 与两份 MiniFASNet 权重，并执行 scripts/prepare-face-runtime.ps1 -IncludeLiveness。")
        self._load()
        try:
            self.api.build_model(PAD_DETECTOR, task="face_detector")
            self.anti_spoofing_model = self.api.build_model("Fasnet", task="spoofing")
            self.anti_spoofing_ready = True
            self.anti_spoofing_error = None
        except Exception as error:
            self.anti_spoofing_error = "反欺骗检测器或分类模型加载失败，请查看 output/server.err.log。"
            logging.getLogger(__name__).exception("加载反欺骗人脸检测器或分类模型失败")
            raise EngineUnavailable(self.anti_spoofing_error) from error

    def warmup_anti_spoofing(self):
        """显式加载反欺骗模型；使用现有 TensorFlow 依赖和项目内权重缓存。"""
        if not self.lock.acquire(blocking=False):
            raise EngineUnavailable("模型正在加载或识别，请稍后重试。")
        try:
            self._load_anti_spoofing()
        finally:
            self.lock.release()

    def inspect_liveness(self, frame):
        """分析一张原始帧，并提取同一人脸特征用于跨帧一致性检查。

        score 是模型对其预测类别的分数，不是身份概率或安全保证。
        """
        if not self.lock.acquire(blocking=False):
            raise EngineUnavailable("模型正在加载或识别，请稍后重试。")
        try:
            self._load_anti_spoofing()
            crop, box, quality = self._single_face(frame, detector_backend=PAD_DETECTOR)
            is_real, score = self.anti_spoofing_model.analyze(
                frame, tuple(box[key] for key in ("x", "y", "w", "h")),
            )
            embedding = self.api.represent(
                img_path=crop, model_name=MODEL, detector_backend="skip",
                enforce_detection=True, align=False, normalization="base",
            )[0]["embedding"]
            return {"is_real": bool(is_real), "score": float(score), "embedding": embedding,
                    "box": box, "quality": quality}
        finally:
            self.lock.release()
