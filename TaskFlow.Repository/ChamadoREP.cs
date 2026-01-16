using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;
using TaskFlow.Model.Commom;

namespace TaskFlow.Repository
{
    public class ChamadoREP
    {
        private readonly AcessaDados _acessaDados;

        public ChamadoREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Realiza a busca paginada de chamados com filtros
        /// </summary>
        public async Task<PaginacaoResposta<ChamadoMOD>> BuscarPaginado(Int32 pagina, Int32 itensPorPagina, BuscaChamadoMOD busca = null)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    int offset = (pagina - 1) * itensPorPagina;

                    var parametros = new DynamicParameters();
                    parametros.Add("@Offset", offset);
                    parametros.Add("@ItensPorPagina", itensPorPagina);
                    parametros.Add("@NrChamado", busca?.NrChamado);
                    parametros.Add("@TxTitulo", busca?.TxTitulo);
                    parametros.Add("@CdCategoria", busca?.CdCategoria);
                    parametros.Add("@CdStatus", busca?.CdStatus);
                    parametros.Add("@CdPrioridade", busca?.CdPrioridade);
                    parametros.Add("@CdSolicitante", busca?.CdSolicitante);
                    parametros.Add("@CdResponsavel", busca?.CdResponsavel);
                    parametros.Add("@DtAberturaInicio", busca?.DtAberturaInicio);
                    parametros.Add("@DtAberturaFim", busca?.DtAberturaFim);
                    parametros.Add("@NmSolicitante", busca?.NmSolicitante);

                    // Query principal com joins
                    var query = @"
                        SELECT C.CdChamado,
                               C.NrChamado,
                               C.TxTitulo,
                               C.TxDescricao,
                               C.CdCategoria,
                               C.CdStatus,
                               C.CdPrioridade,
                               C.CdSolicitante,
                               C.CdResponsavel,
                               C.CdEmpresa,
                               C.DtAbertura,
                               C.DtUltimaAtualizacao,
                               C.DtPrazo,
                               C.DtResolucao,
                               C.DtFechamento,
                               C.CdJustificativa,
                               C.TxJustificativaComplemento,
                               C.SnAtivo,
                               CAT.NmCategoria,
                               S.TxStatus,
                               FSOL.NmFuncionario AS NmSolicitante,
                               FRESP.NmFuncionario AS NmResponsavel,
                               CASE C.CdPrioridade
                                   WHEN 1 THEN 'Baixa'
                                   WHEN 2 THEN 'Normal'
                                   WHEN 3 THEN 'Alta'
                                   WHEN 4 THEN 'Urgente'
                                   ELSE 'Indefinida'
                               END AS TxPrioridade,
                               DATEDIFF(HOUR, C.DtAbertura, COALESCE(C.DtFechamento, GETDATE())) AS NrHorasAberto,
                               (SELECT COUNT(*) FROM TB_CHAMADO_COMENTARIO WHERE CdChamado = C.CdChamado AND SnAtivo = 'S') AS QtdComentarios,
                               (SELECT COUNT(*) FROM TB_CHAMADO_ANEXO WHERE CdChamado = C.CdChamado AND SnAtivo = 'S') AS QtdAnexos,
                               CASE WHEN EXISTS(SELECT 1 FROM TB_CHAMADO_AVALIACAO WHERE CdChamado = C.CdChamado) THEN 1 ELSE 0 END AS SnPossuiAvaliacao
                          FROM TB_CHAMADO C
                          INNER JOIN TB_CATEGORIA CAT ON C.CdCategoria = CAT.CdCategoria
                          INNER JOIN TB_STATUS S ON C.CdStatus = S.CdStatus
                          INNER JOIN TB_FUNCIONARIO FSOL ON C.CdSolicitante = FSOL.CdFuncionario
                          LEFT JOIN TB_FUNCIONARIO FRESP ON C.CdResponsavel = FRESP.CdFuncionario
                         WHERE C.SnAtivo = 'S'
                           AND (@NrChamado IS NULL OR C.NrChamado LIKE '%' + @NrChamado + '%')
                           AND (@TxTitulo IS NULL OR C.TxTitulo LIKE '%' + @TxTitulo + '%')
                           AND (@CdCategoria IS NULL OR C.CdCategoria = @CdCategoria)
                           AND (@CdStatus IS NULL OR C.CdStatus = @CdStatus)
                           AND (@CdPrioridade IS NULL OR C.CdPrioridade = @CdPrioridade)
                           AND (@CdSolicitante IS NULL OR C.CdSolicitante = @CdSolicitante)
                           AND (@CdResponsavel IS NULL OR C.CdResponsavel = @CdResponsavel)
                           AND (@DtAberturaInicio IS NULL OR C.DtAbertura >= @DtAberturaInicio)
                           AND (@DtAberturaFim IS NULL OR C.DtAbertura <= @DtAberturaFim)
                           AND (@NmSolicitante IS NULL OR FSOL.NmFuncionario LIKE '%' + @NmSolicitante + '%')
                         ORDER BY C.DtUltimaAtualizacao DESC
                        OFFSET @Offset ROWS
                         FETCH NEXT @ItensPorPagina ROWS ONLY";

                    var chamados = (await con.QueryAsync<ChamadoMOD>(query, parametros)).ToList();

                    // Total de itens
                    var totalQuery = @"
                        SELECT COUNT(*)
                          FROM TB_CHAMADO C
                          INNER JOIN TB_FUNCIONARIO FSOL ON C.CdSolicitante = FSOL.CdFuncionario
                         WHERE C.SnAtivo = 'S'
                           AND (@NrChamado IS NULL OR C.NrChamado LIKE '%' + @NrChamado + '%')
                           AND (@TxTitulo IS NULL OR C.TxTitulo LIKE '%' + @TxTitulo + '%')
                           AND (@CdCategoria IS NULL OR C.CdCategoria = @CdCategoria)
                           AND (@CdStatus IS NULL OR C.CdStatus = @CdStatus)
                           AND (@CdPrioridade IS NULL OR C.CdPrioridade = @CdPrioridade)
                           AND (@CdSolicitante IS NULL OR C.CdSolicitante = @CdSolicitante)
                           AND (@CdResponsavel IS NULL OR C.CdResponsavel = @CdResponsavel)
                           AND (@DtAberturaInicio IS NULL OR C.DtAbertura >= @DtAberturaInicio)
                           AND (@DtAberturaFim IS NULL OR C.DtAbertura <= @DtAberturaFim)
                           AND (@NmSolicitante IS NULL OR FSOL.NmFuncionario LIKE '%' + @NmSolicitante + '%')";

                    int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);

                    return new PaginacaoResposta<ChamadoMOD>
                    {
                        Dados = chamados,
                        Paginacao = new Paginacao
                        {
                            PaginaAtual = pagina,
                            QuantidadePorPagina = itensPorPagina,
                            TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina),
                            TotalItens = totalItens
                        }
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar chamados paginados", ex);
                }
            }
        }

        /// <summary>
        /// Busca chamado por código
        /// </summary>
        public async Task<ChamadoMOD> BuscarPorCodigo(Int32 cdChamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT C.CdChamado,
                               C.NrChamado,
                               C.TxTitulo,
                               C.TxDescricao,
                               C.CdCategoria,
                               C.CdStatus,
                               C.CdPrioridade,
                               C.CdSolicitante,
                               C.CdResponsavel,
                               C.CdEmpresa,
                               C.DtAbertura,
                               C.DtUltimaAtualizacao,
                               C.DtPrazo,
                               C.DtResolucao,
                               C.DtFechamento,
                               C.CdJustificativa,
                               C.TxJustificativaComplemento,
                               C.SnAtivo,
                               CAT.NmCategoria,
                               S.TxStatus,
                               FSOL.NmFuncionario AS NmSolicitante,
                               FRESP.NmFuncionario AS NmResponsavel,
                               CASE C.CdPrioridade
                                   WHEN 1 THEN 'Baixa'
                                   WHEN 2 THEN 'Normal'
                                   WHEN 3 THEN 'Alta'
                                   WHEN 4 THEN 'Urgente'
                                   ELSE 'Indefinida'
                               END AS TxPrioridade,
                               DATEDIFF(HOUR, C.DtAbertura, COALESCE(C.DtFechamento, GETDATE())) AS NrHorasAberto,
                               (SELECT COUNT(*) FROM TB_CHAMADO_COMENTARIO WHERE CdChamado = C.CdChamado AND SnAtivo = 'S') AS QtdComentarios,
                               (SELECT COUNT(*) FROM TB_CHAMADO_ANEXO WHERE CdChamado = C.CdChamado AND SnAtivo = 'S') AS QtdAnexos,
                               CASE WHEN EXISTS(SELECT 1 FROM TB_CHAMADO_AVALIACAO WHERE CdChamado = C.CdChamado) THEN 1 ELSE 0 END AS SnPossuiAvaliacao
                          FROM TB_CHAMADO C
                          INNER JOIN TB_CATEGORIA CAT ON C.CdCategoria = CAT.CdCategoria
                          INNER JOIN TB_STATUS S ON C.CdStatus = S.CdStatus
                          INNER JOIN TB_FUNCIONARIO FSOL ON C.CdSolicitante = FSOL.CdFuncionario
                          LEFT JOIN TB_FUNCIONARIO FRESP ON C.CdResponsavel = FRESP.CdFuncionario
                         WHERE C.CdChamado = @CdChamado";

                    return await con.QueryFirstOrDefaultAsync<ChamadoMOD>(query, new { CdChamado = cdChamado });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar chamado {cdChamado}", ex);
                }
            }
        }

        /// <summary>
        /// Busca chamado por número
        /// </summary>
        public async Task<ChamadoMOD> BuscarPorNumero(string nrChamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT C.CdChamado,
                               C.NrChamado,
                               C.TxTitulo,
                               C.TxDescricao,
                               C.CdCategoria,
                               C.CdStatus,
                               C.CdPrioridade,
                               C.CdSolicitante,
                               C.CdResponsavel,
                               C.CdEmpresa,
                               C.DtAbertura,
                               C.DtUltimaAtualizacao,
                               C.DtPrazo,
                               C.DtResolucao,
                               C.DtFechamento,
                               C.CdJustificativa,
                               C.TxJustificativaComplemento,
                               C.SnAtivo
                          FROM TB_CHAMADO C
                         WHERE C.NrChamado = @NrChamado";

                    return await con.QueryFirstOrDefaultAsync<ChamadoMOD>(query, new { NrChamado = nrChamado });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar chamado {nrChamado}", ex);
                }
            }
        }

        /// <summary>
        /// Busca chamados atribuídos a um funcionário
        /// </summary>
        public async Task<PaginacaoResposta<ChamadoMOD>> BuscarMeusChamados(Int32 cdFuncionario, Int32 pagina, Int32 itensPorPagina)
        {
            var busca = new BuscaChamadoMOD { CdResponsavel = cdFuncionario };
            return await BuscarPaginado(pagina, itensPorPagina, busca);
        }

        /// <summary>
        /// Busca chamados abertos por um solicitante
        /// </summary>
        public async Task<PaginacaoResposta<ChamadoMOD>> BuscarChamadosSolicitante(Int32 cdSolicitante, Int32 pagina, Int32 itensPorPagina)
        {
            var busca = new BuscaChamadoMOD { CdSolicitante = cdSolicitante };
            return await BuscarPaginado(pagina, itensPorPagina, busca);
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Cadastra um novo chamado
        /// </summary>
        public async Task<Int32> Cadastrar(ChamadoMOD chamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    // Gerar número do chamado
                    var nrChamado = await GerarNumeroChamado(con);

                    var query = @"
                        INSERT INTO TB_CHAMADO
                            (NrChamado, TxTitulo, TxDescricao, CdCategoria, CdStatus, 
                             CdPrioridade, CdSolicitante, CdEmpresa, DtAbertura, DtUltimaAtualizacao, SnAtivo)
                        VALUES
                            (@NrChamado, @TxTitulo, @TxDescricao, @CdCategoria, @CdStatus,
                             @CdPrioridade, @CdSolicitante, @CdEmpresa, GETDATE(), GETDATE(), 'S');
                        
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    int cdChamado = await con.ExecuteScalarAsync<int>(query, new
                    {
                        NrChamado = nrChamado,
                        chamado.TxTitulo,
                        chamado.TxDescricao,
                        chamado.CdCategoria,
                        CdStatus = 1, // Em Espera
                        chamado.CdPrioridade,
                        chamado.CdSolicitante,
                        chamado.CdEmpresa
                    });

                    return cdChamado;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cadastrar chamado", ex);
                }
            }
        }

        #endregion

        #region Atualizar

        /// <summary>
        /// Atribui chamado a um responsável
        /// </summary>
        public async Task<bool> AtribuirChamado(Int32 cdChamado, Int32 cdResponsavel)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO
                           SET CdResponsavel = @CdResponsavel,
                               CdStatus = 3, -- Em Atendimento
                               DtUltimaAtualizacao = GETDATE()
                         WHERE CdChamado = @CdChamado";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdChamado = cdChamado, CdResponsavel = cdResponsavel });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atribuir chamado", ex);
                }
            }
        }

        /// <summary>
        /// Transfere chamado para outro responsável
        /// </summary>
        public async Task<bool> TransferirChamado(Int32 cdChamado, Int32 cdNovoResponsavel)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO
                           SET CdResponsavel = @CdNovoResponsavel,
                               DtUltimaAtualizacao = GETDATE()
                         WHERE CdChamado = @CdChamado";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdChamado = cdChamado, CdNovoResponsavel = cdNovoResponsavel });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao transferir chamado", ex);
                }
            }
        }

        /// <summary>
        /// Altera o status do chamado
        /// </summary>
        public async Task<bool> AlterarStatus(Int32 cdChamado, Int32 cdStatus)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO
                           SET CdStatus = @CdStatus,
                               DtUltimaAtualizacao = GETDATE()";

                    // Se status = Resolvido, atualizar data de resolução
                    if (cdStatus == 5)
                        query += ", DtResolucao = GETDATE()";

                    // Se status = Fechado, atualizar data de fechamento
                    if (cdStatus == 6)
                        query += ", DtFechamento = GETDATE()";

                    query += " WHERE CdChamado = @CdChamado";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdChamado = cdChamado, CdStatus = cdStatus });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao alterar status do chamado", ex);
                }
            }
        }

        /// <summary>
        /// Altera a prioridade do chamado
        /// </summary>
        public async Task<bool> AlterarPrioridade(Int32 cdChamado, Int32 cdPrioridade)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO
                           SET CdPrioridade = @CdPrioridade,
                               DtUltimaAtualizacao = GETDATE()
                         WHERE CdChamado = @CdChamado";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdChamado = cdChamado, CdPrioridade = cdPrioridade });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao alterar prioridade do chamado", ex);
                }
            }
        }

        /// <summary>
        /// Resolve o chamado
        /// </summary>
        public async Task<bool> ResolverChamado(Int32 cdChamado)
        {
            return await AlterarStatus(cdChamado, 5); // Status: Resolvido
        }

        /// <summary>
        /// Fecha o chamado com justificativa
        /// </summary>
        public async Task<bool> FecharChamado(Int32 cdChamado, Int32? cdJustificativa, string txJustificativaComplemento)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO
                           SET CdStatus = 6, -- Fechado
                               DtFechamento = GETDATE(),
                               DtUltimaAtualizacao = GETDATE(),
                               CdJustificativa = @CdJustificativa,
                               TxJustificativaComplemento = @TxJustificativaComplemento
                         WHERE CdChamado = @CdChamado";

                    int linhasAfetadas = await con.ExecuteAsync(query, new
                    {
                        CdChamado = cdChamado,
                        CdJustificativa = cdJustificativa,
                        TxJustificativaComplemento = txJustificativaComplemento
                    });

                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao fechar chamado", ex);
                }
            }
        }

        /// <summary>
        /// Cancela o chamado com justificativa
        /// </summary>
        public async Task<bool> CancelarChamado(Int32 cdChamado, Int32 cdJustificativa, string txJustificativaComplemento)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO
                           SET CdStatus = 7, -- Cancelado
                               DtFechamento = GETDATE(),
                               DtUltimaAtualizacao = GETDATE(),
                               CdJustificativa = @CdJustificativa,
                               TxJustificativaComplemento = @TxJustificativaComplemento
                         WHERE CdChamado = @CdChamado";

                    int linhasAfetadas = await con.ExecuteAsync(query, new
                    {
                        CdChamado = cdChamado,
                        CdJustificativa = cdJustificativa,
                        TxJustificativaComplemento = txJustificativaComplemento
                    });

                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cancelar chamado", ex);
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gera número automático do chamado (CHM-2025-00001)
        /// </summary>
        private async Task<string> GerarNumeroChamado(IDbConnection con)
        {
            try
            {
                var query = "EXEC SP_GERAR_NUMERO_CHAMADO @NrChamado OUTPUT";
                var parametros = new DynamicParameters();
                parametros.Add("@NrChamado", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

                await con.ExecuteAsync(query, parametros);

                return parametros.Get<string>("@NrChamado");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar número do chamado", ex);
            }
        }

        #endregion
    }
}
