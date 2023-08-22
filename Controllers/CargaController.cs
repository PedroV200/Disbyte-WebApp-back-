using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 17_8_2023 15:12 (se testeo put, post, getall, delete, getbyid)

[ApiController]
[Route("[controller]")]
public class CargaController : ControllerBase
{
    private readonly ILogger<CargaController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CargaController(ILogger<CargaController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Carga")]
    public async Task<IActionResult>Post(Carga entity)
    {
        try
        {
            var result=await _unitOfWork.micarga.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,Carga entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            //if(id!=entity.id)
            //{
                //return BadRequest();
            //}
            var result=await _unitOfWork.micarga.UpdateAsync(entity);
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

        var result=await _unitOfWork.micarga.DeleteAsync(id);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Carga>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.micarga.GetByIdAsync(id);
            if(result==null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll Carga")]
    public async Task<IEnumerable<Carga>> GetAll()
    {
        try
        {
            return await _unitOfWork.micarga.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
