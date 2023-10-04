using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifonMexService : IGenericService<TarifonMex>
{
    public Task<TarifonMex>getTarifon();
    public Task<bool>putTarifon(TarifonMex miTarifom);
    public string getLastErr();
}