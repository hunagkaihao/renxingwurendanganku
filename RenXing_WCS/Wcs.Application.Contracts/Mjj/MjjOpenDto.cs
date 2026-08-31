using Volo.Abp.Application.Dtos;

namespace Wcs.Mjj;

public class MjjOpenDto : EntityDto
{
    public byte colNo { get; set; }
    public byte zyNo { get; set; }
    public byte state { get; set; }
}