using Microsoft.AspNetCore.Identity;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Identity_Auth
{
    public class Role : IdentityRole<int>, IAggregateRoot
    {
    }
}
