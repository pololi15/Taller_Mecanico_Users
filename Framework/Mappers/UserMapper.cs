using Domain.Entities;
using Framework.DTOs.Users;

namespace Framework.Mappers;

/// <summary>
/// Mapper para convertir entre Domain entities y DTOs
/// </summary>
public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User ToDomain(UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Email = dto.Email,
            Name = dto.Name,
            Phone = dto.Phone,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
