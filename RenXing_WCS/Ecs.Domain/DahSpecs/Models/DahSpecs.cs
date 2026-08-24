using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Ecs.DahSpecss.Models;

public class DahSpecs : Entity<int>
{
    private DahSpecs()
    {

    }

    internal DahSpecs(string specCode, string specName, int specVal)
    {
        Check.NotNullOrEmpty(specCode, nameof(specCode));
        Check.NotNullOrEmpty(specName, nameof(specName));
        Check.Range(specVal, nameof(specVal), 0);
        SpecCode = specCode;
        SpecName = specName;
        SpecValue = specVal;
    }

    /// <summary>
    /// 档案盒规格号
    /// </summary>
    /// <value></value>
    [Required]
    [StringLength(50)]
    public string SpecCode { get; set; }

    /// <summary>
    /// 档案盒规格名
    /// </summary>
    /// <value></value>
    [Required]
    [StringLength(50)]
    public string SpecName { get; set; }

    /// <summary>
    /// 档案盒规格值（用于PLC）
    /// </summary>
    /// <value></value>
    [Required]
    public int SpecValue { get; set; }
}