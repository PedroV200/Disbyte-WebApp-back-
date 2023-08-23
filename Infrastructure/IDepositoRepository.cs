using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IDepositoRepository : IGenericRepository<Deposito>
{
     public Task<IEnumerable<DepositoVista>> GetAllVistaAsync();
}