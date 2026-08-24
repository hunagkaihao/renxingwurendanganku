using Volo.Abp.Application.Dtos;

namespace Ecs.Mjj;

public class MjjStatusDto : EntityDto
{
    public string quNo { get; set; } = "error";
    public string temp { get; set; } = "error";
    public string hum { get; set; } = "error";
    public string pm2_5 { get; set; } = "error";
    public string pm10 { get; set; } = "error";
    public string tvoc { get; set; } = "error";
    public string co2 { get; set; } = "error";
    public string colNo { get; set; } = "error";
    public string mjjZTLX { get; set; } = "error";
    public string mjjZTLXName { get; set; } = "error";
    public string columnDWZT_changed { get; set; } = "error";
    public string data { get; set; } = "error";
    public string isBJ { get; set; } = "error";
    public string isLock { get; set; } = "error";
    public string isVent { get; set; } = "error";
    public string isPower { get; set; } = "error";
    public string isZDKJ { get; set; } = "error";
    public string columnStatus { get; set; } = "error";
}