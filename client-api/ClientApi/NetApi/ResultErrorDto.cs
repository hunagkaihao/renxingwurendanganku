namespace ClientApi.NetApi
{
    public class ResultErrorDto
    {
        public int code { get; set; }

        public string? message { get; set; }

        public object? details { get; set; }
        public object? validationErrors { get; set; }
    }
}
