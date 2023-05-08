using Chat.Communication.ViewObjects.User;
using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.ViewObjects.Account
{
    public class LoginDataVO
    {
        [Required(ErrorMessage = "Nome de uusário não informado")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Senha não informada")]
        public string Password { get; set; }
    }

    public class LoginResponseVO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpirationAt { get; set; }
        public UserFindVO User { get; set; }
    }
}
