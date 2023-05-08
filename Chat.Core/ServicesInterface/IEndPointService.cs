using Chat.Communication.ViewObjects.EndPoints;
using Chat.Communication.ViewObjects.Utils;

namespace Chat.Core.ServicesInterface
{
    public interface IEndPointService
    {
        public Task<EndPointSaveVO> CreateAsync(EndPointSaveVO model);
        public Task<EndPointSaveVO> EditAsync(EndPointSaveVO model);
        public Task<EndPointSaveVO> DeleteAsync(string id);
        public Task<EndPointSaveVO> GetByIdAsync(string id);
        public Task<ListAllEntityVO<EndPointSaveVO>> GetAllAsync(FilterEndpointVO filter);
    }
}
