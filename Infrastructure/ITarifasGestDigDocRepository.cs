using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasGestDigDocRepository : IGenericRepository<TarifasGestDigDoc>
    {
         public Task<IEnumerable<TarifasGestDigDocVista>> GetAllVistaAsync();
         public Task<TarifasGestDigDoc> GetByNearestDateAsync(string fechahora, int paisregion_id);
    }