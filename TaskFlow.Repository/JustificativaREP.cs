using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class JustificativaREP
    {
        private readonly AcessaDados _acessaDados;

        public JustificativaREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todas as Justificativas cadastradas
        /// </summary>
        public List<JustificativaMOD> BuscarJustificativas()
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdJustificativa,
                                         TxJustificativa,
                                         SnTipoJustificativa,
                                         CdUsuarioCadastro,
                                         DtCadastro,
                                         CdUsuarioAlteracao,
                                         DtAlteracao,
                                         SnAtivo
                                    FROM TB_JUSTIFICATIVA
                                   WHERE SnAtivo = 'S'
                                   ORDER BY TxJustificativa";

                    return con.Query<JustificativaMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar justificativas", ex);
                }
            }
        }

        /// <summary>
        /// Busca justificativas por tipo (F=Fechamento, C=Cancelamento)
        /// </summary>
        public async Task<List<JustificativaMOD>> BuscarPorTipo(string snTipoJustificativa)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdJustificativa,
                                         TxJustificativa,
                                         SnTipoJustificativa,
                                         CdUsuarioCadastro,
                                         DtCadastro,
                                         CdUsuarioAlteracao,
                                         DtAlteracao,
                                         SnAtivo
                                    FROM TB_JUSTIFICATIVA
                                   WHERE SnTipoJustificativa = @SnTipoJustificativa
                                     AND SnAtivo = 'S'
                                   ORDER BY TxJustificativa";

                    var justificativas = await con.QueryAsync<JustificativaMOD>(query, new { SnTipoJustificativa = snTipoJustificativa });
                    return justificativas.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar justificativas do tipo {snTipoJustificativa}", ex);
                }
            }
        }

        /// <summary>
        /// Busca uma Justificativa específica pelo código
        /// </summary>
        public JustificativaMOD BuscarJustificativaPorCodigo(Int32 cdJustificativa)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"SELECT CdJustificativa,
                                         TxJustificativa,
                                         SnTipoJustificativa,
                                         CdUsuarioCadastro,
                                         DtCadastro,
                                         CdUsuarioAlteracao,
                                         DtAlteracao,
                                         SnAtivo
                                    FROM TB_JUSTIFICATIVA
                                   WHERE CdJustificativa = @CdJustificativa";

                    return con.QueryFirstOrDefault<JustificativaMOD>(query, new { CdJustificativa = cdJustificativa });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar justificativa com código {cdJustificativa}", ex);
                }
            }
        }

        #endregion
    }
}
