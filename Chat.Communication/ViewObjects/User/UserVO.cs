using Chat.Communication.CustomAnotations;
using Chat.Communication.ViewObjects.Role;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Chat.Communication.ViewObjects.User
{
    public class UserCreateVO
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

        [Required(ErrorMessage = "Perfis não informados")]
        [ArrayMustHaveOneElement(ErrorMessage = "Deve ser informado ao menos 1 perfil")]
        [IsObjectIdList(ErrorMessage = "Códigos de perfis informados inválidos")]
        public List<string>? Roles { get; set; } = null;
    }

    public class UserEditVO
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

        [Required(ErrorMessage = "Perfis não informados")]
        [ArrayMustHaveOneElement(ErrorMessage = "Deve ser informado ao menos 1 perfil")]
        [IsObjectIdList(ErrorMessage = "Códigos de perfis informados inválidos")]
        public List<string>? Roles { get; set; } = null;
    }

    public class UserReturnVO
    {
        public string Id { get; set; }
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Username { get; set; } = null;

        [JsonIgnore]
        public string? PasswordHash { get; set; } = null;
        public string? RefreshToken { get; set; } = null;
        public string? TokenPass { get; set; } = null;
        public int AttemptAccessLogin { get; set; } = 0;
        public DateTime? LockoutDate { get; set; } = null;
        public DateTime? LockoutDateEnd { get; set; } = null;
        public List<RoleReturnVO>? Roles { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DisabledAt { get; set; } = null;
    }

    public class UserFindVO
    {
        public string Id { get; set; }
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Username { get; set; } = null;
        public int AttemptAccessLogin { get; set; } = 0;
        public DateTime? LockoutDate { get; set; } = null;
        public DateTime? LockoutDateEnd { get; set; } = null;
        public List<RoleReturnVO>? Roles { get; set; } = null;
    }
}
