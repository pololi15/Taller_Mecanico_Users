using Domain.Entities;
using Data.Repositories;

namespace UseCases.Users;

/// <summary>
/// Caso de uso para eliminar un usuario
/// </summary>
public class DeleteUser
{
    private readonly IUserRepository _userRepository;

    public DeleteUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ExecuteAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("El ID del usuario debe ser mayor a 0");

        var exists = await _userRepository.UserExistsAsync(userId);
        if (!exists)
            throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");

        return await _userRepository.DeleteUserAsync(userId);
    }
}
