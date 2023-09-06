using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasDepositoRepository:IGenericRepository<TarifasDeposito>
{
    public Task<IEnumerable<TarifasDepositoVista>>GetAllVistaAsync();

    public Task<IEnumerable<TarifasDepositoVista>> GetAllVistaByDateAsync(string fecha);

    public Task<TarifasDeposito> GetByNearestDateAsync(string fecha, int carga_id, int paisregion_id);

}