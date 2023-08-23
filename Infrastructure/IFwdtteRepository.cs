using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IFwdtteRepository:IGenericRepository<Fwdtte>
{
    public Task<IEnumerable<FwdtteVista>> GetAllVistaAsync();
}