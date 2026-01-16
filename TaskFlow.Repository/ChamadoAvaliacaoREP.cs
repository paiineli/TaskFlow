using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class ChamadoAvaliacaoREP
    {
        private readonly AcessaDados _acessaDados;

        public ChamadoAvaliacaoREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca avaliação de um chamado
        /// </summary>
        public async Task<ChamadoAvaliacaoMOD> BuscarPorChamado(Int32 cdChamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CA.CdAvaliacao,
                               CA.CdChamado,
                               CA.CdFuncionario,
                               CA.NrNota,
                               CA.TxComentario,
                               CA.DtCadastro,
                               F.NmFuncionario
                          FROM TB_CHAMADO_AVALIACAO CA
                          INNER JOIN TB_FUNCIONARIO F ON CA.CdFuncionario = F.CdFuncionario
                         WHERE CA.CdChamado = @CdChamado";

                    return await con.QueryFirstOrDefaultAsync<ChamadoAvaliacaoMOD>(query, new { CdChamado = cdChamado });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar avaliação do chamado {cdChamado}", ex);
                }
            }
        }

        /// <summary>
        /// Busca avaliação por código
        /// </summary>
        public async Task<ChamadoAvaliacaoMOD> BuscarPorCodigo(Int32 cdAvaliacao)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CA.CdAvaliacao,
                               CA.CdChamado,
                               CA.CdFuncionario,
                               CA.NrNota,
                               CA.TxComentario,
                               CA.DtCadastro,
                               F.NmFuncionario
                          FROM TB_CHAMADO_AVALIACAO CA
                          INNER JOIN TB_FUNCIONARIO F ON CA.CdFuncionario = F.CdFuncionario
                         WHERE CA.CdAvaliacao = @CdAvaliacao";

                    return await con.QueryFirstOrDefaultAsync<ChamadoAvaliacaoMOD>(query, new { CdAvaliacao = cdAvaliacao });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar avaliação {cdAvaliacao}", ex);
                }
            }
        }

        /// <summary>
        /// Verifica se o chamado já possui avaliação
        /// </summary>
        public async Task<bool> PossuiAvaliacao(Int32 cdChamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT COUNT(*)
                          FROM TB_CHAMADO_AVALIACAO
                         WHERE CdChamado = @CdChamado";

                    int count = await con.ExecuteScalarAsync<int>(query, new { CdChamado = cdChamado });
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao verificar avaliação do chamado {cdChamado}", ex);
                }
            }
        }

        /// <summary>
        /// Busca média de avaliações de um responsável
        /// </summary>
        public async Task<decimal?> BuscarMediaAvaliacaoResponsavel(Int32 cdResponsavel)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT AVG(CAST(CA.NrNota AS DECIMAL(10,2)))
                          FROM TB_CHAMADO_AVALIACAO CA
                          INNER JOIN TB_CHAMADO C ON CA.CdChamado = C.CdChamado
                         WHERE C.CdResponsavel = @CdResponsavel";

                    return await con.ExecuteScalarAsync<decimal?>(query, new { CdResponsavel = cdResponsavel });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar média de avaliações do responsável {cdResponsavel}", ex);
                }
            }
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Cadastra uma avaliação
        /// </summary>
        public async Task<Int32> Cadastrar(ChamadoAvaliacaoMOD avaliacao)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    // Verificar se já existe avaliação
                    bool jaAvaliado = await PossuiAvaliacao(avaliacao.CdChamado);
                    if (jaAvaliado)
                    {
                        throw new Exception("Este chamado já possui uma avaliação.");
                    }

                    var query = @"
                        INSERT INTO TB_CHAMADO_AVALIACAO
                            (CdChamado, CdFuncionario, NrNota, TxComentario, DtCadastro)
                        VALUES
                            (@CdChamado, @CdFuncionario, @NrNota, @TxComentario, GETDATE());
                        
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    int cdAvaliacao = await con.ExecuteScalarAsync<int>(query, avaliacao);
                    return cdAvaliacao;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cadastrar avaliação", ex);
                }
            }
        }

        #endregion
    }
}
