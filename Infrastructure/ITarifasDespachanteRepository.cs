using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasDespachanteRepository : IGenericRepository<TarifasDespachante>
    {
         public Task<TarifasDespachante> GetByNearestDateAsync(string fechahora);
    }