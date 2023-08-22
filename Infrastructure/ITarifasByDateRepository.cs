using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasByDateRepository : IGenericRepository<TarifasByDate>
    {
         public Task<TarifasByDate> GetByNearestDateAsync(string fechahora);
    }