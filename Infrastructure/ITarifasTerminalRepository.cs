using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasTerminalRepository : IGenericRepository<TarifasTerminal>
{
    public Task<IEnumerable<TarifasTerminalVista>> GetAllVistaAsync();

    public Task<IEnumerable<TarifasTerminalVista>> GetAllVistaByDateAsync(string fecha);

    public Task<TarifasTerminal> GetByNearestDateAsync(string fecha, int carga_id, int paisregion_id);
}