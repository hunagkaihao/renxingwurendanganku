using Volo.Abp.Application.Dtos;

namespace Wcs.Mjj;

public class MjjStatusNmValMapDto : EntityDto
{
    public string tagName { get; set; }
    public string tagValue { get; set; }
}