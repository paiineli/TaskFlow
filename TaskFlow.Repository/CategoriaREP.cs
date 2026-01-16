using Dapper;
using System.Data;
using TaskFlow.Data;
using TaskFlow.Model;

namespace TaskFlow.Repository
{
    public class CategoriaREP
    {
        private readonly AcessaDados _acessaDados;

        public CategoriaREP(AcessaDados acessaDados)
        {
            _acessaDados = acessaDados;
        }

        #region Buscar

        /// <summary>
        /// Busca todas as categorias ativas
        /// </summary>
        public async Task<List<CategoriaMOD>> BuscarTodas()
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CdCategoria,
                               NmCategoria,
                               DsCategoria,
                               CdUsuarioCadastro,
                               DtCadastro,
                               CdUsuarioAlteracao,
                               DtAlteracao,
                               SnAtivo
                          FROM TB_CATEGORIA
                         WHERE SnAtivo = 'S'
                         ORDER BY NmCategoria";

                    var categorias = await con.QueryAsync<CategoriaMOD>(query);
                    return categorias.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar categorias", ex);
                }
            }
        }

        /// <summary>
        /// Busca categoria por código
        /// </summary>
        public async Task<CategoriaMOD> BuscarPorCodigo(Int32 cdCategoria)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        SELECT CdCategoria,
                               NmCategoria,
                               DsCategoria,
                               CdUsuarioCadastro,
                               DtCadastro,
                               CdUsuarioAlteracao,
                               DtAlteracao,
                               SnAtivo
                          FROM TB_CATEGORIA
                         WHERE CdCategoria = @CdCategoria";

                    return await con.QueryFirstOrDefaultAsync<CategoriaMOD>(query, new { CdCategoria = cdCategoria });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao buscar categoria {cdCategoria}", ex);
                }
            }
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Cadastra uma nova categoria
        /// </summary>
        public async Task<Int32> Cadastrar(CategoriaMOD categoria)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        INSERT INTO TB_CATEGORIA
                            (NmCategoria, DsCategoria, CdUsuarioCadastro, DtCadastro, SnAtivo)
                        VALUES
                            (@NmCategoria, @DsCategoria, @CdUsuarioCadastro, GETDATE(), 'S');
                        
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    int cdCategoria = await con.ExecuteScalarAsync<int>(query, categoria);
                    return cdCategoria;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao cadastrar categoria", ex);
                }
            }
        }

        #endregion

        #region Atualizar

        /// <summary>
        /// Atualiza uma categoria
        /// </summary>
        public async Task<bool> Atualizar(CategoriaMOD categoria)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CATEGORIA
                           SET NmCategoria = @NmCategoria,
                               DsCategoria = @DsCategoria,
                               CdUsuarioAlteracao = @CdUsuarioAlteracao,
                               DtAlteracao = GETDATE()
                         WHERE CdCategoria = @CdCategoria";

                    int linhasAfetadas = await con.ExecuteAsync(query, categoria);
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar categoria", ex);
                }
            }
        }

        #endregion

        #region Deletar

        /// <summary>
        /// Inativa uma categoria (soft delete)
        /// </summary>
        public async Task<bool> Inativar(Int32 cdCategoria, Int32 cdUsuarioAlteracao)
        {
            using (IDbConnection con = _acessaDados.GetConnection())
            {
                try
                {
                    var query = @"
                        UPDATE TB_CATEGORIA
                           SET SnAtivo = 'N',
                               CdUsuarioAlteracao = @CdUsuarioAlteracao,
                               DtAlteracao = GETDATE()
                         WHERE CdCategoria = @CdCategoria";

                    int linhasAfetadas = await con.ExecuteAsync(query, new { CdCategoria = cdCategoria, CdUsuarioAlteracao = cdUsuarioAlteracao });
                    return linhasAfetadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inativar categoria", ex);
                }
            }
        }

        #endregion
    }
}
