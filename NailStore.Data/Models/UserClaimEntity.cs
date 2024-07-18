using Microsoft.AspNetCore.Identity;

namespace NailStore.Data.Models;

public class UserClaimEntity : IdentityUserClaim<int>
{
    public Guid UserId { get; set; }
}
