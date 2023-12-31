﻿using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class EstimateDetailController : ControllerBase
{
    private readonly ILogger<EstimateDetailController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public EstimateDetailController(ILogger<EstimateDetailController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post EstemateDetails")]
    public async Task<IActionResult> Post(EstimateDetailDB entity)
    {
        var result = await _unitOfWork.EstimateDetailsDB.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if (result == 0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, EstimateDetailDB entity)
    {
        // Controlo que el id sea consistente.
        if (id != entity.id)
        {
            return BadRequest();
        }
        var result = await _unitOfWork.EstimateDetailsDB.UpdateAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if (result == 0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _unitOfWork.EstimateDetailsDB.DeleteAsync(id);
        // Ninguna fila afectada .... El id no existe
        if (result == 0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EstimateDetailDB>> Get(int id)
    {
        var result = await _unitOfWork.EstimateDetailsDB.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        else
        {
            return result;
        }
    }

    [HttpGet(Name = "GetAll EstemateDetails")]
    public async Task<IEnumerable<EstimateDetailDB>> GetAll()
    {
        return await _unitOfWork.EstimateDetailsDB.GetAllAsync();
    }
}

