using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.ViewObjects.Role
{
    public class RoleSaveVO
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Nome não informado")]
        [StringLength(50, ErrorMessage = "Nome do perfil deve ter no máximo 50 caracteres", MinimumLength = 1)]
        public string? Name { get; set; }
    }

    public class RoleReturnVO
    {
        public string? Id { get; set; } = null;
        public string? Name { get; set; } = null;
        public string? NormalizedName { get; set; } = null;
    }
}
