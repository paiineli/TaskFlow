using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class ChamadoAnexoREP
    {
        private readonly AcessaDados _acessaDados;

        public ChamadoAnexoREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todos os anexos de um chamado
        /// </summary>
        public async Task<List<ChamadoAnexoMOD>> BuscarPorChamado(Int32 cdChamado)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CA.CdAnexo,
                               CA.CdChamado,
                               CA.CdFuncionario,
                               CA.NmArquivo,
                               CA.TxCaminhoArquivo,
                               CA.NrTamanhoBytes,
                               CA.TxTipoMime,
                               CA.DtCadastro,
                               CA.SnAtivo,
                               F.NmFuncionario
                          FROM TB_CHAMADO_ANEXO CA
                          INNER JOIN TB_FUNCIONARIO F ON CA.CdFuncionario = F.CdFuncionario
                         WHERE CA.CdChamado = @CdChamado
                           AND CA.SnAtivo = 'S'
                         ORDER BY CA.DtCadastro DESC";

                    var anexos = await con.QueryAsync<ChamadoAnexoMOD>(query, new { CdChamado = cdChamado });
                    return anexos.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar anexos do chamado {cdChamado}", ex);
                }
            }
        }

        /// <summary>
        /// Busca anexo por código
        /// </summary>
        public async Task<ChamadoAnexoMOD> BuscarPorCodigo(Int32 cdAnexo)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CA.CdAnexo,
                               CA.CdChamado,
                               CA.CdFuncionario,
                               CA.NmArquivo,
                               CA.TxCaminhoArquivo,
                               CA.NrTamanhoBytes,
                               CA.TxTipoMime,
                               CA.DtCadastro,
                               CA.SnAtivo,
                               F.NmFuncionario
                          FROM TB_CHAMADO_ANEXO CA
                          INNER JOIN TB_FUNCIONARIO F ON CA.CdFuncionario = F.CdFuncionario
                         WHERE CA.CdAnexo = @CdAnexo";

                    return await con.QueryFirstOrDefaultAsync<ChamadoAnexoMOD>(query, new { CdAnexo = cdAnexo });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar anexo {cdAnexo}", ex);
                }
            }
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Cadastra um novo anexo
        /// </summary>
        public async Task<Int32> Cadastrar(ChamadoAnexoMOD anexo)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        INSERT INTO TB_CHAMADO_ANEXO
                            (CdChamado, CdFuncionario, NmArquivo, TxCaminhoArquivo, 
                             NrTamanhoBytes, TxTipoMime, DtCadastro, SnAtivo)
                        VALUES
                            (@CdChamado, @CdFuncionario, @NmArquivo, @TxCaminhoArquivo,
                             @NrTamanhoBytes, @TxTipoMime, GETDATE(), 'S');
                        
                        -- Atualizar data de última atualização do chamado
                        UPDATE TB_CHAMADO
                           SET DtUltimaAtualizacao = GETDATE()
                         WHERE CdChamado = @CdChamado;
                        
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    int cdAnexo = await con.ExecuteScalarAsync<int>(query, anexo);
                    return cdAnexo;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cadastrar anexo", ex);
                }
            }
        }

        #endregion

        #region Deletar

        /// <summary>
        /// Inativa um anexo (soft delete)
        /// </summary>
        public async Task<bool> Inativar(Int32 cdAnexo)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CHAMADO_ANEXO
                           SET SnAtivo = 'N'
                         WHERE CdAnexo = @CdAnexo";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdAnexo = cdAnexo });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inativar anexo", ex);
                }
            }
        }

        #endregion
    }
}
