using AutoMapper;
using Chat.Communication.ViewObjects.Role;
using Chat.Core.Entities.NoSql;

namespace Chat.Core.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile() : base()
        {
            CreateMap<Role, RoleSaveVO>();

            CreateMap<RoleSaveVO, Role>()
                .ForMember(x => x.NormalizedName, x => x.MapFrom(x => x.Name.ToUpper()));

            CreateMap<Role, RoleReturnVO>()
                .ReverseMap();
        }
    }
}
