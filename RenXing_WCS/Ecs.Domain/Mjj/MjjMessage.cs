namespace Ecs.Mjj;

public class MjjMessage
{
    public string Cmd { get; set; } = string.Empty;

    public object Para { get; set; } = new object();

    public string ResponseChannel { get; set; } = string.Empty;
}