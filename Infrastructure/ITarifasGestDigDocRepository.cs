using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasGestDigDocRepository : IGenericRepository<TarifasGestDigDoc>
    {
         public Task<TarifasGestDigDoc> GetByNearestDateAsync(string fechahora);
    }