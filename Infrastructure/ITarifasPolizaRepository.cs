using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasPolizaRepository:IGenericRepository<TarifasPoliza>
{
    public Task<TarifasPoliza> GetByNearestDateAsync(string fecha);
}