using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
using Microsoft.AspNetCore.Authorization;

namespace WebApiSample.Controllers;

// LISTED 27/10/2023
// Para consultar fecha / hora de compilacion embebida en el assembly.    

[ApiController]
[Route("[controller]")]
public class BuildDateController : ControllerBase
{
    private readonly ILogger<BuildDateController> _logger;

    public IBuilDateService _buildDateService;

    public BuildDateController(ILogger<BuildDateController> logger, IBuilDateService miBuildDateService)
    {
        _logger = logger;
        _buildDateService=miBuildDateService;
    }

    
    [HttpGet]
    //[Authorize("put:sample-role-admin-messages")]
    public async Task<string> GetAll()
    {
        try
        {
            return _buildDateService.getBuildDate();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    
}
