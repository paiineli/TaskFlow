using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class ChamadoComentarioREP
    {
        private readonly AcessaDados _acessaDados;

        public ChamadoComentarioREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todos os comentários de um chamado
        /// </summary>
        public async Task<List<ChamadoComentarioMOD>> BuscarPorChamado(Int32 cdChamado, bool incluirInternos = true)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CC.CdComentario,
                               CC.CdChamado,
                               CC.CdFuncionario,
                               CC.TxComentario,
                               CC.SnVisibilidadeInterna,
                               CC.DtCadastro,
                               CC.SnAtivo,
                               F.NmFuncionario,
                               F.SnTipoUsuario
                          FROM TB_CHAMADO_COMENTARIO CC
                          INNER JOIN TB_FUNCIONARIO F ON CC.CdFuncionario = F.CdFuncionario
                         WHERE CC.CdChamado = @CdChamado
                           AND CC.SnAtivo = 'S'";

                    if (!incluirInternos)
                        query += " AND CC.SnVisibilidadeInterna = 'N'";

                    query += " ORDER BY CC.DtCadastro ASC";

                    var comentarios = await con.QueryAsync<ChamadoComentarioMOD>(query, new { CdChamado = cdChamado });
                    return comentarios.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar comentários do chamado {cdChamado}", ex);
                }
            }
        }

        /// <summary>
        /// Busca comentário por código
        /// </summary>
        public async Task<ChamadoComentarioMOD> BuscarPorCodigo(Int32 cdComentario)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CC.CdComentario,
                               CC.CdChamado,
                               CC.CdFuncionario,
                               CC.TxComentario,
                               CC.SnVisibilidadeInterna,
                               CC.DtCadastro,
                               CC.SnAtivo,
                               F.NmFuncionario,
                               F.SnTipoUsuario
                          FROM TB_CHAMADO_COMENTARIO CC
                          INNER JOIN TB_FUNCIONARIO F ON CC.CdFuncionario = F.CdFuncionario
                         WHERE CC.CdComentario = @CdComentario";

                    return await con.QueryFirstOrDefaultAsync<ChamadoComentarioMOD>(query, new { CdComentario = cdComentario });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar comentário {cdComentario}", ex);
                }
            }
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Cadastra um novo comentário
        /// </summary>
        public async Task<Int32> Cadastrar(ChamadoComentarioMOD comentario)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        INSERT INTO TB_CHAMADO_COMENTARIO
                            (CdChamado, CdFuncionario, TxComentario, SnVisibilidadeInterna, DtCadastro, SnAtivo)
                        VALUES
                            (@CdChamado, @CdFuncionario, @TxComentario, @SnVisibilidadeInterna, GETDATE(), 'S');
                        
                        -- Atualizar data de última atualização do chamado
                        UPDATE TB_CHAMADO
                           SET DtUltimaAtualizacao = GETDATE()
                         WHERE CdChamado = @CdChamado;
                        
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    int cdComentario = await con.ExecuteScalarAsync<int>(query, new
                    {
                        comentario.CdChamado,
                        comentario.CdFuncionario,
                        comentario.TxComentario,
                        SnVisibilidadeInterna = comentario.SnVisibilidadeInterna ?? "N"
                    });

                    return cdComentario;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cadastrar comentário", ex);
                }
            }
        }

        #endregion

        #region Deletar

        /// <summary>
        /// Inativa um comentário (soft delete)
        /// </summary>
        public async Task<bool> Inativar(Int32 cdComentario)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO_COMENTARIO
                           SET SnAtivo = 'N'
                         WHERE CdComentario = @CdComentario";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdComentario = cdComentario });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inativar comentário", ex);
                }
            }
        }

        #endregion
    }
}
