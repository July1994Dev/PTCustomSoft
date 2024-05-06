using Microsoft.AspNetCore.Mvc;
using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistration)
    {
        if (string.IsNullOrEmpty(userRegistration.Username) || string.IsNullOrEmpty(userRegistration.Email) || string.IsNullOrEmpty(userRegistration.Password))
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Campos requeridos",
                Detail = "Usuario, correo y contraseña son requeridos."
            });
        }

        var result = await _userService.RegisterUserAsync(userRegistration.Username, userRegistration.Email, userRegistration.Password);
        if (!result)
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Error de Registro",
                Detail = "Ya existe un registro con los mismos datos de usuario."
            });
        }

        return Ok("Usuario registrado correctamente.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
    {
        if (string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrEmpty(userLogin.Password))
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Campos requeridos",
                Detail = "Usuario y contraseña son requeridos."
            });
        }

        var (Success, Token) = await _userService.AuthenticateUserAsync(userLogin.Username, userLogin.Password);
        if (!Success)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = 401,
                Title = "Autenticación Fallida",
                Detail = "Datos invalidos."
            });
        }

        return Ok(new { Token = Token });
    }
}
