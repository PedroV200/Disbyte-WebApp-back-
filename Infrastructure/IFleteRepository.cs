using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IFleteRepository:IGenericRepository<Flete>
{
    public Task<IEnumerable<FleteVista>> GetAllVistaAsync();
}