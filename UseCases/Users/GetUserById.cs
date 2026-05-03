using Domain.Entities;
using Data.Repositories;

namespace UseCases.Users;

/// <summary>
/// Caso de uso para obtener un usuario por ID
/// Representa una acción que el usuario puede realizar
/// </summary>
public class GetUserById
{
    private readonly IUserRepository _userRepository;

    public GetUserById(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> ExecuteAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("El ID del usuario debe ser mayor a 0");

        return await _userRepository.GetUserByIdAsync(userId);
    }
}
