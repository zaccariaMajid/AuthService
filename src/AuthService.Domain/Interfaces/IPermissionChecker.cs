using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces;

public interface IPermissionChecker
{
    Task<bool> UserHasPermissionAsync(Guid userId, string permission);
}