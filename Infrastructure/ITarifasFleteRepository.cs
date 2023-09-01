using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasFleteRepository:IGenericRepository<TarifasFlete>
{
    public Task<IEnumerable<TarifasFleteVista>>GetAllVistaAsync();
    public Task<TarifasFlete> GetByNearestDateAsync(string fecha,int carga,int paisregion_id);
}