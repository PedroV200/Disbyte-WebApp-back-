using Microsoft.AspNetCore.Mvc; 
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers; 

// LISTED 12/7/2023 15:41PM

[ApiController]
[Route("[controller]")]
public class TarifasDepositoController : ControllerBase
{
    private readonly ILogger<TarifasDepositoController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasDepositoController(ILogger<TarifasDepositoController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Deposito/tarifa")]
    public async Task<IActionResult>Post(TarifasDeposito entity)
    {
        try
        {
            var result=await _unitOfWork.TarifasDepositos.AddAsync(entity);
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
    public async Task<IActionResult>Put(int id,TarifasDeposito entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result=await _unitOfWork.TarifasDepositos.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifasDepositos.DeleteAsync(id);
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

    

    [HttpGet(Name = "GetAll Depositos/Tarifas")]
    public async Task<IEnumerable<TarifasDeposito>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifasDepositos.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<TarifasDeposito> Get(int id)
    {
        try
        {
            return await _unitOfWork.TarifasDepositos.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("fecha/{fechahora}")]
    public async Task<TarifasDeposito> GetByNearestFecha(string fechahora)
    {
        try
        {
            return await _unitOfWork.TarifasDepositos.GetByNearestDateAsync(fechahora);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    public async Task<IEnumerable<TarifasDepositoVista>>GetAllVista()
    {
        try
        {
            return await _unitOfWork.TarifasDepositos.GetAllVistaAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
