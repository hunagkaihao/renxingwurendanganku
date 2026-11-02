"""验证父进程传递的请求配置，省略字段时保持原识别与存储默认行为。"""

import math
from dataclasses import dataclass


def section(value: dict, key: str) -> dict:
    """读取可省略的配置对象；显式 null、字符串或数组均不等同于缺省。"""
    result = value.get(key, {})
    if not isinstance(result, dict):
        raise ValueError(f"人脸配置 {key} 必须为对象。")
    return result


def distance(value: dict, key: str, default: float) -> float:
    """校验余弦距离参数为 0～2 范围内的有限数值，拒绝布尔与隐式转换。"""
    result = value.get(key, default)
    if type(result) not in (int, float) or not math.isfinite(result) or not 0 <= result <= 2:
        raise ValueError(f"人脸配置 {key} 必须为 0～2 范围内的有限数值。")
    return float(result)


def switch(value: dict, key: str, default: bool) -> bool:
    """校验开关是布尔值，避免字符串 false 被判定为启用。"""
    result = value.get(key, default)
    if type(result) is not bool:
        raise ValueError(f"人脸配置 {key} 必须为布尔值。")
    return result


@dataclass(frozen=True)
class FaceSettings:
    """单次请求不可变配置；参数有效性在执行前统一检查。"""

    threshold: float = 0.593  # 最大余弦距离，范围 0～2。
    margin: float = 0.05  # 不同候选身份的最小距离差，范围 0～2。
    max_samples: int = 10  # 每用户样本上限，必须为正整数。
    liveness_enabled: bool = False  # 单帧反欺骗总开关。
    check_on_enroll: bool = True  # 启用活体时，是否检查录入图片。
    check_on_match: bool = True  # 启用活体时，是否检查识别图片。

    @classmethod
    def from_payload(cls, value: object) -> "FaceSettings":
        """解析内部请求配置；字段省略使用默认值，无效值明确失败。"""
        if not isinstance(value, dict):
            raise ValueError("人脸配置 settings 必须为对象。")
        matching = section(value, "matching")
        storage = section(value, "storage")
        liveness = section(value, "liveness")
        max_samples = storage.get("maxSamplesPerUser", 10)
        if type(max_samples) is not int or max_samples <= 0:
            raise ValueError("人脸配置 maxSamplesPerUser 必须为正整数。")
        return cls(distance(matching, "distanceThreshold", 0.593),
                   distance(matching, "candidateMargin", 0.05), max_samples,
                   switch(liveness, "enabled", False),
                   switch(liveness, "checkOnEnroll", True),
                   switch(liveness, "checkOnMatch", True))
