using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class StatusREP
    {
        private readonly AcessaDados _acessaDados;

        public StatusREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todos os status cadastrados
        /// </summary>
        /// <returns>Lista de status</returns>
        public List<StatusMOD> BuscarStatus()
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdStatus,
                                         TxStatus,
                                         DtCadastro,
                                         SnAtivo
                                    FROM TB_STATUS
                                   WHERE SnAtivo = 'S'
                                   ORDER BY TxStatus";

                    return con.Query<StatusMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar status", ex);
                }
            }
        }

        /// <summary>
        /// Busca um status específico pelo código
        /// </summary>
        /// <param name="cdStatus">Código do status</param>
        /// <returns>Status encontrado</returns>
        public StatusMOD BuscarStatusPorCodigo(Int32 cdStatus)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdStatus,
                                         TxStatus,
                                         DtCadastro,
                                         SnAtivo
                                    FROM TB_STATUS
                                   WHERE CdStatus = @CdStatus";

                    return con.QueryFirstOrDefault<StatusMOD>(query, new { CdStatus = cdStatus });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar status com código {cdStatus}", ex);
                }
            }
        }

        #endregion
    }
}