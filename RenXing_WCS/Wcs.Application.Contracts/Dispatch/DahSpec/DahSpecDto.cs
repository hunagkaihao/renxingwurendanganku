using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class DahSpecDto : EntityDto<int>
{
    /// <summary>
    /// 档案盒规格号
    /// </summary>
    /// <value></value>
    public string SpecCode { get; set; }

    /// <summary>
    /// 档案盒规格名
    /// </summary>
    /// <value></value>
    public string SpecName { get; set; }

    /// <summary>
    /// 档案盒规格值（用于PLC）
    /// </summary>
    /// <value></value>
    public int SpecValue { get; set; }
}