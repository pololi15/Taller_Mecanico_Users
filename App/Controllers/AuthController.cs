using Microsoft.AspNetCore.Mvc;
using Taller_Mecanico_Users.Domain.Ports;
using Taller_Mecanico_Users.Framework.DTOs.Users;

namespace Taller_Mecanico_Users.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioLoginRepository _loginRepository;

        public AuthController(IUsuarioLoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _loginRepository.GetByEmailAsync(request.Email);
            if (usuario == null)
                return Unauthorized(new { message = "Credenciales inválidas." });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized(new { message = "Credenciales inválidas." });

            return Ok(new { usuario.UsuarioLoginId, usuario.Email, usuario.EsCliente });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
