using Chat.Communication.ViewObjects.Role;
using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;
using FluentResults;

namespace Chat.Core.RepositoriesInterface.NoSql
{
    public interface IApplicationUserRepository
    {
        public Task<UserReturnVO> CreateAsync(UserCreateVO applicationUser, List<RoleReturnVO> roles);
        public Task<UserReturnVO> EditAsync(string userId, UserEditVO applicationUser, List<RoleReturnVO> roles);
        public Task<UserReturnVO> DeleteAsync(string userId);
        public Task<UserReturnVO> GetByIdAsync(string userId);
        public Task<UserReturnVO> GetByUsernameAsync(string username);
        public Task<UserReturnVO> GetByEmailAsync(string email);
        public Task<ListAllEntityVO<UserReturnVO>> GetAllAsync(FilterUserVO filter);
        public Task<Result> SetRefreshTokenAsync(string userId, string refreshToken);
        public Task<Result> SetTokenPassAsync(string userId, string tokenPass);
        public Task<Result> SetBlockUnblockUserAsync(string userId, bool isUnblock, DateTime? lockouDateEnd = null);
        public Task<Result> SetAttemptLoginAsync(string userId, bool isIncrement = false);
        public Task<Result> SetNewPasswordAsync(string userId, string newPassword);
    }
}
