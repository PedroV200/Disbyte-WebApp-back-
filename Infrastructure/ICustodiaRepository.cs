using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ICustodiaRepository : IGenericRepository<Custodia>
{
     public Task<IEnumerable<CustodiaVista>> GetAllPaisAsync();
}