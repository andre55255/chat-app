using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.ViewObjects.EndPoints
{
    public class EndPointSaveVO
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Rota não informada")]
        public string Route { get; set; }

        [Required(ErrorMessage = "Verbo não informado")]
        public string Verb { get; set; }

        [Required(ErrorMessage = "Perfis com acesso não informados")]
        public List<string> Roles { get; set; }
    }
}
