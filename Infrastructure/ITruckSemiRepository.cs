using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITruckSemiRepository : IGenericRepository<TruckSemi>
{
    public Task<IEnumerable<TruckSemiVista>> GetAllVistaAsync();
}
