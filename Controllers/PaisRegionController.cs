using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class PaisRegionController : ControllerBase
{
    private readonly ILogger<PaisRegionController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PaisRegionController(ILogger<PaisRegionController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post PaisRegion")]
    public async Task<IActionResult>Post(PaisRegion entity)
    {
        try
        {
            var result=await _unitOfWork.PaisesRegiones.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,PaisRegion entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.PaisesRegiones.UpdateAsync(entity);
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
            var result=await _unitOfWork.PaisesRegiones.DeleteAsync(id);
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
    public async Task<ActionResult<PaisRegion>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.PaisesRegiones.GetByIdAsync(id);
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

    [HttpGet(Name = "GetAll PaisRegion")]
    //[Authorize("update_presup:jefe_area_finanzas")]
    //[Authorize("update_presup:jefe_area_comex")]
    //[Authorize("update_presup:jefe_area_sourcing")]
    public async Task<IEnumerable<PaisRegion>> GetAll()
    {
        //Cliente miCliente=new Cliente();

        //string clientePermisos=miCliente.getClientPermisos(User.Claims.ToList());

        try
        {
            return await _unitOfWork.PaisesRegiones.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
