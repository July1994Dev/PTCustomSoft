using Microsoft.AspNetCore.Mvc;
using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ActorsController : ControllerBase
{
    private readonly IActorService _actorService;
    private readonly ILogger<ActorsController> _logger;

    public ActorsController(IActorService actorService, ILogger<ActorsController> logger)
    {
        _actorService = actorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Actor>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los actores");
        var actors = await _actorService.GetAllActorsAsync();
        return Ok(actors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Actor>> Get(int id)
    {
        _logger.LogInformation("Obteniendo detalles del actor con ID: {Id}", id);
        var actor = await _actorService.GetActorByIdAsync(id);
        if (actor == null)
        {
            _logger.LogWarning("Actor con ID {Id} no encontrado", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Actor no encontrado", Detail = $"No se encontró un actor con el ID {id}." });
        }
        return Ok(actor);
    }

    [HttpPost]
    public async Task<ActionResult<Actor>> Create(Actor actor)
    {
        if (actor == null)
        {
            _logger.LogWarning("Intento de crear un actor con datos inválidos");
            return BadRequest(new ProblemDetails { Status = 400, Title = "Datos de actor inválidos", Detail = "Los datos proporcionados para el actor no son válidos." });
        }

        var createdActor = await _actorService.CreateActorAsync(actor);
        _logger.LogInformation("Actor creado con ID {Id}", createdActor.ActorID);
        return CreatedAtAction(nameof(Get), new { id = createdActor.ActorID }, createdActor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Actor actor)
    {
        if (id != actor.ActorID)
        {
            _logger.LogWarning("Intento de actualizar actor con ID {Id} que no coincide", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "ID de Actor no coincide", Detail = $"El ID proporcionado {id} no coincide con el ID del actor." });
        }
        _logger.LogInformation("Actualizando actor con ID {Id}", id);
        await _actorService.UpdateActorAsync(actor);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando actor con ID {Id}", id);
        await _actorService.DeleteActorAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/upload-biography")]
    public async Task<IActionResult> UploadBiography(int id, IFormFile file)
    {
        _logger.LogInformation("Subiendo biografía para el actor con ID {Id}", id);
        var actor = await _actorService.GetActorByIdAsync(id);
        if (actor == null)
        {
            _logger.LogWarning("Actor con ID {Id} no encontrado al intentar subir biografía", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Actor no encontrado", Detail = $"No se encontró un actor con el ID {id} al intentar subir una biografía." });
        }

        if (file.Length > 0)
        {
            if (!file.ContentType.Equals("application/pdf"))
            {
                _logger.LogWarning("Tipo de archivo no permitido para biografía del actor con ID {Id}", id);
                return BadRequest(new ProblemDetails { Status = 400, Title = "Tipo de archivo no permitido", Detail = "Solo se permiten archivos PDF para la biografía." });
            }

            var folderPath = Path.Combine("uploads", "actors", "biographies");
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            // Asegura que el directorio existe
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            actor.BiographyPath = filePath;
            _logger.LogInformation("Biografía subida para el actor con ID {Id}", id);
            await _actorService.UpdateActorAsync(actor);
        }
        else
        {
            _logger.LogWarning("Archivo vacío recibido para biografía del actor con ID {Id}", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "Archivo vacío", Detail = "No se recibió ningún archivo para la biografía." });
        }

        return Ok(new { Path = actor.BiographyPath });
    }

    [HttpGet("{movieId}/actors")]
    public async Task<IActionResult> GetActorsByMovie(int movieId)
    {
        var actors = await _actorService.GetActorsByMovieId(movieId);
        if (actors == null || !actors.Any())
            return NotFound("No se encontraron actores para esta pelicula.");
        return Ok(actors);
    }
}
