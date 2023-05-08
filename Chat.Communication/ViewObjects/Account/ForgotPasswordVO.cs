using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.ViewObjects.Account
{
    public class ForgotPasswordVO
    {
        [Required(ErrorMessage = "Nome de usuário não informado")]
        public string UserName { get; set; }
    }
}
