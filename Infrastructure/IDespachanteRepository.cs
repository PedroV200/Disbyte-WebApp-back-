using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IDespachanteRepository : IGenericRepository<Despachante>
{
    public Task<IEnumerable<DespachanteVista>> GetAllVistaAsync();
}
