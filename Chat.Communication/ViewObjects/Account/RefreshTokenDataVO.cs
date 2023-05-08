using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.ViewObjects.Account
{
    public class RefreshTokenDataVO
    {
        [Required(ErrorMessage = "Token de acesso não informado")]
        public string AccessToken { get; set; }
        
        [Required(ErrorMessage = "Refresh token não informado")]
        public string RefreshToken { get; set; }
    }
}
