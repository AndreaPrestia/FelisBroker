namespace FelisBroker.Interfaces.Sources;

public interface IDataSource
{
    Task StartAsync();
    Task StopAsync();
}