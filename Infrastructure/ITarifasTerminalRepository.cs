using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasTerminalRepository : IGenericRepository<TarifasTerminal>
{
    public Task<TarifasTerminal> GetByNearestDateAsync(string fecha);
}