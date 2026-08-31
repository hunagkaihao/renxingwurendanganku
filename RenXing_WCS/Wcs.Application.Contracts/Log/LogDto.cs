using Volo.Abp.Application.Dtos;

namespace Wcs.Log
{
    public class LogDto : EntityDto
    {
        public int id { get; set; }

        public string date { get; set; }

        public string grade { get; set; }

        public string message { get; set; }

        public string source { get; set; }
    }
}