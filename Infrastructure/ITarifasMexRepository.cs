using WebApiSample.Models;

 namespace WebApiSample.Infrastructure;

    public interface ITarifasMexRepository : IGenericRepository<TarifasMex>
    {
            public Task<TarifasMex> GetByNearestDateAsync(string fecha);
    }