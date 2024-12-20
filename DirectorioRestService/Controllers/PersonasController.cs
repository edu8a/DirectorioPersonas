using DirectorioRestService.Models;
using DirectorioRestService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirectorioRestService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly DirectorioService _directorioService;

        public PersonasController(DirectorioService directorioService)
        {
            _directorioService = directorioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var personas = await _directorioService.GetPersonasAsync();
            return Ok(personas);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Persona persona)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _directorioService.AddPersonaAsync(persona);
                return CreatedAtAction(nameof(Get), new { id = persona.Id }, persona);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al crear la persona.", Error = ex.Message });
            }
        }


        [HttpDelete("{identificacion}")]
        public async Task<IActionResult> DeleteByIdentificacion(string identificacion)
        {
            try
            {
                await _directorioService.DeletePersonaByIdentificacionAsync(identificacion);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al eliminar la persona.", Error = ex.Message });
            }
        }

        [HttpGet("{identificacion}")]
        public async Task<IActionResult> GetByIdentificacion(string identificacion)
        {
            var persona = await _directorioService.GetPersonaByIdentificacionAsync(identificacion);
            if (persona == null)
            {
                return NotFound(new { Message = $"No se encontró persona con identificación: {identificacion}" });
            }

            return Ok(persona);
        }
    }

}
