using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// LISTED 8/8/2023 10:14AM

[ApiController]
[Route("[controller]")]
public class TarifasPolizaController : ControllerBase
{
    private readonly ILogger<TarifasPolizaController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasPolizaController(ILogger<TarifasPolizaController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tarifa Polizas")]
    public async Task<IActionResult>Post(TarifasPoliza entity)
    {
        try
        {
            var result=await _unitOfWork.TarifPoliza.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,TarifasPoliza entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifPoliza.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca ... OK
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    //[HttpDelete("{id}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try
        {
            var result=await _unitOfWork.TarifPoliza.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    

    [HttpGet(Name = "GetAll Tarifas Poliza")]
    public async Task<IEnumerable<TarifasPoliza>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifPoliza.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasPoliza> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifPoliza.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}/{paisregion}")]
    public async Task<TarifasPoliza> GetByNearestFecha(string fechahora, int paisregion)
    {
        try
        {
            return await _unitOfWork.TarifPoliza.GetByNearestDateAsync(fechahora, paisregion);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    public async Task<IEnumerable<TarifasPolizaVista>> GetAllVista()
    {
        try
        {
            return await _unitOfWork.TarifPoliza.GetAllVistaAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vistafecha/")]
    public async Task<IEnumerable<TarifasPolizaVista>> GetAllVistaByDate()
    {
        try
        {   // HH en las horas significa formato 24h.
            return await _unitOfWork.TarifPoliza.GetAllVistaByDateAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
