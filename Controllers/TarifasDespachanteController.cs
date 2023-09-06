using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// LISTED 8/8/2023 10:14AM    

[ApiController]
[Route("[controller]")]
public class TarifasDespachanteController : ControllerBase
{
    private readonly ILogger<TarifasDespachanteController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasDespachanteController(ILogger<TarifasDespachanteController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tarifa Despachante")]
    public async Task<IActionResult>Post(TarifasDespachante entity)
    {
        try
        {
            var result=await _unitOfWork.TarifDespa.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasDespachante entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifDespa.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifDespa.DeleteAsync(id);
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

    

    [HttpGet(Name = "GetAll Tarifas Despachante")]
    public async Task<IEnumerable<TarifasDespachante>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifDespa.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasDespachante> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifDespa.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}/{paisregion}")]
    public async Task<TarifasDespachante> GetByNearestFecha(string fechahora, int paisregion)
    {
        try
        {
            return await _unitOfWork.TarifDespa.GetByNearestDateAsync(fechahora,paisregion);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    public async Task<IEnumerable<TarifasDespachanteVista>>GetAllVista()
    {
        try
        {
            return await _unitOfWork.TarifDespa.GetAllVistaAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vistafecha/")]
    public async Task<IEnumerable<TarifasDespachanteVista>> GetAllVistaByDate()
    {
        try
        {   // HH en las horas significa formato 24h.
            return await _unitOfWork.TarifDespa.GetAllVistaByDateAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
