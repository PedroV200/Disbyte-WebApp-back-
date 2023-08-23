using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IBancoRepository : IGenericRepository<Banco>
{
    public Task<IEnumerable<Banco>> GetByPaisAsync(int paisreg);
    public Task<IEnumerable<BancoVista>> GetAllVistaAsync();
}