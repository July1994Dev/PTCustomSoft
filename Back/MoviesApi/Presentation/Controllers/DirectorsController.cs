using Microsoft.AspNetCore.Mvc;
using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DirectorsController : ControllerBase
{
    private readonly IDirectorService _directorService;
    private readonly ILogger<DirectorsController> _logger;

    public DirectorsController(IDirectorService directorService, ILogger<DirectorsController> logger)
    {
        _directorService = directorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Director>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los directores");
        var directors = await _directorService.GetAllDirectorsAsync();
        return Ok(directors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Director>> Get(int id)
    {
        _logger.LogInformation("Obteniendo detalles del director con ID: {Id}", id);
        var director = await _directorService.GetDirectorByIdAsync(id);
        if (director == null)
        {
            _logger.LogWarning("Director con ID {Id} no encontrado", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Director no encontrado", Detail = $"No se encontró un director con el ID {id}." });
        }
        return Ok(director);
    }

    [HttpPost]
    public async Task<ActionResult<Director>> Create(Director director)
    {
        if (director == null)
        {
            _logger.LogWarning("Intento de crear un director con datos inválidos");
            return BadRequest(new ProblemDetails { Status = 400, Title = "Datos de director inválidos", Detail = "Los datos proporcionados para el director no son válidos." });
        }

        var createdDirector = await _directorService.CreateDirectorAsync(director);
        _logger.LogInformation("Director creado con ID {Id}", createdDirector.DirectorID);
        return CreatedAtAction(nameof(Get), new { id = createdDirector.DirectorID }, createdDirector);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Director director)
    {
        if (id != director.DirectorID)
        {
            _logger.LogWarning("Intento de actualizar director con ID {Id} que no coincide", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "ID de Director no coincide", Detail = $"El ID proporcionado {id} no coincide con el ID del director." });
        }
        _logger.LogInformation("Actualizando director con ID {Id}", id);
        await _directorService.UpdateDirectorAsync(director);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando director con ID {Id}", id);
        await _directorService.DeleteDirectorAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/upload-biography")]
    public async Task<IActionResult> UploadBiography(int id, IFormFile file)
    {
        _logger.LogInformation("Subiendo biografía para el director con ID {Id}", id);
        var director = await _directorService.GetDirectorByIdAsync(id);
        if (director == null)
        {
            _logger.LogWarning("Director con ID {Id} no encontrado al intentar subir biografía", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Director no encontrado", Detail = $"No se encontró un director con el ID {id} al intentar subir una biografía." });
        }

        if (file.Length > 0)
        {
            if (!file.ContentType.Equals("application/pdf"))
            {
                _logger.LogWarning("Tipo de archivo no permitido para biografía del director con ID {Id}", id);
                return BadRequest(new ProblemDetails { Status = 400, Title = "Tipo de archivo no permitido", Detail = "Solo se permiten archivos PDF para la biografía." });
            }

            var folderPath = Path.Combine("uploads", "directors", "biographies");
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

            director.BiographyPath = filePath;
            _logger.LogInformation("Biografía subida para el director con ID {Id}", id);
            await _directorService.UpdateDirectorAsync(director);
        }
        else
        {
            _logger.LogWarning("Archivo vacío recibido para biografía del director con ID {Id}", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "Archivo vacío", Detail = "No se recibió ningún archivo para la biografía." });
        }

        return Ok(new { Path = director.BiographyPath });
    }
}
