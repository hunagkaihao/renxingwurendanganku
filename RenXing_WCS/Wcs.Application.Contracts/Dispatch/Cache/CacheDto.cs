using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class CacheDto : EntityDto<int>
{
    public byte CachePos { get; set; }

    /// <summary>
    /// 档案盒规格，若兼容每种档案盒，填入any
    /// </summary>
    /// <value></value>
    public string Specs { get; set; } 

    /// <summary>
    /// 占用该缓存位的任务号，若没有占用，默认为0
    /// </summary>
    /// <value></value>
    public int TaskIdOwnIt { get; set; } 
}