using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class ChamadoMOD
    {
        [Display(Name = "Código do Chamado")]
        public Int32 CdChamado { get; set; }

        [Required(ErrorMessage = "O Número do Chamado é obrigatório")]
        [Display(Name = "Número do Chamado")]
        public string NrChamado { get; set; }

        [Required(ErrorMessage = "O Título é obrigatório")]
        [StringLength(200, ErrorMessage = "O Título deve ter no máximo 200 caracteres")]
        [Display(Name = "Título")]
        public string TxTitulo { get; set; }

        [Required(ErrorMessage = "A Descrição é obrigatória")]
        [Display(Name = "Descrição")]
        public string TxDescricao { get; set; }

        [Required(ErrorMessage = "A Categoria é obrigatória")]
        [Display(Name = "Código da Categoria")]
        public Int32 CdCategoria { get; set; }

        [Display(Name = "Código do Status")]
        public Int32 CdStatus { get; set; }

        [Display(Name = "Código da Prioridade")]
        public Int32 CdPrioridade { get; set; }

        [Display(Name = "Código do Solicitante")]
        public Int32 CdSolicitante { get; set; }

        [Display(Name = "Código do Responsável")]
        public Int32? CdResponsavel { get; set; }

        [Display(Name = "Código da Empresa")]
        public Int32 CdEmpresa { get; set; }

        [Display(Name = "Data de Abertura")]
        public DateTime DtAbertura { get; set; }

        [Display(Name = "Data da Última Atualização")]
        public DateTime DtUltimaAtualizacao { get; set; }

        [Display(Name = "Data do Prazo")]
        public DateTime? DtPrazo { get; set; }

        [Display(Name = "Data de Resolução")]
        public DateTime? DtResolucao { get; set; }

        [Display(Name = "Data de Fechamento")]
        public DateTime? DtFechamento { get; set; }

        [Display(Name = "Código da Justificativa")]
        public Int32? CdJustificativa { get; set; }

        [Display(Name = "Complemento da Justificativa")]
        [StringLength(1000, ErrorMessage = "O complemento deve ter no máximo 1000 caracteres")]
        public string TxJustificativaComplemento { get; set; }

        [Display(Name = "Situação")]
        public string SnAtivo { get; set; }

        // Propriedades de Navegação/Relacionamento
        public CategoriaMOD Categoria { get; set; }
        public StatusMOD Status { get; set; }
        public FuncionarioMOD Solicitante { get; set; }
        public FuncionarioMOD Responsavel { get; set; }
        public JustificativaMOD Justificativa { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Nome da Categoria")]
        public string NmCategoria { get; set; }

        [Display(Name = "Nome do Status")]
        public string TxStatus { get; set; }

        [Display(Name = "Nome do Solicitante")]
        public string NmSolicitante { get; set; }

        [Display(Name = "Nome do Responsável")]
        public string NmResponsavel { get; set; }

        [Display(Name = "Prioridade")]
        public string TxPrioridade { get; set; }

        [Display(Name = "Tempo em Aberto (horas)")]
        public Int32 NrHorasAberto { get; set; }

        [Display(Name = "Quantidade de Comentários")]
        public Int32 QtdComentarios { get; set; }

        [Display(Name = "Quantidade de Anexos")]
        public Int32 QtdAnexos { get; set; }

        [Display(Name = "Possui Avaliação")]
        public bool SnPossuiAvaliacao { get; set; }
    }
}
