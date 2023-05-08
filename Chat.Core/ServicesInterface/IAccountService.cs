using Chat.Communication.ViewObjects.Account;
using FluentResults;

namespace Chat.Core.ServicesInterface
{
    public interface IAccountService
    {
        public Task<LoginResponseVO> LoginAsync(LoginDataVO model);
        public Task<RefreshTokenDataVO> RefreshTokenAsync(RefreshTokenDataVO model);
        public Task<UserInfoVO> GetUserInfoAsync();
        public Task<Result> ResetPasswordSignInUserAsync(ResetPasswordSignInVO model);
        public Task<Result> ForgotPasswordSendMailAsync(ForgotPasswordVO model);
        public Task<Result> RegisterUserPublicAsync(RegisterUserPublicVO model);
    }
}
