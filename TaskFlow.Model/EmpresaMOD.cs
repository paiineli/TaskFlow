using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class EmpresaMOD
    {
        [Display(Name = "Código da Empresa")]
        public Int32 CdEmpresa { get; set; }

        [Display(Name = "Nome da Empresa")]
        public string NmEmpresa { get; set; }

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