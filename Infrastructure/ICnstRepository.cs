using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ICnstRepository : IGenericRepository<CONSTANTES>
{
    public Task<CONSTANTES> GetLastIdAsync();
}