using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasFwdRepository:IGenericRepository<TarifasFwd>
{
        public Task<TarifasFwd> GetByNearestDateAsync(string fecha);
}