namespace AuthService.Application.Permissions.CreatePermission;

public record CreatePermissionResponse(
    Guid PermissionId, 
    string Name
);
