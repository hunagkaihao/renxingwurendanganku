namespace ClientApi.NetApi
{
    public class ResultDto
    {
        public bool success { get; set; }

        public ResultErrorDto? error { get; set; }

        public object? result { get; set; }
        public bool unAuthorizedRequest { get; set; }
    }
}
