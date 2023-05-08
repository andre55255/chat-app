using AutoMapper;
using Chat.Communication.ViewObjects.Account;
using Chat.Communication.ViewObjects.User;
using Chat.Core.Entities.NoSql;

namespace Chat.Core.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() : base()
        {
            CreateMap<ApplicationUser, UserReturnVO>();

            CreateMap<ApplicationUser, UserFindVO>();

            CreateMap<UserReturnVO, UserFindVO>();

            CreateMap<UserReturnVO, UserCreateVO>();

            CreateMap<UserReturnVO, UserInfoVO>()
                .ForMember(x => x.Roles, x => x.MapFrom(x => x.Roles!.Select(x => x.Name).ToList()));

            CreateMap<UserCreateVO, ApplicationUser>()
                .ForMember(x => x.PasswordHash, x => x.MapFrom(x => x.Password))
                .ForMember(x => x.Roles, x => x.Ignore());

            CreateMap<UserEditVO, ApplicationUser>()
                .ForMember(x => x.Roles, x => x.Ignore());

            CreateMap<RegisterUserPublicVO, UserCreateVO>()
                .ReverseMap();
        }
    }
}
