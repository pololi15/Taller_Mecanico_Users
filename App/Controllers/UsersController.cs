using Framework.DTOs.Users;
using Framework.Mappers;
using Microsoft.AspNetCore.Mvc;
using UseCases.Users;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly GetUserById _getUserById;
    private readonly CreateUser _createUser;
    private readonly GetAllUsers _getAllUsers;
    private readonly DeleteUser _deleteUser;

    public UsersController(GetUserById getUserById, CreateUser createUser, GetAllUsers getAllUsers, DeleteUser deleteUser)
    {
        _getUserById = getUserById;
        _createUser = createUser;
        _getAllUsers = getAllUsers;
        _deleteUser = deleteUser;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _getUserById.ExecuteAsync(id);
            if (user == null)
                return NotFound();

            return Ok(UserMapper.ToDto(user));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _getAllUsers.ExecuteAsync();
        return Ok(users.Select(UserMapper.ToDto).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _createUser.ExecuteAsync(request.Email, request.Name, request.Phone);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, UserMapper.ToDto(user));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            await _deleteUser.ExecuteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
