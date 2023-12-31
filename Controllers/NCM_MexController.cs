using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class NCM_MexController : ControllerBase
{
    private readonly ILogger<NCM_MexController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NCM_MexController(ILogger<NCM_MexController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post NCM_Mex")]
    public async Task<IActionResult>Post(NCM_Mex_py entity)
    {
        try
        {
            var result=await _unitOfWork.NCM_MEXs.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,NCM_Mex_py entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if (id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.NCM_MEXs.UpdateAsync(entity);
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
            var result=await _unitOfWork.NCM_MEXs.DeleteAsync(id);
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
    public async Task<NCM_Mex_py> Get(int id)
    {
        try
        {
            return await _unitOfWork.NCM_MEXs.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll_NCM_Mex")]
    public async Task<IEnumerable<NCM_Mex_py>> GetAll()
    {
        return await _unitOfWork.NCM_MEXs.GetAllAsync();
    }
}
