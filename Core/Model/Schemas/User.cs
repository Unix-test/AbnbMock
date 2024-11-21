using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Core.Model.Schemas;

public class User: IdentityUser<Guid>
{
    public DateTime CreatedDate { get; init; }
}

public class IdentityUserRoles: IdentityUserRole<Guid>;

public class Roles : IdentityRole<Guid>;
  