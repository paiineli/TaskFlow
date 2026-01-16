using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class ChamadoHistoricoREP
    {
        private readonly AcessaDados _acessaDados;

        public ChamadoHistoricoREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todo o histórico de um chamado
        /// </summary>
        public async Task<List<ChamadoHistoricoMOD>> BuscarPorChamado(Int32 cdChamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CH.CdHistorico,
                               CH.CdChamado,
                               CH.CdFuncionario,
                               CH.TxAcao,
                               CH.TxValorAnterior,
                               CH.TxValorNovo,
                               CH.TxObservacao,
                               CH.DtCadastro,
                               F.NmFuncionario
                          FROM TB_CHAMADO_HISTORICO CH
                          INNER JOIN TB_FUNCIONARIO F ON CH.CdFuncionario = F.CdFuncionario
                         WHERE CH.CdChamado = @CdChamado
                         ORDER BY CH.DtCadastro DESC";

                    var historico = await con.QueryAsync<ChamadoHistoricoMOD>(query, new { CdChamado = cdChamado });
                    return historico.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar histórico do chamado {cdChamado}", ex);
                }
            }
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Registra uma ação no histórico
        /// </summary>
        public async Task<Int32> RegistrarAcao(ChamadoHistoricoMOD historico)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        INSERT INTO TB_CHAMADO_HISTORICO
                            (CdChamado, CdFuncionario, TxAcao, TxValorAnterior, TxValorNovo, TxObservacao, DtCadastro)
                        VALUES
                            (@CdChamado, @CdFuncionario, @TxAcao, @TxValorAnterior, @TxValorNovo, @TxObservacao, GETDATE());
                        
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    int cdHistorico = await con.ExecuteScalarAsync<int>(query, historico);
                    return cdHistorico;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao registrar ação no histórico", ex);
                }
            }
        }

        /// <summary>
        /// Registra abertura do chamado
        /// </summary>
        public async Task<Int32> RegistrarAbertura(Int32 cdChamado, Int32 cdFuncionario, string nmCategoria, string txPrioridade)
        {
            var historico = new ChamadoHistoricoMOD
            {
                CdChamado = cdChamado,
                CdFuncionario = cdFuncionario,
                TxAcao = "CHAMADO_ABERTO",
                TxValorAnterior = null,
                TxValorNovo = $"Categoria: {nmCategoria} | Prioridade: {txPrioridade}",
                TxObservacao = "Chamado aberto"
            };

            return await RegistrarAcao(historico);
        }

        /// <summary>
        /// Registra atribuição do chamado
        /// </summary>
        public async Task<Int32> RegistrarAtribuicao(Int32 cdChamado, Int32 cdFuncionario, string nmResponsavel)
        {
            var historico = new ChamadoHistoricoMOD
            {
                CdChamado = cdChamado,
                CdFuncionario = cdFuncionario,
                TxAcao = "ATRIBUIDO",
                TxValorAnterior = null,
                TxValorNovo = nmResponsavel,
                TxObservacao = "Chamado atribuído"
            };

            return await RegistrarAcao(historico);
        }

        /// <summary>
        /// Registra transferência do chamado
        /// </summary>
        public async Task<Int32> RegistrarTransferencia(Int32 cdChamado, Int32 cdFuncionario, string nmResponsavelAnterior, string nmNovoResponsavel)
        {
            var historico = new ChamadoHistoricoMOD
            {
                CdChamado = cdChamado,
                CdFuncionario = cdFuncionario,
                TxAcao = "TRANSFERIDO",
                TxValorAnterior = nmResponsavelAnterior,
                TxValorNovo = nmNovoResponsavel,
                TxObservacao = "Chamado transferido"
            };

            return await RegistrarAcao(historico);
        }

        /// <summary>
        /// Registra mudança de status
        /// </summary>
        public async Task<Int32> RegistrarMudancaStatus(Int32 cdChamado, Int32 cdFuncionario, string txStatusAnterior, string txStatusNovo, string observacao = null)
        {
            var historico = new ChamadoHistoricoMOD
            {
                CdChamado = cdChamado,
                CdFuncionario = cdFuncionario,
                TxAcao = "STATUS_ALTERADO",
                TxValorAnterior = txStatusAnterior,
                TxValorNovo = txStatusNovo,
                TxObservacao = observacao
            };

            return await RegistrarAcao(historico);
        }

        /// <summary>
        /// Registra mudança de prioridade
        /// </summary>
        public async Task<Int32> RegistrarMudancaPrioridade(Int32 cdChamado, Int32 cdFuncionario, string txPrioridadeAnterior, string txPrioridadeNova, string observacao = null)
        {
            var historico = new ChamadoHistoricoMOD
            {
                CdChamado = cdChamado,
                CdFuncionario = cdFuncionario,
                TxAcao = "PRIORIDADE_ALTERADA",
                TxValorAnterior = txPrioridadeAnterior,
                TxValorNovo = txPrioridadeNova,
                TxObservacao = observacao
            };

            return await RegistrarAcao(historico);
        }

        #endregion
    }
}
