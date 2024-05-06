using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<bool> RegisterUserAsync(string username, string email, string password)
    {
        // Primero verifica si el usuario o email ya existe
        if (await _userRepository.GetUserByUsernameAsync(username) != null ||
            await _userRepository.GetUserByEmailAsync(email) != null)
        {
            return false;  // El usuario ya existe
        }

        // Crea el usuario y guarda el hash de la contraseña
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var prueba = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        var user = new User { Username = username, Email = email, PasswordHash = hashedPassword };
        return await _userRepository.CreateUserAsync(user);
    }

    public async Task<(bool Success, string Token)> AuthenticateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(username);

       if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            // Genera el token si las credenciales son correctas
            var token = GenerateJwtToken(user);
            return (true, token);
        }
        return (false, null);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // Asegúrate de que la clave sea la misma que la configurada en el middleware
        var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:SecretKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.UserId.ToString())
            }),
            // Asegúrate de que el issuer y audience sean correctos y coincidan con los definidos en el middleware
            Issuer = _configuration["JwtConfig:Issuer"],
            Audience = _configuration["JwtConfig:Audience"],
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
