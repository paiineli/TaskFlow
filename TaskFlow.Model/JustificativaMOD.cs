using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class JustificativaMOD
    {
        [Display(Name = "Código da Justificativa")]
        public Int32 CdJustificativa { get; set; }

        [Required(ErrorMessage = "O Texto da Justificativa é obrigatório")]
        [StringLength(500, ErrorMessage = "O Texto deve ter no máximo 500 caracteres")]
        [Display(Name = "Texto da Justificativa")]
        public string TxJustificativa { get; set; }

        [Display(Name = "Tipo de Justificativa")]
        public string SnTipoJustificativa { get; set; } // F=Fechamento, C=Cancelamento

        [Display(Name = "Código do Usuário que Cadastrou")]
        public Int32 CdUsuarioCadastro { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Código do Usuário que Alterou")]
        public Int32? CdUsuarioAlteracao { get; set; }

        [Display(Name = "Data de Alteração")]
        public DateTime? DtAlteracao { get; set; }

        [Display(Name = "Situação")]
        public string SnAtivo { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Descrição do Tipo")]
        public string TxTipoJustificativa
        {
            get
            {
                return SnTipoJustificativa switch
                {
                    "F" => "Fechamento",
                    "C" => "Cancelamento",
                    _ => "Indefinido"
                };
            }
        }
    }
}
