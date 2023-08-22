using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasFleteRepository:IGenericRepository<TarifasFlete>
{
    public Task<TarifasFlete> GetByNearestDateAsync(string fecha);
}