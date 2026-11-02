namespace ClientApi.FaceRecognition;

/// <summary>每次内部请求携带的配置快照，供 Python 验证并应用最新 YAML 设置。</summary>
public sealed class FaceRecognitionSettings
{
    /// <summary>身份匹配的距离与候选区分规则。</summary>
    public FaceMatchingSettings Matching { get; set; } = new();
    /// <summary>录入样本的存储约束。</summary>
    public FaceStorageSettings Storage { get; set; } = new();
    /// <summary>单帧反欺骗开关；不包含动作或双目活体。</summary>
    public FaceLivenessSettings Liveness { get; set; } = new();
}

/// <summary>余弦距离匹配参数，距离越小越相似。</summary>
public sealed class FaceMatchingSettings
{
    /// <summary>允许的最大余弦距离，范围 0～2；默认沿用 SFace 的 0.593。</summary>
    public double DistanceThreshold { get; set; } = 0.593;
    /// <summary>第一、第二候选身份的最小距离差，范围 0～2。</summary>
    public double CandidateMargin { get; set; } = 0.05;
}

/// <summary>SQLite 人脸样本约束，不改变身份库格式和现有样本。</summary>
public sealed class FaceStorageSettings
{
    /// <summary>每个用户允许的最大样本数，必须为正整数；降低上限不会删除已有样本。</summary>
    public int MaxSamplesPerUser { get; set; } = 10;
}

/// <summary>使用现有 YuNet 与 Fasnet 模型进行单张图片反欺骗检查。</summary>
public sealed class FaceLivenessSettings
{
    /// <summary>总开关，默认关闭；启用后模型或检测失败会拒绝相应请求。</summary>
    public bool Enabled { get; set; }
    /// <summary>总开关启用时，是否在录入前检查活体。</summary>
    public bool CheckOnEnroll { get; set; } = true;
    /// <summary>总开关启用时，是否在识别前检查活体。</summary>
    public bool CheckOnMatch { get; set; } = true;
}
