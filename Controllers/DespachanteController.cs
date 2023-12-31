﻿using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

//Listed 17_8_2023 12:20 (Agrega paisregion_id. Testgetall, put, post, delete)

[ApiController]
[Route("[controller]")]
public class DespachanteController : ControllerBase
{
    private readonly ILogger<DespachanteController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DespachanteController(ILogger<DespachanteController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Despachante")]
    public async Task<IActionResult> Post(Despachante entity)
    {
        try
        {
            var result = await _unitOfWork.Despachantes.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if (result == -1) return BadRequest("Error en el metodo AddAsync: No se pudo agregar el objeto Despachante.");
            if (result == 0) return NotFound();
            // Ok
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Despachante entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id != entity.id)
            {
                return BadRequest();
            }
            var result = await _unitOfWork.Despachantes.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if (result == 0)
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
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _unitOfWork.Despachantes.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if (result == 0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }
        catch (Exception)
        {
            throw new Exception($"Could not delete {id}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Despachante>> Get(int id)
    {
        try
        {
            var result = await _unitOfWork.Despachantes.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }
        catch (Exception)
        {
            throw new Exception($"No existe Despachante con Id {id}");
        }
    }

    [HttpGet(Name = "GetAll Despachante")]
    public async Task<IEnumerable<Despachante>> GetAll()
    {
        try
        {
            return await _unitOfWork.Despachantes.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("vista/")]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<IEnumerable<DespachanteVista>>GetAllVista()
    {
        try
        {
            return await _unitOfWork.Despachantes.GetAllVistaAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
