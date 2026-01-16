using System.ComponentModel.DataAnnotations;

namespace TaskFlow.UI.Web.Models
{
    public class NovoComentarioViewMOD
    {
        public Int32 CdChamado { get; set; }

        [Required(ErrorMessage = "O comentário é obrigatório")]
        [Display(Name = "Comentário")]
        public string TxComentario { get; set; }

        [Display(Name = "Comentário Interno (visível apenas para atendentes)")]
        public bool SnVisibilidadeInterna { get; set; }
    }
}
