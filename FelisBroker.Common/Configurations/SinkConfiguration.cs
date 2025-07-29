namespace FelisBroker.Common.Configurations;

public class SinkConfiguration
{
    public string? Name { get; set; }
    public List<string> Topics { get; set; } = new();
    public string? Target { get; set; }
    public string? Operation { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}