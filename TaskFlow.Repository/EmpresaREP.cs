using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class EmpresaREP
    {
        private readonly AcessaDados _acessaDados;

        public EmpresaREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todas as Empresas cadastradas
        /// </summary>
        /// <returns>Lista de empresas</returns>
        public List<EmpresaMOD> BuscarEmpresas()
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdEmpresa,
                                         NmEmpresa,
                                         CdUsuarioCadastro,
                                         DtCadastro,
                                         CdUsuarioAlteracao,
                                         DtAlteracao,
                                         SnAtivo
                                    FROM TB_EMPRESA
                                   WHERE SnAtivo = 'S'
                                   ORDER BY NmEmpresa";

                    return con.Query<EmpresaMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar empresas", ex);
                }
            }
        }

        /// <summary>
        /// Busca uma Empresa específica pelo código
        /// </summary>
        /// <param name="cdEmpresa">Código da empresa</param>
        /// <returns>Empresa encontrada</returns>
        public EmpresaMOD BuscarEmpresaPorCodigo(Int32 cdEmpresa)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdEmpresa,
                                         NmEmpresa,
                                         CdUsuarioCadastro,
                                         DtCadastro,
                                         CdUsuarioAlteracao,
                                         DtAlteracao,
                                         SnAtivo
                                    FROM TB_EMPRESA
                                   WHERE CdEmpresa = @CdEmpresa";

                    return con.QueryFirstOrDefault<EmpresaMOD>(query, new { CdEmpresa = cdEmpresa });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar empresa com código {cdEmpresa}", ex);
                }
            }
        }

        #endregion
    }
}