using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateHeaderDBRepository : IGenericRepository<EstimateHeaderDB>
{
    
    public Task<IEnumerable<EstimateHeaderDB>> GetByEstNumberLastVersAsync(int estNumber);
    public Task<EstimateHeaderDB> GetByEstNumberAnyVersAsync(int estnumber, int estVers);
    public Task<EstimateHeaderDBVista> GetByEstNumberAnyVersVistaAsync(int estnumber, int estVers);
    public Task<int> GetNextEstNumber();
    public Task<int> GetNextEstVersByEstNumber(int estNumber);
    public Task<IEnumerable<EstimateHeaderDB>> GetAllVersionsFromEstimate(int estNumbet);
    public Task<IEnumerable<EstimateHeaderDB>> GetByDescripAsync(string descrip);
    public Task<EstimateHeaderDB> GetByEstNumberLastVers_1ROW_Async(int estnumber);
    public Task<IEnumerable<TraceUser>> GetUserTraceByEstNumberUDAsync(int estnumber);
    public Task<EstimateHeaderDBVista>GetByEstNumberLastVersBySectionVistaAsync(int estnumber, int code);
}