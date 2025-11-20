namespace AuthService.Application.Roles.CreateRole;

public record CreateRoleResponse(
    Guid RoleId,
    string Name
);
