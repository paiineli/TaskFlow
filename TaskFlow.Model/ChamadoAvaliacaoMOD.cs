using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class ChamadoAvaliacaoMOD
    {
        [Display(Name = "Código da Avaliação")]
        public Int32 CdAvaliacao { get; set; }

        [Display(Name = "Código do Chamado")]
        public Int32 CdChamado { get; set; }

        [Display(Name = "Código do Funcionário")]
        public Int32 CdFuncionario { get; set; }

        [Required(ErrorMessage = "A Nota é obrigatória")]
        [Range(1, 5, ErrorMessage = "A Nota deve ser entre 1 e 5")]
        [Display(Name = "Nota")]
        public Int32 NrNota { get; set; }

        [StringLength(1000, ErrorMessage = "O Comentário deve ter no máximo 1000 caracteres")]
        [Display(Name = "Comentário")]
        public string TxComentario { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        // Propriedades de Navegação
        public FuncionarioMOD Funcionario { get; set; }
        public ChamadoMOD Chamado { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Nome do Funcionário")]
        public string NmFuncionario { get; set; }

        [Display(Name = "Nota em Estrelas")]
        public string TxNotaEstrelas { get; set; }
    }
}
