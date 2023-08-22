using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasBancoRepository : IGenericRepository<TarifasBanco>
    {
         public Task<TarifasBanco> GetByNearestDateAsync(string fechahora);
    }