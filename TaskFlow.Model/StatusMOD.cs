using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class StatusMOD
    {
        [Display(Name = "Código do Status")]
        public Int32 CdStatus { get; set; }

        [Display(Name = "Descrição do Status")]
        public string TxStatus { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Situação")]
        public string SnAtivo { get; set; }
    }
}