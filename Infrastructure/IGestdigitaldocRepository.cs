using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IGestDigitalDocRepository : IGenericRepository<GestDigitalDoc>
{
     public Task<IEnumerable<GestDigitalDocVista>> GetAllPaisAsync();
}