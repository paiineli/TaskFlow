using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class FuncionarioREP
    {
        private readonly AcessaDados _acessaDados;

        public FuncionarioREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Realiza a autenticação do funcionário
        /// </summary>
        public FuncionarioMOD BuscarLogin(String txLogin, String txSenha)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT F.CdFuncionario,
                                         F.NmFuncionario,
                                         F.NrCpf,
                                         F.CdEmpresa,
                                         F.TxLogin,
                                         F.TxEmail,
                                         F.SnTipoUsuario
                                    FROM TB_FUNCIONARIO F
                                   WHERE UPPER(F.TxLogin) = UPPER(@TxLogin)
                                     AND F.TxSenha = @TxSenha
                                     AND F.SnAtivo = 'S'";

                    return con.QueryFirstOrDefault<FuncionarioMOD>(query, new { TxLogin = txLogin, TxSenha = txSenha });
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar login do funcionário", ex);
                }
            }
        }

        /// <summary>
        /// Busca um funcionário específico pelo código
        /// </summary>
        public FuncionarioMOD BuscarFuncionarioPorCodigo(Int32 cdFuncionario)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT F.CdFuncionario,
                                         F.NrCpf,
                                         F.NmFuncionario,
                                         F.CdEmpresa,
                                         F.TxLogin,
                                         F.TxEmail,
                                         F.SnTipoUsuario,
                                         F.CdUsuarioCadastro,
                                         F.DtCadastro,
                                         F.CdUsuarioAlteracao,
                                         F.DtAlteracao,
                                         F.SnAtivo
                                    FROM TB_FUNCIONARIO F
                                   WHERE F.CdFuncionario = @CdFuncionario";

                    return con.QueryFirstOrDefault<FuncionarioMOD>(query, new { CdFuncionario = cdFuncionario });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar funcionário com código {cdFuncionario}", ex);
                }
            }
        }

        /// <summary>
        /// Busca todos os funcionários de uma empresa
        /// </summary>
        public List<FuncionarioMOD> BuscarFuncionariosPorEmpresa(Int32 cdEmpresa)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT F.CdFuncionario,
                                         F.NmFuncionario,
                                         F.NrCpf,
                                         F.CdEmpresa,
                                         F.TxLogin,
                                         F.TxEmail,
                                         F.SnTipoUsuario,
                                         F.SnAtivo
                                    FROM TB_FUNCIONARIO F
                                   WHERE F.CdEmpresa = @CdEmpresa
                                     AND F.SnAtivo = 'S'
                                   ORDER BY F.NmFuncionario";

                    return con.Query<FuncionarioMOD>(query, new { CdEmpresa = cdEmpresa }).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar funcionários da empresa {cdEmpresa}", ex);
                }
            }
        }

        /// <summary>
        /// Busca funcionários atendentes (para atribuição de chamados)
        /// </summary>
        public async Task<List<FuncionarioMOD>> BuscarAtendentes(Int32? cdEmpresa = null)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT F.CdFuncionario,
                                         F.NmFuncionario,
                                         F.NrCpf,
                                         F.CdEmpresa,
                                         F.TxLogin,
                                         F.TxEmail,
                                         F.SnTipoUsuario,
                                         F.SnAtivo,
                                         E.NmEmpresa
                                    FROM TB_FUNCIONARIO F
                                    INNER JOIN TB_EMPRESA E ON F.CdEmpresa = E.CdEmpresa
                                   WHERE F.SnTipoUsuario IN ('A', 'U')
                                     AND F.SnAtivo = 'S'";

                    if (cdEmpresa.HasValue)
                        query += " AND F.CdEmpresa = @CdEmpresa";

                    query += " ORDER BY F.NmFuncionario";

                    var atendentes = await con.QueryAsync<FuncionarioMOD>(query, new { CdEmpresa = cdEmpresa });
                    return atendentes.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar atendentes", ex);
                }
            }
        }

        /// <summary>
        /// Busca funcionários por tipo
        /// </summary>
        public async Task<List<FuncionarioMOD>> BuscarPorTipo(string snTipoUsuario, Int32? cdEmpresa = null)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT F.CdFuncionario,
                                         F.NmFuncionario,
                                         F.NrCpf,
                                         F.CdEmpresa,
                                         F.TxLogin,
                                         F.TxEmail,
                                         F.SnTipoUsuario,
                                         F.SnAtivo
                                    FROM TB_FUNCIONARIO F
                                   WHERE F.SnTipoUsuario = @SnTipoUsuario
                                     AND F.SnAtivo = 'S'";

                    if (cdEmpresa.HasValue)
                        query += " AND F.CdEmpresa = @CdEmpresa";

                    query += " ORDER BY F.NmFuncionario";

                    var funcionarios = await con.QueryAsync<FuncionarioMOD>(query, new { SnTipoUsuario = snTipoUsuario, CdEmpresa = cdEmpresa });
                    return funcionarios.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar funcionários do tipo {snTipoUsuario}", ex);
                }
            }
        }

        #endregion
    }
}
