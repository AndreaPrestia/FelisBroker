using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Services;

public interface IOriginService
{
    Task<IList<OriginConfiguration>> GetOrigins();
}