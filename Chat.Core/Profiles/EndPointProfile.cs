using AutoMapper;
using Chat.Communication.ViewObjects.EndPoints;
using Chat.Core.Entities.NoSql;

namespace Chat.Core.Profiles
{
    public class EndPointProfile : Profile
    {
        public EndPointProfile()
        {
            CreateMap<EndPointMap, EndPointSaveVO>()
                .ReverseMap();
        }
    }
}
