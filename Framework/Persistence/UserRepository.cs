using Domain.Entities;
using Data.Repositories;

namespace Framework.Persistence;

/// <summary>
/// Implementación del repositorio de usuarios
/// NOTA: Esta es una implementación en memoria. En producción, usar Entity Framework Core, Dapper, etc.
/// </summary>
public class UserRepository : IUserRepository
{
    // Simulación de base de datos en memoria
    private static readonly List<User> _users = new();
    private static int _nextId = 1;

    public Task<User?> GetUserByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return Task.FromResult(_users.ToList());
    }

    public Task<User> CreateUserAsync(User user)
    {
        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User> UpdateUserAsync(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
            throw new KeyNotFoundException($"Usuario con ID {user.Id} no encontrado");

        existingUser.Email = user.Email;
        existingUser.Name = user.Name;
        existingUser.Phone = user.Phone;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(existingUser);
    }

    public Task<bool> DeleteUserAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return Task.FromResult(false);

        _users.Remove(user);
        return Task.FromResult(true);
    }

    public Task<bool> UserExistsAsync(int id)
    {
        return Task.FromResult(_users.Any(u => u.Id == id));
    }
}
