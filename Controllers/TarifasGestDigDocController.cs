using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// En conformidad con las tablas version "3B"   

[ApiController]
[Route("[controller]")]
public class TarifasGestDigDocController : ControllerBase
{
    private readonly ILogger<TarifasGestDigDocController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasGestDigDocController(ILogger<TarifasGestDigDocController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tarifa GestDigDoc")]
    public async Task<IActionResult>Post(TarifasGestDigDoc entity)
    {
        try
        {
            var result=await _unitOfWork.TarifGestDigDoc.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasGestDigDoc entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifGestDigDoc.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifGestDigDoc.DeleteAsync(id);
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

    

    [HttpGet(Name = "GetAll Tarifas GestDigDoc")]
    public async Task<IEnumerable<TarifasGestDigDoc>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifGestDigDoc.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasGestDigDoc> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifGestDigDoc.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}/{paisregion}")]
    public async Task<TarifasGestDigDoc> GetByNearestFecha(string fechahora, int paisregion)
    {
        try
        {
            return await _unitOfWork.TarifGestDigDoc.GetByNearestDateAsync(fechahora, paisregion);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<TarifasGestDigDocVista>>GetAllVista()
    {
        try
        {
            return await _unitOfWork.TarifGestDigDoc.GetAllVistaAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
