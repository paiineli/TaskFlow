using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Model;
using TaskFlow.Repository;
using TaskFlow.UI.Web.Models;
using X.PagedList;

namespace TaskFlow.UI.Web.Controllers
{
    [Authorize]
    public class ChamadoController : Controller
    {
        private readonly ChamadoREP _repositorioChamado;
        private readonly ChamadoComentarioREP _repositorioComentario;
        private readonly ChamadoAnexoREP _repositorioAnexo;
        private readonly ChamadoHistoricoREP _repositorioHistorico;
        private readonly ChamadoAvaliacaoREP _repositorioAvaliacao;
        private readonly CategoriaREP _repositorioCategoria;
        private readonly StatusREP _repositorioStatus;
        private readonly FuncionarioREP _repositorioFuncionario;
        private readonly JustificativaREP _repositorioJustificativa;

        private const int _take = 10;

        public ChamadoController(
            ChamadoREP repositorioChamado,
            ChamadoComentarioREP repositorioComentario,
            ChamadoAnexoREP repositorioAnexo,
            ChamadoHistoricoREP repositorioHistorico,
            ChamadoAvaliacaoREP repositorioAvaliacao,
            CategoriaREP repositorioCategoria,
            StatusREP repositorioStatus,
            FuncionarioREP repositorioFuncionario,
            JustificativaREP repositorioJustificativa)
        {
            _repositorioChamado = repositorioChamado;
            _repositorioComentario = repositorioComentario;
            _repositorioAnexo = repositorioAnexo;
            _repositorioHistorico = repositorioHistorico;
            _repositorioAvaliacao = repositorioAvaliacao;
            _repositorioCategoria = repositorioCategoria;
            _repositorioStatus = repositorioStatus;
            _repositorioFuncionario = repositorioFuncionario;
            _repositorioJustificativa = repositorioJustificativa;
        }

        #region Index - Listagem de Chamados

        public async Task<IActionResult> Index(int? pagina, BuscaChamadoViewMOD busca)
        {
            int numeroPagina = pagina ?? 1;
            int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")?.Value ?? "0");
            string tipoUsuario = User.FindFirst("SnTipoUsuario")?.Value ?? "C";

            var buscaRepo = new BuscaChamadoMOD
            {
                NrChamado = busca?.NrChamado,
                TxTitulo = busca?.TxTitulo,
                CdCategoria = busca?.CdCategoria,
                CdStatus = busca?.CdStatus,
                CdPrioridade = busca?.CdPrioridade,
                CdResponsavel = busca?.CdResponsavel,
                DtAberturaInicio = busca?.DtAberturaInicio,
                DtAberturaFim = busca?.DtAberturaFim,
                NmSolicitante = busca?.NmSolicitante
            };

            // Se for cliente, mostrar apenas seus chamados
            if (tipoUsuario == "C")
            {
                buscaRepo.CdSolicitante = cdFuncionario;
            }

            // Se solicitou apenas seus chamados (para atendentes/admins)
            if (busca?.ApenasMeusChamados == true && tipoUsuario != "C")
            {
                buscaRepo.CdResponsavel = cdFuncionario;
            }

            // Se solicitou apenas chamados abertos
            if (busca?.ApenasAbertos == true)
            {
                // Status 1 a 4 (Em Espera, Em Análise, Em Atendimento, Aguardando Solicitante)
                // Pode ajustar conforme necessário
            }

            var retorno = await _repositorioChamado.BuscarPaginado(numeroPagina, _take, buscaRepo);

            var chamadoView = new ChamadoViewMOD
            {
                ListaChamadosPaginada = new StaticPagedList<ChamadoMOD>(
                    retorno.Dados, numeroPagina, _take, retorno.Paginacao.TotalItens),
                QtdTotalDeRegistros = retorno.Paginacao.TotalItens,
                Categorias = await _repositorioCategoria.BuscarTodas(),
                Status = _repositorioStatus.BuscarStatus(),
                Atendentes = await _repositorioFuncionario.BuscarAtendentes()
            };

            ViewBag.Busca = busca;
            ViewBag.TipoUsuario = tipoUsuario;

            return View(chamadoView);
        }

        #endregion

        #region Novo Chamado

        [HttpGet]
        public async Task<IActionResult> Novo()
        {
            var categorias = await _repositorioCategoria.BuscarTodas();
            ViewBag.Categorias = categorias;

            var chamadoView = new ChamadoViewMOD
            {
                Chamado = new ChamadoMOD
                {
                    CdPrioridade = 2 // Normal por padrão
                }
            };

            return View(chamadoView);
        }

        [HttpPost]
        public async Task<IActionResult> Novo(ChamadoViewMOD model)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                int cdEmpresa = int.Parse(User.FindFirst("CdEmpresa")!.Value);

                model.Chamado.CdSolicitante = cdFuncionario;
                model.Chamado.CdEmpresa = cdEmpresa;

                int cdChamado = await _repositorioChamado.Cadastrar(model.Chamado);

                if (cdChamado > 0)
                {
                    // Buscar informações para o histórico
                    var categoria = await _repositorioCategoria.BuscarPorCodigo(model.Chamado.CdCategoria);
                    string txPrioridade = model.Chamado.CdPrioridade switch
                    {
                        1 => "Baixa",
                        2 => "Normal",
                        3 => "Alta",
                        4 => "Urgente",
                        _ => "Normal"
                    };

                    // Registrar abertura no histórico
                    await _repositorioHistorico.RegistrarAbertura(
                        cdChamado,
                        cdFuncionario,
                        categoria?.NmCategoria ?? "",
                        txPrioridade
                    );

                    TempData["Modal-Sucesso"] = "Chamado aberto com sucesso!";
                    return RedirectToAction("Detalhes", new { id = cdChamado });
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao abrir chamado.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao abrir chamado: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Detalhes do Chamado

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            try
            {
                var chamado = await _repositorioChamado.BuscarPorCodigo(id);
                if (chamado == null)
                {
                    TempData["Modal-Erro"] = "Chamado não encontrado.";
                    return RedirectToAction("Index");
                }

                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                string tipoUsuario = User.FindFirst("SnTipoUsuario")?.Value ?? "C";

                // Verificar permissão de acesso
                if (tipoUsuario == "C" && chamado.CdSolicitante != cdFuncionario)
                {
                    TempData["Modal-Erro"] = "Você não tem permissão para acessar este chamado.";
                    return RedirectToAction("Index");
                }

                var comentarios = await _repositorioComentario.BuscarPorChamado(id, tipoUsuario != "C");
                var anexos = await _repositorioAnexo.BuscarPorChamado(id);
                var historico = await _repositorioHistorico.BuscarPorChamado(id);
                var avaliacao = await _repositorioAvaliacao.BuscarPorChamado(id);
                var atendentes = await _repositorioFuncionario.BuscarAtendentes();
                var justificativas = await _repositorioJustificativa.BuscarPorTipo("C"); // Cancelamento

                var chamadoView = new ChamadoViewMOD
                {
                    Chamado = chamado,
                    Comentarios = comentarios,
                    Anexos = anexos,
                    Historico = historico,
                    Avaliacao = avaliacao,
                    Atendentes = atendentes,
                    Justificativas = justificativas
                };

                ViewBag.TipoUsuario = tipoUsuario;
                ViewBag.CdFuncionario = cdFuncionario;

                return View(chamadoView);
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao carregar chamado: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Atribuir Chamado

        [HttpPost]
        public async Task<IActionResult> Atribuir(int cdChamado)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                string nmFuncionario = User.FindFirst("NmFuncionario")?.Value ?? "";

                bool sucesso = await _repositorioChamado.AtribuirChamado(cdChamado, cdFuncionario);

                if (sucesso)
                {
                    // Registrar no histórico
                    await _repositorioHistorico.RegistrarAtribuicao(cdChamado, cdFuncionario, nmFuncionario);

                    TempData["Modal-Sucesso"] = "Chamado atribuído com sucesso!";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao atribuir chamado.";
                }

                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao atribuir chamado: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Transferir Chamado

        [HttpPost]
        public async Task<IActionResult> Transferir(int cdChamado, int cdNovoResponsavel)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                
                // Buscar dados do chamado atual
                var chamado = await _repositorioChamado.BuscarPorCodigo(cdChamado);
                var responsavelAnterior = chamado?.NmResponsavel ?? "Não atribuído";
                
                // Buscar novo responsável
                var novoResponsavel = _repositorioFuncionario.BuscarFuncionarioPorCodigo(cdNovoResponsavel);
                var nmNovoResponsavel = novoResponsavel?.NmFuncionario ?? "";

                bool sucesso = await _repositorioChamado.TransferirChamado(cdChamado, cdNovoResponsavel);

                if (sucesso)
                {
                    // Registrar no histórico
                    await _repositorioHistorico.RegistrarTransferencia(
                        cdChamado, 
                        cdFuncionario, 
                        responsavelAnterior, 
                        nmNovoResponsavel
                    );

                    TempData["Modal-Sucesso"] = "Chamado transferido com sucesso!";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao transferir chamado.";
                }

                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao transferir chamado: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
        }

        #endregion

        #region Alterar Status

        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int cdChamado, int cdStatus)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                
                // Buscar status anterior
                var chamado = await _repositorioChamado.BuscarPorCodigo(cdChamado);
                var statusAnterior = chamado?.TxStatus ?? "";
                
                bool sucesso = await _repositorioChamado.AlterarStatus(cdChamado, cdStatus);

                if (sucesso)
                {
                    // Buscar novo status
                    var novoStatus = _repositorioStatus.BuscarStatusPorCodigo(cdStatus);
                    
                    // Registrar no histórico
                    await _repositorioHistorico.RegistrarMudancaStatus(
                        cdChamado, 
                        cdFuncionario, 
                        statusAnterior, 
                        novoStatus?.TxStatus ?? ""
                    );

                    TempData["Modal-Sucesso"] = "Status alterado com sucesso!";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao alterar status.";
                }

                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao alterar status: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
        }

        #endregion

        #region Resolver Chamado

        [HttpPost]
        public async Task<IActionResult> Resolver(int cdChamado)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                
                var chamado = await _repositorioChamado.BuscarPorCodigo(cdChamado);
                var statusAnterior = chamado?.TxStatus ?? "";

                bool sucesso = await _repositorioChamado.ResolverChamado(cdChamado);

                if (sucesso)
                {
                    // Registrar no histórico
                    await _repositorioHistorico.RegistrarMudancaStatus(
                        cdChamado, 
                        cdFuncionario, 
                        statusAnterior, 
                        "Resolvido",
                        "Chamado marcado como resolvido"
                    );

                    TempData["Modal-Sucesso"] = "Chamado resolvido com sucesso! Aguardando confirmação do solicitante.";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao resolver chamado.";
                }

                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao resolver chamado: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
        }

        #endregion

        #region Fechar Chamado

        [HttpGet]
        public async Task<IActionResult> Fechar(int id)
        {
            try
            {
                var chamado = await _repositorioChamado.BuscarPorCodigo(id);
                if (chamado == null)
                {
                    TempData["Modal-Erro"] = "Chamado não encontrado.";
                    return RedirectToAction("Index");
                }

                var justificativas = await _repositorioJustificativa.BuscarPorTipo("F"); // Fechamento
                ViewBag.Justificativas = justificativas;

                var chamadoView = new ChamadoViewMOD
                {
                    Chamado = chamado
                };

                return View(chamadoView);
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao carregar página: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Fechar(int cdChamado, int? cdJustificativa, string? txJustificativaComplemento)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                
                var chamado = await _repositorioChamado.BuscarPorCodigo(cdChamado);
                var statusAnterior = chamado?.TxStatus ?? "";

                bool sucesso = await _repositorioChamado.FecharChamado(cdChamado, cdJustificativa, txJustificativaComplemento);

                if (sucesso)
                {
                    // Registrar no histórico
                    await _repositorioHistorico.RegistrarMudancaStatus(
                        cdChamado, 
                        cdFuncionario, 
                        statusAnterior, 
                        "Fechado",
                        "Chamado fechado"
                    );

                    TempData["Modal-Sucesso"] = "Chamado fechado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao fechar chamado.";
                    return RedirectToAction("Detalhes", new { id = cdChamado });
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao fechar chamado: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
        }

        #endregion

        #region Cancelar Chamado

        [HttpGet]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var chamado = await _repositorioChamado.BuscarPorCodigo(id);
                if (chamado == null)
                {
                    TempData["Modal-Erro"] = "Chamado não encontrado.";
                    return RedirectToAction("Index");
                }

                var justificativas = await _repositorioJustificativa.BuscarPorTipo("C"); // Cancelamento
                ViewBag.Justificativas = justificativas;

                var chamadoView = new ChamadoViewMOD
                {
                    Chamado = chamado
                };

                return View(chamadoView);
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao carregar página: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancelar(int cdChamado, int cdJustificativa, string? txJustificativaComplemento)
        {
            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);
                
                var chamado = await _repositorioChamado.BuscarPorCodigo(cdChamado);
                var statusAnterior = chamado?.TxStatus ?? "";

                bool sucesso = await _repositorioChamado.CancelarChamado(cdChamado, cdJustificativa, txJustificativaComplemento);

                if (sucesso)
                {
                    // Registrar no histórico
                    await _repositorioHistorico.RegistrarMudancaStatus(
                        cdChamado, 
                        cdFuncionario, 
                        statusAnterior, 
                        "Cancelado",
                        "Chamado cancelado"
                    );

                    TempData["Modal-Sucesso"] = "Chamado cancelado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao cancelar chamado.";
                    return RedirectToAction("Detalhes", new { id = cdChamado });
                }
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao cancelar chamado: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = cdChamado });
            }
        }

        #endregion

        #region Adicionar Comentário

        [HttpPost]
        public async Task<IActionResult> AdicionarComentario(NovoComentarioViewMOD model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Modal-Erro"] = "Por favor, preencha o comentário.";
                return RedirectToAction("Detalhes", new { id = model.CdChamado });
            }

            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);

                var comentario = new ChamadoComentarioMOD
                {
                    CdChamado = model.CdChamado,
                    CdFuncionario = cdFuncionario,
                    TxComentario = model.TxComentario,
                    SnVisibilidadeInterna = model.SnVisibilidadeInterna ? "S" : "N"
                };

                int cdComentario = await _repositorioComentario.Cadastrar(comentario);

                if (cdComentario > 0)
                {
                    TempData["Modal-Sucesso"] = "Comentário adicionado com sucesso!";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao adicionar comentário.";
                }

                return RedirectToAction("Detalhes", new { id = model.CdChamado });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao adicionar comentário: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = model.CdChamado });
            }
        }

        #endregion

        #region Avaliar Chamado

        [HttpPost]
        public async Task<IActionResult> Avaliar(AvaliacaoViewMOD model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Modal-Erro"] = "Por favor, preencha a avaliação corretamente.";
                return RedirectToAction("Detalhes", new { id = model.CdChamado });
            }

            try
            {
                int cdFuncionario = int.Parse(User.FindFirst("CdFuncionario")!.Value);

                var avaliacao = new ChamadoAvaliacaoMOD
                {
                    CdChamado = model.CdChamado,
                    CdFuncionario = cdFuncionario,
                    NrNota = model.NrNota,
                    TxComentario = model.TxComentario
                };

                int cdAvaliacao = await _repositorioAvaliacao.Cadastrar(avaliacao);

                if (cdAvaliacao > 0)
                {
                    TempData["Modal-Sucesso"] = "Avaliação registrada com sucesso! Obrigado pelo seu feedback.";
                }
                else
                {
                    TempData["Modal-Erro"] = "Erro ao registrar avaliação.";
                }

                return RedirectToAction("Detalhes", new { id = model.CdChamado });
            }
            catch (Exception ex)
            {
                TempData["Modal-Erro"] = $"Erro ao avaliar: {ex.Message}";
                return RedirectToAction("Detalhes", new { id = model.CdChamado });
            }
        }

        #endregion
    }
}
