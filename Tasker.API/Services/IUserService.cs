using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Tasker.API.Models.Database;
using Tasker.API.Models.Transfer.User;

namespace Tasker.API.Services
{
    public interface IUserService
    {
        IQueryable<TaskerUser> GetAllUsers();
        Task<TaskerUser?> GetByIdAsync(string id);
        Task<TaskerUser?> GetByEmailAsync(string email);
        Task<List<string>?> GetRolesForUserAsync(TaskerUser user);
        Task<TaskerUser?> AuthenticateAsync(LoginRequestModel model);
        Task<IdentityResult> RegisterAsync(UserRegisterRequest model);
    }

    public class UserService : IUserService
    {
        private readonly SignInManager<TaskerUser> _signInManager;
        private readonly UserManager<TaskerUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(SignInManager<TaskerUser> signInManager, UserManager<TaskerUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<TaskerUser?> AuthenticateAsync(LoginRequestModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return null;

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (signInResult.Succeeded)
                return user;

            return null;
        }

        public IQueryable<TaskerUser> GetAllUsers()
        {
            var userList = _userManager.Users;
            return userList;
        }

        public async Task<TaskerUser?> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<TaskerUser?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<List<string>?> GetRolesForUserAsync(TaskerUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new List<string>(roles) ?? null;
        }

        public async Task<IdentityResult> RegisterAsync(UserRegisterRequest model)
        {
            TaskerUser userModel = _mapper.Map<TaskerUser>(model);

            // Confirm with email later on
            userModel.EmailConfirmed = true;

            if (_userManager.Users.Any(u => u.UserName == model.UserName))
                throw new Exception($"User with Username: {model.UserName} already exists!");

            if (_userManager.Users.Any(u => u.Email == model.Email))
                throw new Exception($"User with Email: {model.Email} already exists!");

            IdentityResult result = await _userManager.CreateAsync(userModel, model.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(userModel, "Administrator");

            return result;
        }
    }
}
