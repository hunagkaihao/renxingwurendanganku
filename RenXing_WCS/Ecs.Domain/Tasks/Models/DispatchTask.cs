using System.ComponentModel.DataAnnotations;
using Ecs.Dispatch;
using Volo.Abp.Domain.Entities;

namespace Ecs.Tasks.Models;

public class DispatchTask : Entity<int>
{
    /// <summary>
    /// 对应的订单Code
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 托盘或物料承载物条码
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string PlateCode { get; set; } = string.Empty;

    /// <summary>
    /// 物流起点，可能是库位
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string StartNode { get; set; } = string.Empty;

    /// <summary>
    /// 物流终点，可能是库位
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string EndNode { get; set; } = string.Empty;

    /// <summary>
    /// 根据物流起点和物流终点确定的物流路线
    /// </summary>
    /// <value></value>
    public int ProcessId { get; set; }

    public EnumDispatchTaskState State { get; set; }

    /// <summary>
    /// 是否是最后一个库位盘点任务
    /// </summary>
    /// <value></value>
    public bool? LastChkOrder { get; set; }

    /// <summary>
    /// 针对出入库任务，分配的缓存
    /// </summary>
    /// <value></value>
    public int CachePos { get; set; }

    public int Priority { get; set; }

    [StringLength(50)]
    public string CreateTime { get; set; }

    public DispatchTask(int id)
    {
        Id = id;
    }
}