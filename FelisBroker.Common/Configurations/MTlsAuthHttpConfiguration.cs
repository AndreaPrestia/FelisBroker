namespace FelisBroker.Common.Configurations;

public class MTlsAuthHttpConfiguration : HttpAuthConfiguration
{
    public bool RequireClientCertificate { get; set; } = true;
    public List<string>? AllowedThumbprints { get; set; }
    public List<string>? AllowedSubjects { get; set; }
}