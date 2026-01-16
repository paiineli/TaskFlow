using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Model
{
    public sealed class FuncionarioMOD
    {
        [Display(Name = "Código do Funcionário")]
        public Int32 CdFuncionario { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres")]
        [Display(Name = "Número de CPF")]
        public string NrCpf { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O Nome deve ter no máximo 200 caracteres")]
        [Display(Name = "Nome do Funcionário")]
        public string NmFuncionario { get; set; }

        [Display(Name = "Código da Empresa")]
        public Int32 CdEmpresa { get; set; }

        [Required(ErrorMessage = "O Login é obrigatório")]
        [StringLength(50, ErrorMessage = "O Login deve ter no máximo 50 caracteres")]
        [Display(Name = "Login de Acesso")]
        public string TxLogin { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória")]
        [StringLength(255, ErrorMessage = "A Senha deve ter no máximo 255 caracteres")]
        [Display(Name = "Senha de Acesso")]
        public string TxSenha { get; set; }

        [StringLength(200, ErrorMessage = "O E-mail deve ter no máximo 200 caracteres")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string TxEmail { get; set; }

        [Required(ErrorMessage = "O Tipo de Usuário é obrigatório")]
        [Display(Name = "Tipo de Usuário")]
        public string SnTipoUsuario { get; set; } // A=Admin, U=User/Atendente, C=Cliente

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

        // Propriedades de Navegação
        public EmpresaMOD Empresa { get; set; }

        // Propriedades Calculadas
        [Display(Name = "Nome da Empresa")]
        public string NmEmpresa { get; set; }

        [Display(Name = "Descrição do Tipo de Usuário")]
        public string TxTipoUsuario
        {
            get
            {
                return SnTipoUsuario switch
                {
                    "A" => "Administrador",
                    "U" => "Atendente",
                    "C" => "Cliente",
                    _ => "Indefinido"
                };
            }
        }
    }
}
