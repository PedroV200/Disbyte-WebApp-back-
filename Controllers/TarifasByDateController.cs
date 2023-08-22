using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// LISTED 8/8/2023 10:14AM

[ApiController]
[Route("[controller]")]
public class TarifasByDateController : ControllerBase
{
    private readonly ILogger<TarifasByDateController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasByDateController(ILogger<TarifasByDateController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post TarifaPorFecha")]
    public async Task<IActionResult>Post(TarifasByDate entity)
    {
        try
        {
            var result=await _unitOfWork.TarifasPorFecha.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasByDate entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifasPorFecha.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifasPorFecha.DeleteAsync(id);
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

    

    [HttpGet(Name = "GetAll TarifasPorFecha")]
    public async Task<IEnumerable<TarifasByDate>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifasPorFecha.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasByDate> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifasPorFecha.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}")]
    public async Task<TarifasByDate> GetByNearestFecha(string fechahora)
    {
        try
        {
            return await _unitOfWork.TarifasPorFecha.GetByNearestDateAsync(fechahora);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
