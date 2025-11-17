using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Project.Application.Interfaces.IServices;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public RoleService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            RoleManager<Role> roleManager,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
    }
}
