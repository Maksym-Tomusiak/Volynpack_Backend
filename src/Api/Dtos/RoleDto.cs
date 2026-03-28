using Domain.Roles;

namespace Api.Dtos;

public record RoleDto(Guid Id, string Name)
{
    public static RoleDto FromDomainModel(Role role) => new(role.Id, role.Name);
}