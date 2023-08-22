using WebApiSample.Models;

namespace WebApiSample.Infrastructure;



public interface ICnstService : IGenericService<CONSTANTES>
{

    //IEstimateDetailService estDetServ{get;}

    // COL J
    public Task<CONSTANTES> getConstantesLastVers();
    public Task<CONSTANTES> ReclaimConstantes(int id);
}