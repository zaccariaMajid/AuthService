using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;

namespace AuthService.Domain.Events;

public record RoleCreated(Guid RoleId, string RoleName) : DomainEvent;
