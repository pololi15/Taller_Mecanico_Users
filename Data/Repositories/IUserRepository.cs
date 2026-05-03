using Domain.Entities;

namespace Data.Repositories;

/// <summary>
/// Contrato para el repositorio de Usuarios
/// Define las operaciones que se pueden hacer con usuarios sin saber cómo se implementan
/// </summary>
public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UserExistsAsync(int id);
}
