using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;

namespace Chat.Core.ServicesInterface
{
    public interface IUserService
    {
        public Task<UserReturnVO> CreateAsync(UserCreateVO model);
        public Task<UserReturnVO> EditAsync(string userId, UserEditVO model);
        public Task<UserReturnVO> DeleteAsync(string userId);
        public Task<UserReturnVO> GetByIdAsync(string userId);
        public Task<ListAllEntityVO<UserReturnVO>> GetAllAsync(FilterUserVO filter);
    }
}
