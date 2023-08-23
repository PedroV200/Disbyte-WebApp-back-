using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITerminalRepository:IGenericRepository<Terminal>
{
     public Task<IEnumerable<TerminalVista>> GetAllPaisAsync();
}