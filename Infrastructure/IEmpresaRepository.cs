using WebApiSample.Models;
namespace WebApiSample.Infrastructure;

public interface IEmpresaRepository : IGenericRepository<Empresa>
{
    public Task<IEnumerable<EmpresaVista>> GetAllPaisAsync();
}
