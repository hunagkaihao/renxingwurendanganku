using Volo.Abp.Application.Dtos;

namespace Wcs;

public class ResponseDto : EntityDto
{
    public bool success { get; set; }   

    public string message { get; set; }
}