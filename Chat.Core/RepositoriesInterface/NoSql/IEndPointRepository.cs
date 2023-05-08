using Chat.Communication.ViewObjects.EndPoints;
using Chat.Communication.ViewObjects.Utils;

namespace Chat.Core.RepositoriesInterface.NoSql
{
    public interface IEndPointRepository
    {
        public Task<EndPointSaveVO> CreateAsync(EndPointSaveVO model);
        public Task<EndPointSaveVO> EditAsync(EndPointSaveVO model);
        public Task<EndPointSaveVO> DeleteAsync(string id);
        public Task<EndPointSaveVO> GetByIdAsync(string id);
        public Task<EndPointSaveVO> GetByRouteByVerbAsync(string route, string verb);
        public EndPointSaveVO GetByRouteByVerbSync(string route, string verb);
        public Task<ListAllEntityVO<EndPointSaveVO>> GetAllAsync(FilterEndpointVO filter);
    }
}
