using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Wcs.Jobs.Models;

public class DispatchJobCmd : Entity<int>
{
    /// <summary>
    ///命令类的类名
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string JobCmdClassName { get; set; } = string.Empty;

    /// <summary>
    /// 对命令的描述
    /// </summary>
    /// <value></value>
    [StringLength(100)]
    public string Describe { get; set; }

    public DispatchJobCmd() { }

    public DispatchJobCmd(int id, string stepCmdClassName, string describe)
    {
        Id = id;
        JobCmdClassName = stepCmdClassName;
        Describe = describe;
    }
}