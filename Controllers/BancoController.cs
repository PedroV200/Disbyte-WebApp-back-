using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
using Microsoft.AspNetCore.Authorization;

namespace WebApiSample.Controllers;

// LISTED 19_7_2023 7:48PM
// 14_08_2023 Probado el endpoint de listar filtrado por pais. Ojo, se usa ID ..... 
// caso contrario (si quiero usar el nombre del pais en string) necesito una dobleconsulta.
// y perderia la separacion entre modulos.   

[ApiController]
[Route("[controller]")]
public class BancoController : ControllerBase
{
    private readonly ILogger<BancoController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BancoController(ILogger<BancoController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Banco")]
    public async Task<IActionResult>Post(Banco entity)
    {
        try
        {
            var result=await _unitOfWork.Bancos.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result == -1) return BadRequest("Error en el metodo AddAsync: No se pudo agregar el objeto Banco.");
            if(result == 0) return NotFound();
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Banco entity)
    {
        try {
            // Controlo que el id sea consistente.
            if(id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.Bancos.UpdateAsync(entity);
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
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try { 
            var result=await _unitOfWork.Bancos.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }catch (Exception)
        {
            throw new Exception($"Could not delete {id}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Banco>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.Bancos.GetByIdAsync(id);
            if(result==null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }catch (Exception)
        {
            throw new Exception($"No existe Banco con Id {id}");
        }
    }




    [HttpGet(Name = "GetAll Banco")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<Banco>> GetAll()
    {
        try
        {
            return await _unitOfWork.Bancos.GetAllAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("pais/{pais}")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<Banco>> GetByPais(int pais)
    {
        try
        {
            return await _unitOfWork.Bancos.GetByPaisAsync(pais);
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<BancoVista>> GetAllPais()
    {
        try
        {
            return await _unitOfWork.Bancos.GetAllVistaAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
