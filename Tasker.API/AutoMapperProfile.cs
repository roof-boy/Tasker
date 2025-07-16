using AutoMapper;
using Tasker.API.Models.Database;
using Tasker.API.Models.Transfer.Team;
using Tasker.API.Models.Transfer.User;
namespace Tasker.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // USER
            CreateMap<TaskerUser, LoginResponseModel>();
            CreateMap<TaskerUser, UserInfoModel>();
            CreateMap<UserRegisterRequest, TaskerUser>();

            // TEAM
            CreateMap<Team, TeamInfoModel>();
        }
    }
}
