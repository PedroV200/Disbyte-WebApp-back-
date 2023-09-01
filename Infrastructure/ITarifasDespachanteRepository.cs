using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasDespachanteRepository : IGenericRepository<TarifasDespachante>
    {
         public Task<IEnumerable<TarifasDespachanteVista>>GetAllVistaAsync();
         public Task<TarifasDespachante> GetByNearestDateAsync(string fechahora,int paisregion_id);
    }