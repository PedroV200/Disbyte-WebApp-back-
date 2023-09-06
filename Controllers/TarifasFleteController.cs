using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 12/06/2023 12:27PM

[ApiController]
[Route("[controller]")]
public class TarifasFleteController : ControllerBase
{
    private readonly ILogger<TarifasFleteController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasFleteController(ILogger<TarifasFleteController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Flete Local")]
    public async Task<IActionResult>Post(TarifasFlete entity)
    {
        try
        {
            var result=await _unitOfWork.TarifFlete.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasFlete entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if(id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.TarifFlete.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifFlete.DeleteAsync(id);
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TarifasFlete>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.TarifFlete.GetByIdAsync(id);
            if(result==null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet(Name = "GetAll Tarifas flete local")]
    public async Task<IEnumerable<TarifasFlete>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifFlete.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}/{carga}/{paisorig}")]
    public async Task<TarifasFlete> GetByNearestFecha(string fechahora, int carga, int paisorig)
    {
        try
        {
            return await _unitOfWork.TarifFlete.GetByNearestDateAsync(fechahora,carga,paisorig);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    public async Task<IEnumerable<TarifasFleteVista>> GetAllVista()
    {
        try
        {
            return await _unitOfWork.TarifFlete.GetAllVistaAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    [HttpGet("vistafecha/")]
    public async Task<IEnumerable<TarifasFleteVista>> GetAllVistaByDate()
    {
        try
        {   // HH en las horas significa formato 24h.
            return await _unitOfWork.TarifFlete.GetAllVistaByDateAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}