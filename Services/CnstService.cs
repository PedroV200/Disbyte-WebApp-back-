namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class CnstService: ICnstService
{
    IUnitOfWork _unitOfWork;
    public CnstService(IUnitOfWork unitOfWork)
    {
       _unitOfWork=unitOfWork;
    }
// Pide la tabla de constantes y la carga en un objeto CONSTATES para que Estimate y EstimateDetail puedan usarlo
    public async Task<CONSTANTES> getConstantesLastVers()
    {
        CONSTANTES misConstantes=new CONSTANTES();
        misConstantes=await _unitOfWork.Constantes.GetLastIdAsync();
        return misConstantes;
    }

    public async Task<CONSTANTES> ReclaimConstantes(int id)
    {
        CONSTANTES misConstantes=new CONSTANTES();
        misConstantes=await _unitOfWork.Constantes.GetByIdAsync(id);
        return misConstantes;
    }
}
