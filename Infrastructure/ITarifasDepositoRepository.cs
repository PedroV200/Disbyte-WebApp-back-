using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasDepositoRepository:IGenericRepository<TarifasDeposito>
{
    public Task<IEnumerable<TarifasDepositoVista>>GetAllVistaAsync();
    public Task<TarifasDeposito> GetByNearestDateAsync(string fecha);

}