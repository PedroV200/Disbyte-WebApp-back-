using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IPolizaRepository : IGenericRepository<Poliza>
{
     public Task<IEnumerable<PolizaVista>> GetAllVistaAsync();
}