using ClientApi.FaceRecognition;
using ClientApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers;

/// <summary>兼容现有客户端的人脸接口，内部使用托管的 DeepFace 身份服务。</summary>
[ApiController]
[Route("[controller]")]
[RequestSizeLimit(6_000_000)]
public class FaceController : ControllerBase
{
    private readonly FaceRecognitionService _recognition;
    private readonly ILogger<FaceController> _logger;

    /// <summary>注入身份服务及接口边界日志。</summary>
    public FaceController(FaceRecognitionService recognition, ILogger<FaceController> logger)
    {
        _recognition = recognition;
        _logger = logger;
    }

    /// <summary>识别业务用户；0 为未匹配，-1 为输入或服务失败。</summary>
    /// <param name="input">包含 Base64 人脸照片的识别请求。</param>
    /// <param name="cancellationToken">客户端断开连接时取消请求。</param>
    /// <returns>匹配到的业务用户编号；未匹配返回 0，输入或服务失败返回 -1。</returns>
    [HttpPost("FaceMatch")]
    public async Task<int> FaceMatch(FaceMatchDto input, CancellationToken cancellationToken)
    {
        try { return await _recognition.MatchAsync(input.ImageData, cancellationToken); }
        catch (FaceServiceException error) { _logger.LogWarning("人脸识别失败：{Message}", error.Message); return -1; }
        catch (HttpRequestException error) { _logger.LogError(error, "访问人脸识别服务失败。"); return -1; }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        { _logger.LogWarning("人脸识别请求超时。"); return -1; }
    }

    /// <summary>按业务用户 ID 录入；保留 Errno、Msg、Data.Log_id 返回结构。</summary>
    /// <param name="input">业务用户编号及 Base64 人脸照片。</param>
    /// <param name="cancellationToken">客户端断开连接时取消请求。</param>
    /// <returns>录入结果，Errno 为 0 表示成功，99 表示业务或服务失败。</returns>
    [HttpPost("FaceAdd")]
    public async Task<FaceManagerResultDto> FaceAdd(FaceAddDto input, CancellationToken cancellationToken)
    {
        try
        {
            await _recognition.EnrollAsync(input.UserId, input.ImageData, cancellationToken);
            return new FaceManagerResultDto(0, "人脸录入成功。", HttpContext.TraceIdentifier);
        }
        catch (FaceServiceException error) { _logger.LogWarning("人脸请求失败：{Message}", error.Message); return Failure(error.Message); }
        catch (HttpRequestException error) { _logger.LogError(error, "访问人脸录入服务失败。"); return Failure("人脸识别服务不可用。"); }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return Failure("人脸录入请求超时。"); }
    }

    /// <summary>删除用户的人脸及全部样本，保留原返回结构。</summary>
    /// <param name="input">需要删除人脸样本的业务用户编号。</param>
    /// <param name="cancellationToken">客户端断开连接时取消请求。</param>
    /// <returns>删除结果，Errno 为 0 表示成功，99 表示业务或服务失败。</returns>
    [HttpPost("FaceDelete")]
    public async Task<FaceManagerResultDto> FaceDelete(FaceDeleteDto input, CancellationToken cancellationToken)
    {
        try
        {
            await _recognition.DeleteAsync(input.UserId, cancellationToken);
            return new FaceManagerResultDto(0, "人脸删除成功。", HttpContext.TraceIdentifier);
        }
        catch (FaceServiceException error) { _logger.LogWarning("人脸请求失败：{Message}", error.Message); return Failure(error.Message); }
        catch (HttpRequestException error) { _logger.LogError(error, "访问人脸删除服务失败。"); return Failure("人脸识别服务不可用。"); }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return Failure("人脸删除请求超时。"); }
    }

    /// <summary>将预期失败映射到原有错误码，传输故障已在调用处记录。</summary>
    private FaceManagerResultDto Failure(string message)
    {
        return new FaceManagerResultDto(99, message, HttpContext.TraceIdentifier);
    }
}
