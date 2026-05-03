using Domain.Entities;
using Data.Repositories;

namespace UseCases.Users;

/// <summary>
/// Caso de uso para crear un nuevo usuario
/// </summary>
public class CreateUser
{
    private readonly IUserRepository _userRepository;

    public CreateUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> ExecuteAsync(string email, string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es requerido");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es requerido");

        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
            throw new InvalidOperationException($"Ya existe un usuario con el email {email}");

        var user = new User
        {
            Email = email,
            Name = name,
            Phone = phone,
            CreatedAt = DateTime.UtcNow
        };

        return await _userRepository.CreateUserAsync(user);
    }
}
