using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// LISTED 17_8_2023 10:31AM 

[ApiController]
[Route("[controller]")]
public class TarifasTerminalController : ControllerBase
{
    private readonly ILogger<TarifasTerminalController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasTerminalController(ILogger<TarifasTerminalController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tarifa Terminal")]
    public async Task<IActionResult>Post(TarifasTerminal entity)
    {
        try
        {
            var result=await _unitOfWork.TarifTerminal.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasTerminal entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifTerminal.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifTerminal.DeleteAsync(id);
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

    

    [HttpGet(Name = "GetAll Tarifas Terminales")]
    public async Task<IEnumerable<TarifasTerminal>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifTerminal.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasTerminal> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifTerminal.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}")]
    public async Task<TarifasTerminal> GetByNearestFecha(string fechahora)
    {
        try
        {
            return await _unitOfWork.TarifTerminal.GetByNearestDateAsync(fechahora);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
