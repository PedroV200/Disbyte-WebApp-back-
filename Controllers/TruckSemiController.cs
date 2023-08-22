using Microsoft.AspNetCore.Mvc;
using WebApiSample.Infrastructure;
using WebApiSample.Models;

//LISTED 17_8_2023 (Se agrego paisregion_id. Se teste post put getall y delete)

namespace WebApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class TruckSemiController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TruckSemiController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "Post tipo de camion remolque")]
        public async Task<IActionResult> Post(TruckSemi entity)
        {
            try
            {
                var result = await _unitOfWork.Camiones.AddAsync(entity);
                // Cero filas afectada ... we have problems.
                if (result == 0)
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
        public async Task<IActionResult> Put(int id, TruckSemi entity)
        {
            try
            {
                // Controlo que el id sea consistente.
                if (id != entity.id)
                {
                    return BadRequest();
                }
                var result = await _unitOfWork.Camiones.UpdateAsync(entity);
                // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
                if (result == 0)
                {
                    return NotFound();
                }
                // Si llegue hasta aca ... OK
                return Ok(result);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _unitOfWork.Camiones.DeleteAsync(id);
                // Ninguna fila afectada .... El id no existe
                if (result == 0)
                {
                    return NotFound();
                }
                // Si llegue hasta aca, OK
                return Ok(result);
            }catch(Exception)
            {
                throw new Exception($"Could not delete {id}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TruckSemi>> Get(int id)
        {
            try
            {
                var result = await _unitOfWork.Camiones.GetByIdAsync(id);
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
                throw new Exception($"No existe Carga con Id {id}");
            }
        }

        [HttpGet(Name = "GetAll Tipo camion remolque")]
        public async Task<IEnumerable<TruckSemi>> GetAll()
        {
            try
            {
                return await _unitOfWork.Camiones.GetAllAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
