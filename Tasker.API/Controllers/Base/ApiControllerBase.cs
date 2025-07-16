using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tasker.API.Models.Database;
using Tasker.API.Services;

namespace Tasker.API.Controllers.Base
{
    public partial class ApiControllerBase : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;
        protected readonly IConfiguration _configuration;

        public ApiControllerBase(IMapper mapper, IUserService userService, IConfiguration configuration)
        {
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
        }

        protected TaskerUser GetLoggedInUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return null!;
            return _userService.GetByIdAsync(userId).Result!;
        }
    }
}
