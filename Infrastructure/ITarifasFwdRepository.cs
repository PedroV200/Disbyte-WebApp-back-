using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasFwdRepository:IGenericRepository<TarifasFwd>
{
        public Task<TarifasFwd> GetByNearestDateAsync(string fech,int carga, int pais_dest, int pais_orig);
        public Task<IEnumerable<TarifasFwdVista>> GetAllVistaAsync();
        public Task<IEnumerable<TarifasFwdVista>> GetAllVistaByDateAsync(string fecha);
}