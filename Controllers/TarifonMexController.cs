using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;
namespace WebApiSample.Controllers;

// LISTED 6/7/2023 11:16 AM

[ApiController]
[Route("[controller]")]
public class TarifonMexController : ControllerBase
{
    private readonly ILogger<TarifonMexController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITarifonMexService _tarifonmexService;


    private calc myCalc;

    public TarifonMexController(ILogger<TarifonMexController> logger, IUnitOfWork unitOfWork,ITarifonMexService tarfonMex)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tarifonmexService=tarfonMex;
    }

    [HttpGet]
     public async Task<ActionResult<TarifonMex>>tarifonGet()
     {
        var res=await _tarifonmexService.getTarifon();
        if(res==null)
        {
             return BadRequest(_tarifonmexService.getLastErr()); 
        }
        return res;
     }

     [HttpGet("{carga_id}")]
     public async Task<ActionResult<GastosLocales>>tarifonGetByCarga(int carga_id)
     {
        var res=await _tarifonmexService.getGloc(carga_id);
        if(res==null)
        {
             return BadRequest(_tarifonmexService.getLastErr()); 
        }
        return res;
     }

    [HttpPost]
    public async Task<IActionResult>Post(TarifonMex entity)
    {
        try
        {
            var result=await _tarifonmexService.putTarifon(entity);
            // Cero filas afectada ... we have problems.
            if(result==false)
            {
                return BadRequest(_tarifonmexService.getLastErr()); 
            }
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
// Este endpoint es para un presupuesto nuevo. Notar que no 
}