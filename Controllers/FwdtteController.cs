using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 17_8_2023 (Agrega paisregion_id. Test put, post, delete y getall)

[ApiController]
[Route("[controller]")]
public class FwdtteController : ControllerBase
{
    private readonly ILogger<FwdtteController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public FwdtteController(ILogger<FwdtteController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Fowarders")]
    public async Task<IActionResult>Post(Fwdtte entity)
    {
        try
        {
            var result=await _unitOfWork.Fwds.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Fwdtte entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.Fwds.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca ... OK
            return Ok(result);
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try
        {
            var result=await _unitOfWork.Fwds.DeleteAsync(id);
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
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Fwdtte>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.Fwds.GetByIdAsync(id);
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
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll Fowarders")]
    public async Task<IEnumerable<Fwdtte>> GetAll()
    {
        try
        {
            return await _unitOfWork.Fwds.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<FwdtteVista>>GetAllVista()
    {
        try
        {
            return await _unitOfWork.Fwds.GetAllVistaAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
