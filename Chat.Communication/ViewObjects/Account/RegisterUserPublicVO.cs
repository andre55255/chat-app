using Chat.Communication.CustomAnotations;
using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.ViewObjects.Account
{
    public class RegisterUserPublicVO
    {
        [Required(ErrorMessage = "Primeiro nome não informado")]
        [StringLength(256, ErrorMessage = "Primeiro nome deve ter no máximo 256 caracteres", MinimumLength = 1)]
        public string? FirstName { get; set; } = null;

        [Required(ErrorMessage = "Sobrenome não informado")]
        [StringLength(256, ErrorMessage = "Sobrenome deve ter no máximo 256 caracteres", MinimumLength = 1)]
        public string? LastName { get; set; } = null;

        [Required(ErrorMessage = "Email não informado")]
        [StringLength(256, ErrorMessage = "Email deve ter no máximo 256 caracteres", MinimumLength = 1)]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; } = null;

        [Required(ErrorMessage = "Nome de usuário não informado")]
        [StringLength(128, ErrorMessage = "Nome de usuário deve ter no máximo 128 caracteres", MinimumLength = 1)]
        public string? Username { get; set; } = null;

        [Required(ErrorMessage = "Senha não informada")]
        [StringLength(64, ErrorMessage = "Senha deve ter no máximo 64 caracteres", MinimumLength = 6)]
        [IsValidPassword(ErrorMessage = "Senha deve conter pelo menos: 1 dígito, 1 caractere minúsculo, 1 caractere maiúsculo, 1 caractere especial")]
        public string? Password { get; set; } = null;
    }
}
