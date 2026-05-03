using Domain.Entities;
using Data.Repositories;

namespace UseCases.Users;

/// <summary>
/// Caso de uso para obtener todos los usuarios
/// </summary>
public class GetAllUsers
{
    private readonly IUserRepository _userRepository;

    public GetAllUsers(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> ExecuteAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }
}
