namespace Wcs.Dispatch;

public class AddCacheDto
{
    public byte CachePos { get; set; }

    /// <summary>
    /// 档案盒规格，若兼容每种档案盒，填入any
    /// </summary>
    /// <value></value>
    public string DASpecs { get; set; } 
}