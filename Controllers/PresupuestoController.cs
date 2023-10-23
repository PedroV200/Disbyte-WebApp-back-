using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
using Microsoft.AspNetCore.Authorization;
namespace WebApiSample.Controllers;

// LISTED 6/7/2023 11:16 AM

[ApiController]
[Route("[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly ILogger<PresupuestoController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEstimateService _estService;
    private readonly IPresupuestoService _presupService;

    private readonly IImportService _importService;


    private calc myCalc;

    public PresupuestoController(ILogger<PresupuestoController> logger, IUnitOfWork unitOfWork,IEstimateService estService, IPresupuestoService presupService, IImportService importService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _estService = estService;
        _presupService=presupService;
        _importService=importService;
    }

// Este endpoint es para un presupuesto nuevo. Notar que no se pasa el id
    [HttpPost(Name = "Post Presupuesto Nuevo")]
    public async Task<ActionResult<EstimateV2>>PostNewPresup(EstimateDB entity)
    {
        var result=await _presupService.submitPresupuestoNew(entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

// Este endpoint es para un presupuesto nuevo. Notar que no se pasa el id
    [HttpPost("{num}")]
    // Habilitar cuando este operativo el FRONT !!!!
   /* [Authorize("update_presup:jefe_area_finanzas")]
    [Authorize("update_presup:jefe_area_comex")]
    [Authorize("update_presup:jefe_area_sourcing")]*/
    public async Task<ActionResult<EstimateV2>>PostUpdatedPresup(int num,EstimateDB entity)
    {

        // HABILITAR cuando este operativo el front
        /*Cliente miCliente=new Cliente();
        string clientePermisos=miCliente.getClientPermisos(User.Claims.ToList());*/

        // Simulo que desde el front me vinieron los sig permisos que me habiliten a operar 
        // x todos los estados. Se testeo jefe.
        string clientePermisos="COMFINSRCBSS";

        var result=await _presupService.submitPresupuestoUpdated(num, entity,clientePermisos);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

// Este endpoint es para un presupuesto nuevo. Notar que no se pasa el id
    [HttpPost("/acalc")]
    public async Task<ActionResult<EstimateV2>>PostAcalcPresupuesto(EstimateDB entity)
    {
        var result=await _presupService.acalcPresupuesto(entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

    [HttpPost("/sim")]
    public async Task<ActionResult<EstimateV2>>PostSimulPresupuesto(EstimateDB entity)
    {
        var result=await _presupService.simulaPresupuesto(entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

    
   [HttpGet("{num}/{vers}")]
    public async Task<ActionResult<EstimateV2>>Get(int num, int vers) 
    {
       
        EstimateV2 myEst=new EstimateV2();

        myEst= await _presupService.reclaimPresupuesto(num,vers);

        if(myEst==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return myEst;
        }
    }


   [HttpGet]
    public async Task<ActionResult<List<EstimateHeaderDB>>>GetHeaders() 
    {
        var result=await _unitOfWork.EstimateHeadersDB.GetAllAsync();
        return result.ToList();
        
    }
    // 6/7/2023 Se agrega este endpoint para listar todas las versiones de un presupuesto.
    [HttpGet("{num}")]
    public async Task<ActionResult<List<EstimateHeaderDB>>>GetVersionesFromEstimate(int num) 
    {
        var result=await _unitOfWork.EstimateHeadersDB.GetAllVersionsFromEstimate(num);
        return result.ToList();
        
    }

    [HttpDelete("{estNumber}/{estVers}")]
    public async Task<int>Delete(int estNumber,int estVers)
    {
        dbutils dbHelper=new dbutils(_unitOfWork);
        return await dbHelper.deleteEstimateByNumByVers(estNumber,estVers);
    }


    [HttpGet("/importa")]
    public async Task<ActionResult<bool>>Importa(int num, int vers) 
    {
       _importService.ImportNCM_Mex("/Users/pedroaste/Downloads/FCLV4.xlsx");
       //_importService.ImportProductos("/Users/pedroaste/Downloads/Productos.xlsx");
       return true;
    }
   
}
