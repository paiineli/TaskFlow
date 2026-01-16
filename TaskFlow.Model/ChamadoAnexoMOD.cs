using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class ChamadoAnexoMOD
    {
        [Display(Name = "Código do Anexo")]
        public Int32 CdAnexo { get; set; }

        [Display(Name = "Código do Chamado")]
        public Int32 CdChamado { get; set; }

        [Display(Name = "Código do Funcionário")]
        public Int32 CdFuncionario { get; set; }

        [Required(ErrorMessage = "O Nome do Arquivo é obrigatório")]
        [StringLength(255, ErrorMessage = "O Nome do Arquivo deve ter no máximo 255 caracteres")]
        [Display(Name = "Nome do Arquivo")]
        public string NmArquivo { get; set; }

        [Required(ErrorMessage = "O Caminho do Arquivo é obrigatório")]
        [StringLength(500, ErrorMessage = "O Caminho deve ter no máximo 500 caracteres")]
        [Display(Name = "Caminho do Arquivo")]
        public string TxCaminhoArquivo { get; set; }

        [Display(Name = "Tamanho em Bytes")]
        public Int64 NrTamanhoBytes { get; set; }

        [StringLength(100, ErrorMessage = "O Tipo MIME deve ter no máximo 100 caracteres")]
        [Display(Name = "Tipo MIME")]
        public string TxTipoMime { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Situação")]
        public string SnAtivo { get; set; }

        // Propriedades de Navegação
        public FuncionarioMOD Funcionario { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Nome do Funcionário")]
        public string NmFuncionario { get; set; }

        [Display(Name = "Tamanho Formatado")]
        public string TxTamanhoFormatado { get; set; }
    }
}
