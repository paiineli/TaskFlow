using System.ComponentModel.DataAnnotations;

namespace TaskFlow.UI.Web.Models
{
    public class AvaliacaoViewMOD
    {
        public Int32 CdChamado { get; set; }

        [Required(ErrorMessage = "A nota é obrigatória")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5")]
        [Display(Name = "Nota")]
        public Int32 NrNota { get; set; }

        [Display(Name = "Comentário (opcional)")]
        [StringLength(1000, ErrorMessage = "O comentário deve ter no máximo 1000 caracteres")]
        public string? TxComentario { get; set; }
    }
}
