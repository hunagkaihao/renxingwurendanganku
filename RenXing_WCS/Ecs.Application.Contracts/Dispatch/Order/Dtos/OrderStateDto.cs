namespace Ecs.Dispatch;

public class OrderStateDto
{
    public string orderCode { get; set; } = string.Empty;
    public string execState { get; set; } = string.Empty;
    public string errorInfo { get; set; } = string.Empty;
    public string happenTime { get; set; } = string.Empty;
}