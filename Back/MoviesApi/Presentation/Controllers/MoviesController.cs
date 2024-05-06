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
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todas las películas");
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> Get(int id)
    {
        _logger.LogInformation("Obteniendo detalles de la película con ID: {Id}", id);
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
        {
            _logger.LogWarning("Película con ID {Id} no encontrada", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Película no encontrada", Detail = $"No se encontró una película con el ID {id}." });
        }
        return Ok(movie);
    }

    [HttpPost]
    public async Task<ActionResult<Movie>> Create(Movie movie)
    {
        if (movie == null)
        {
            _logger.LogWarning("Intento de crear una película con datos inválidos");
            return BadRequest(new ProblemDetails { Status = 400, Title = "Datos de película inválidos", Detail = "Los datos proporcionados para la película no son válidos." });
        }

        var createdMovie = await _movieService.CreateMovieAsync(movie);
        _logger.LogInformation("Película creada con ID {Id}", createdMovie.MovieID);
        return CreatedAtAction(nameof(Get), new { id = createdMovie.MovieID }, createdMovie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Movie movie)
    {
        if (id != movie.MovieID)
        {
            _logger.LogWarning("Intento de actualizar película con ID {Id} que no coincide", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "ID de Película no coincide", Detail = $"El ID proporcionado {id} no coincide con el ID de la película." });
        }
        _logger.LogInformation("Actualizando película con ID {Id}", id);
        await _movieService.UpdateMovieAsync(movie);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando película con ID {Id}", id);
        await _movieService.DeleteMovieAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/upload-poster")]
    public async Task<IActionResult> UploadPoster(int id, IFormFile file)
    {
        _logger.LogInformation("Subiendo póster para la película con ID {Id}", id);
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
        {
            _logger.LogWarning("Película con ID {Id} no encontrada al intentar subir póster", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Película no encontrada", Detail = $"No se encontró una película con el ID {id} al intentar subir un póster." });
        }

        if (file.Length > 0)
        {
            var folderPath = Path.Combine("uploads", "posters");
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            movie.PosterPath = filePath;
            _logger.LogInformation("Póster subido para la película con ID {Id}", id);
            await _movieService.UpdateMovieAsync(movie);
        }
        else
        {
            _logger.LogWarning("Archivo vacío recibido para póster de la película con ID {Id}", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "Archivo vacío", Detail = "No se recibió ningún archivo para el póster." });
        }

        return Ok(new { Path = movie.PosterPath });
    }

    [HttpPost("{id}/upload-trailer")]
    public async Task<IActionResult> UploadTrailer(int id, IFormFile file)
    {
        _logger.LogInformation("Subiendo tráiler para la película con ID {Id}", id);
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
        {
            _logger.LogWarning("Película con ID {Id} no encontrada al intentar subir tráiler", id);
            return NotFound(new ProblemDetails { Status = 404, Title = "Película no encontrada", Detail = $"No se encontró una película con el ID {id} al intentar subir un tráiler." });
        }

        if (file.Length > 0)
        {
            var folderPath = Path.Combine("uploads", "trailers");
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            movie.TrailerPath = filePath;
            _logger.LogInformation("Tráiler subido para la película con ID {Id}", id);
            await _movieService.UpdateMovieAsync(movie);
        }
        else
        {
            _logger.LogWarning("Archivo vacío recibido para tráiler de la película con ID {Id}", id);
            return BadRequest(new ProblemDetails { Status = 400, Title = "Archivo vacío", Detail = "No se recibió ningún archivo para el tráiler." });
        }

        return Ok(new { Path = movie.TrailerPath });
    }

    [HttpGet("{directorId}/movies")]
    public async Task<IActionResult> GetMoviesByDirector(int directorId)
    {
        var movies = await _movieService.GetMoviesByDirectorId(directorId);
        if (movies == null || !movies.Any())
        { 
            return NotFound("No se encontraron peliculas de este director.");
        }
        return Ok(movies);
    }

    [HttpPost("{movieId}/actors/{actorId}")]
    public async Task<IActionResult> AddActorToMovie(int movieId, int actorId)
    {
        _logger.LogInformation("Intentando añadir actor con ID {ActorId} a la película con ID {MovieId}", actorId, movieId);
        try
        {
            await _movieService.AddActorToMovie(movieId, actorId);
            _logger.LogInformation("Actor añadido correctamente a la película.");
            return Ok("Actor añadido a la película correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al añadir actor a la película: {Error}", ex.Message);
            return BadRequest(new ProblemDetails { Status = 400, Title = "Error al añadir actor", Detail = ex.Message });
        }
    }

    [HttpGet("{movieId}/actors")]
    public async Task<ActionResult<IEnumerable<Actor>>> GetActorsByMovieId(int movieId)
    {
        _logger.LogInformation("Obteniendo actores para la película con ID {MovieId}", movieId);
        try
        {
            var actors = await _movieService.GetActorsByMovieId(movieId);
            if (actors == null || !actors.Any())
            {
                _logger.LogWarning("No se encontraron actores para la película con ID {MovieId}", movieId);
                return NotFound(new ProblemDetails { Status = 404, Title = "Actores no encontrados", Detail = "No se encontraron actores para la película especificada." });
            }

            _logger.LogInformation("Actores obtenidos correctamente para la película con ID {MovieId}", movieId);
            return Ok(actors);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener actores de la película: {Error}", ex.Message);
            return BadRequest(new ProblemDetails { Status = 400, Title = "Error al obtener actores", Detail = ex.Message });
        }
    }

    [HttpPost("{movieId}/actors")]
    public async Task<IActionResult> UpdateMovieActors(int movieId, [FromBody] List<int> actorIds)
    {
        try
        {
            await _movieService.UpdateMovieActorsAsync(movieId, actorIds);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al actualizar actores de la película: {Message}", ex.Message);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
