using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Wcs.Caches.Models;

public class DispatchCache : Entity<int>
{
    private DispatchCache()
    {

    }

    //可见性为internal，是因为Specs的有效性必须经过领域服务来验证
    internal DispatchCache(byte cachePos, string specs)
    {
        Check.Positive(cachePos, nameof(cachePos));
        Check.NotNullOrEmpty(specs, nameof(specs));
        CachePos = cachePos;
        Specs = specs;
        TaskIdOwnIt = -1;
    }


    /// <summary>
    /// 缓存位置
    /// </summary>
    /// <value></value>
    [Required]
    public byte CachePos { get; private set; }

    /// <summary>
    /// 档案盒规格
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string Specs { get; private set; }

    /// <summary>
    /// 占用该缓存位的任务号，若没有占用，默认为-1
    /// </summary>
    /// <value></value>
    public int TaskIdOwnIt { get; set; }
}