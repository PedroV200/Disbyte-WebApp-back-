using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// LISTED 8/8/2023 10:14AM

[ApiController]
[Route("[controller]")]
public class TarifasMexController : ControllerBase
{
    private readonly ILogger<TarifasMexController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasMexController(ILogger<TarifasMexController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tarifas Mex")]
    public async Task<IActionResult>Post(TarifasMex entity)
    {
        try
        {
            var result=await _unitOfWork.TarifasMexico.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasMex entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifasMexico.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifasMexico.DeleteAsync(id);
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

    

    [HttpGet(Name = "GetAll Tarifas Mexico")]
    public async Task<IEnumerable<TarifasMex>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifasMexico.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasMex> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifasMexico.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}")]
    public async Task<TarifasMex> GetByNearestFecha(string fechahora)
    {
        try
        {
            return await _unitOfWork.TarifasMexico.GetByNearestDateAsync(fechahora);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


}
