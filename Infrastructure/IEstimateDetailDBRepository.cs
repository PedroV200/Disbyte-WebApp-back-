using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateDetailDBRepository : IGenericRepository<EstimateDetailDB>
{
     Task<IEnumerable<EstimateDetailDB>>GetAllByIdEstHeadersync(int code);
     public Task<IEnumerable<EstimateDetailDBVista>>GetAllByIdEstHeaderVistasync(int Id);
      public Task<int> DeleteByIdEstHeaderAsync(int IdEstHeader);
      public Task<int> ClearFlagsByEstimateHeaderIdAsync(int id);
     
}