using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasPolizaRepository:IGenericRepository<TarifasPoliza>
{
    public Task<IEnumerable<TarifasPolizaVista>> GetAllVistaAsync();
    public Task<IEnumerable<TarifasPolizaVista>> GetAllVistaByDateAsync(string fecha);
    public Task<TarifasPoliza> GetByNearestDateAsync(string fecha, int paisregion_id);
}