using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Taller_Mecanico_Users.UseCases.Users;

namespace Taller_Mecanico_Users.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly UpdateUserUseCase _updateUserUseCase;

        public UsersController(CreateUserUseCase createUserUseCase, UpdateUserUseCase updateUserUseCase)
        {
            _createUserUseCase = createUserUseCase;
            _updateUserUseCase = updateUserUseCase;
        }

        [HttpPost]
        [Authorize(Roles = "Empleado")] // Solo empleados pueden crear
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var usuario = await _createUserUseCase.ExecuteAsync(request.EmpleadoId, request.Email);
                return CreatedAtAction(nameof(GetUserById), new { id = usuario.UsuarioLoginId }, new { usuario.UsuarioLoginId, usuario.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            return Ok(new { message = $"Simulación: Datos del usuario con ID {id}" });
        }

        // ==========================================
        // AQUÍ ESTÁ EL MÉTODO QUE ACABAMOS DE AGREGAR
        // ==========================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Empleado")] // Solo empleados pueden actualizar
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var result = await _updateUserUseCase.ExecuteAsync(id, request.Email, request.Activo);

            if (result.IsFailure)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return NoContent(); 
        }
    }

    // ==========================================
    // AQUÍ ESTÁN LAS CLASES DE REQUEST (AL FINAL)
    // ==========================================
    public class CreateUserRequest
    {
        public int EmpleadoId { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    // NUEVA CLASE AGREGADA
    public class UpdateUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}