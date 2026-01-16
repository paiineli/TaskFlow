using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class ChamadoHistoricoMOD
    {
        [Display(Name = "Código do Histórico")]
        public Int32 CdHistorico { get; set; }

        [Display(Name = "Código do Chamado")]
        public Int32 CdChamado { get; set; }

        [Display(Name = "Código do Funcionário")]
        public Int32 CdFuncionario { get; set; }

        [Required(ErrorMessage = "A Ação é obrigatória")]
        [StringLength(100, ErrorMessage = "A Ação deve ter no máximo 100 caracteres")]
        [Display(Name = "Ação")]
        public string TxAcao { get; set; }

        [StringLength(500, ErrorMessage = "O Valor Anterior deve ter no máximo 500 caracteres")]
        [Display(Name = "Valor Anterior")]
        public string TxValorAnterior { get; set; }

        [StringLength(500, ErrorMessage = "O Valor Novo deve ter no máximo 500 caracteres")]
        [Display(Name = "Valor Novo")]
        public string TxValorNovo { get; set; }

        [StringLength(1000, ErrorMessage = "A Observação deve ter no máximo 1000 caracteres")]
        [Display(Name = "Observação")]
        public string TxObservacao { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        // Propriedades de Navegação
        public FuncionarioMOD Funcionario { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Nome do Funcionário")]
        public string NmFuncionario { get; set; }

        [Display(Name = "Descrição da Ação")]
        public string TxAcaoDescricao { get; set; }
    }
}
