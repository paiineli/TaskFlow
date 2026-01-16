using System.ComponentModel.DataAnnotations;

namespace TaskFlow.UI.Web.Models
{
    public class UsuarioViewMOD
    {
        [Required(ErrorMessage = "Preencha seu usuário", AllowEmptyStrings = false)]
        [Display(Name = "Usuário")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Preencha sua senha", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string? Password { get; set; }
    }
}