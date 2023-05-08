using Chat.Communication.ViewObjects.Email;
using Chat.Communication.ViewObjects.User;
using FluentResults;

namespace Chat.Core.ServicesInterface
{
    public interface ISendMailService
    {
        public Task<Result> SendMailForgotPasswordAsync(EmailDataForgotPasswordVO data, UserReturnVO user);
    }
}
