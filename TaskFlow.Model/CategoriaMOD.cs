using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class CategoriaMOD
    {
        [Display(Name = "Código da Categoria")]
        public Int32 CdCategoria { get; set; }

        [Required(ErrorMessage = "O Nome da Categoria é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome da Categoria")]
        public string NmCategoria { get; set; }

        [StringLength(500, ErrorMessage = "A Descrição deve ter no máximo 500 caracteres")]
        [Display(Name = "Descrição da Categoria")]
        public string DsCategoria { get; set; }

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
    }
}
