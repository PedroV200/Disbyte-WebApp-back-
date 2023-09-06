using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasBancoRepository : IGenericRepository<TarifasBanco>
    {
         public Task<IEnumerable<TarifasBancoVista>>GetAllVistaAsync();
         public Task<IEnumerable<TarifasBancoVista>>GetAllVistaByDateAsync(string fecha);
         public Task<TarifasBanco> GetByNearestDateAsync(string fechahora, int paisregion_id);
    }