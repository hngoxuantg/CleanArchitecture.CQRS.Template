using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Project.Application.Interfaces.IDataSeedingServices;
using Project.Common.Options;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.DataSeedingServices
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AdminAccount _adminAccount;
        public DataSeedingService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<AdminAccount> adminAccount,
            ApplicationDbContext applicationDbContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminAccount = adminAccount.Value;
            _dbContext = applicationDbContext;
        }
        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
            if (!await _dbContext.Roles.AnyAsync(cancellationToken))
            {
                List<Role> roles = new List<Role>
                {
                    new Role {Name = "Admin" },
                    new Role {Name = "User" }
                };
                for (int i = 0; i < roles.Count; i++)
                    await _roleManager.CreateAsync(roles[i]);
            }
            if (!await _dbContext.Users.AnyAsync(cancellationToken))
            {
                Role? role = await _roleManager.FindByNameAsync("Admin");
                User user = new User
                {
                    FullName = _adminAccount.Account.FullName,
                    UserName = _adminAccount.Account.UserName,
                    Email = _adminAccount.Account.Email,
                    PhoneNumber = _adminAccount.Account.PhoneNumber,
                };
                IdentityResult result = await _userManager.CreateAsync(user, _adminAccount.Account.Password);
                if (!result.Succeeded)
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                await _userManager.AddToRoleAsync(user, role.Name);

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, token);
                if (!confirmResult.Succeeded)
                    throw new Exception($"Failed to confirm admin email:" +
                        $" {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
