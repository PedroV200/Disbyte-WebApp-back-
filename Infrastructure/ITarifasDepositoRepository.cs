using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasDepositoRepository:IGenericRepository<TarifasDeposito>
{
    public Task<TarifasDeposito> GetByNearestDateAsync(string fecha);

}