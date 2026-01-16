using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class ChamadoComentarioMOD
    {
        [Display(Name = "Código do Comentário")]
        public Int32 CdComentario { get; set; }

        [Display(Name = "Código do Chamado")]
        public Int32 CdChamado { get; set; }

        [Display(Name = "Código do Funcionário")]
        public Int32 CdFuncionario { get; set; }

        [Required(ErrorMessage = "O Comentário é obrigatório")]
        [Display(Name = "Comentário")]
        public string TxComentario { get; set; }

        [Display(Name = "Visibilidade Interna")]
        public string SnVisibilidadeInterna { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Situação")]
        public string SnAtivo { get; set; }

        // Propriedades de Navegação
        public FuncionarioMOD Funcionario { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Nome do Funcionário")]
        public string NmFuncionario { get; set; }

        [Display(Name = "Tipo de Usuário")]
        public string SnTipoUsuario { get; set; }
    }
}
