using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class CheckOrderResultsDto : EntityDto
{
    public List<CheckOrderResultDto> cells { get; set; } = new List<CheckOrderResultDto>();

    public CheckOrderResultsDto()
    {
        cells = new List<CheckOrderResultDto>();
    }
}