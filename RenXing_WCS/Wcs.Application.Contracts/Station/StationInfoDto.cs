using Volo.Abp.Application.Dtos;

namespace Wcs.Station;

public class StationInfoDto : EntityDto
{
    public string StaInfoName { get; set; }

    public string StaInformation { get; set; }
}