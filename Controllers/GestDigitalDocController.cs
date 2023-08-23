using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

//LISTED 17_8_2023 (Se agrego paisregion_id. Se teste put, post, delete y getall)

[ApiController]
[Route("[controller]")]
public class GestDigitalDocController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GestDigitalDocController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post GestDigitalDoc")]
    public async Task<IActionResult>Post(GestDigitalDoc entity)
    {
        try
        {
            var result=await _unitOfWork.GestDigDoc.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,GestDigitalDoc entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.GestDigDoc.UpdateAsync(entity);
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
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try
        {
            var result=await _unitOfWork.GestDigDoc.DeleteAsync(id);
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
    public async Task<ActionResult<GestDigitalDoc>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.GestDigDoc.GetByIdAsync(id);
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

    [HttpGet(Name = "GetAll Gestdigitaldoc")]
    public async Task<IEnumerable<GestDigitalDoc>> GetAll()
    {
        try
        {
            return await _unitOfWork.GestDigDoc.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("pais/")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<GestDigitalDocVista>> GetAllPais()
    {
        try
        {
            return await _unitOfWork.GestDigDoc.GetAllPaisAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
