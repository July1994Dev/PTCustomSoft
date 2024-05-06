using Microsoft.AspNetCore.Mvc;
using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenresController> _logger;

    public GenresController(IGenreService genreService, ILogger<GenresController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los géneros");
        var genres = await _genreService.GetAllGenresAsync();
        return Ok(genres);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> Get(int id)
    {
        _logger.LogInformation("Obteniendo detalles del género con ID: {Id}", id);
        var genre = await _genreService.GetGenreByIdAsync(id);
        if (genre == null)
        {
            _logger.LogWarning("Género con ID {Id} no encontrado", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Género no encontrado", Detail = $"No se encontró un género con el ID {id}." });
        }
        return Ok(genre);
    }

    [HttpPost]
    public async Task<ActionResult<Genre>> Create(Genre genre)
    {
        if (genre == null)
        {
            _logger.LogWarning("Intento de crear un género con datos inválidos");
            return BadRequest(new ProblemDetails { Status = 400, Title = "Datos de género inválidos", Detail = "Los datos proporcionados para el género no son válidos." });
        }

        var createdGenre = await _genreService.CreateGenreAsync(genre);
        _logger.LogInformation("Género creado con ID {Id}", createdGenre.GenreID);
        return CreatedAtAction(nameof(Get), new { id = createdGenre.GenreID }, createdGenre);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Genre genre)
    {
        if (id != genre.GenreID)
        {
            _logger.LogWarning("Intento de actualizar género con ID {Id} que no coincide", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "ID de Género no coincide", Detail = $"El ID proporcionado {id} no coincide con el ID del género." });
        }
        _logger.LogInformation("Actualizando género con ID {Id}", id);
        await _genreService.UpdateGenreAsync(genre);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando género con ID {Id}", id);
        await _genreService.DeleteGenreAsync(id);
        return NoContent();
    }
}
