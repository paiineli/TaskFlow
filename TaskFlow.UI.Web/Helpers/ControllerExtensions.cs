using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TaskFlow.UI.Web.Helpers
{
    public static class ControllerExtensions
    {
        #region Render Async

        /// <summary>
        /// Renderiza uma view para string de forma assíncrona
        /// </summary>
        /// <typeparam name="TModel">Tipo do modelo</typeparam>
        /// <param name="controller">Controller atual</param>
        /// <param name="viewName">Nome da view</param>
        /// <param name="model">Modelo da view</param>
        /// <param name="partial">Se é partial view</param>
        /// <returns>HTML renderizado como string</returns>
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                if (viewResult.Success == false)
                {
                    return $"A view com o nome '{viewName}' não foi encontrada";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        #endregion

        #region Auxiliares de Exibição

        /// <summary>
        /// Exibe o valor da string ou "Não informado" se vazio
        /// </summary>
        public static string MostrarOuNaoInformado(this string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "Não informado" : valor;
        }

        /// <summary>
        /// Exibe a data formatada ou "Não informado" se vazio
        /// </summary>
        public static string MostrarDataOuNaoInformado(this DateTime? data, bool exibirHora = false)
        {
            if (!data.HasValue || data.Value == DateTime.MinValue)
                return "Não informado";

            return data.Value.ToString(exibirHora ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy");
        }

        /// <summary>
        /// Sobrecarga para DateTime não-nullable
        /// </summary>
        public static string MostrarDataOuNaoInformado(this DateTime data, bool exibirHora = false)
        {
            return data != DateTime.MinValue
                ? data.ToString(exibirHora ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy")
                : "Não informado";
        }

        /// <summary>
        /// Exibe o número ou "Não informado" se zero
        /// </summary>
        public static string MostrarNumeroOuNaoInformado(this int numero)
        {
            return numero > 0
                ? numero.ToString()
                : "Não informado";
        }

        /// <summary>
        /// Exibe o número ou "Não informado" se null/zero
        /// </summary>
        public static string MostrarNumeroOuNaoInformado(this int? numero)
        {
            return numero.HasValue && numero.Value > 0
                ? numero.Value.ToString()
                : "Não informado";
        }

        #endregion

        #region Helpers de Formatação

        /// <summary>
        /// Formata CPF (000.000.000-00)
        /// </summary>
        public static string FormatarCpf(this string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                return cpf ?? string.Empty;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formata tamanho de arquivo em bytes para formato legível
        /// </summary>
        public static string FormatarTamanhoArquivo(this long bytes)
        {
            string[] tamanhos = { "B", "KB", "MB", "GB", "TB" };
            double tamanho = bytes;
            int ordem = 0;

            while (tamanho >= 1024 && ordem < tamanhos.Length - 1)
            {
                ordem++;
                tamanho /= 1024;
            }

            return $"{tamanho:0.##} {tamanhos[ordem]}";
        }

        #endregion

        #region Badges de Status e Prioridade - Chamados

        /// <summary>
        /// Retorna badge HTML de acordo com o status do chamado
        /// </summary>
        public static string BadgeStatusChamado(this int cdStatus)
        {
            return cdStatus switch
            {
                1 => "<span class='badge bg-secondary'>Em Espera</span>",
                2 => "<span class='badge bg-info'>Em Análise</span>",
                3 => "<span class='badge bg-primary'>Em Atendimento</span>",
                4 => "<span class='badge bg-warning text-dark'>Aguardando Solicitante</span>",
                5 => "<span class='badge bg-success'>Resolvido</span>",
                6 => "<span class='badge bg-dark'>Fechado</span>",
                7 => "<span class='badge bg-danger'>Cancelado</span>",
                _ => "<span class='badge bg-secondary'>Desconhecido</span>"
            };
        }

        /// <summary>
        /// Retorna badge HTML de acordo com o status do chamado (sobrecarga por nome)
        /// </summary>
        public static string BadgeStatusChamado(this string? txStatus)
        {
            if (string.IsNullOrWhiteSpace(txStatus))
                return "<span class='badge bg-secondary'>Desconhecido</span>";

            return txStatus.ToLower() switch
            {
                "em espera" => "<span class='badge bg-secondary'>Em Espera</span>",
                "em análise" => "<span class='badge bg-info'>Em Análise</span>",
                "em atendimento" => "<span class='badge bg-primary'>Em Atendimento</span>",
                "aguardando solicitante" => "<span class='badge bg-warning text-dark'>Aguardando Solicitante</span>",
                "resolvido" => "<span class='badge bg-success'>Resolvido</span>",
                "fechado" => "<span class='badge bg-dark'>Fechado</span>",
                "cancelado" => "<span class='badge bg-danger'>Cancelado</span>",
                _ => $"<span class='badge bg-secondary'>{txStatus}</span>"
            };
        }

        /// <summary>
        /// Retorna badge HTML de acordo com a prioridade
        /// </summary>
        public static string BadgePrioridade(this int cdPrioridade)
        {
            return cdPrioridade switch
            {
                1 => "<span class='badge bg-secondary'>Baixa</span>",
                2 => "<span class='badge bg-primary'>Normal</span>",
                3 => "<span class='badge bg-warning text-dark'>Alta</span>",
                4 => "<span class='badge bg-danger'>Urgente</span>",
                _ => "<span class='badge bg-secondary'>Indefinida</span>"
            };
        }

        /// <summary>
        /// Retorna badge HTML de acordo com a prioridade (sobrecarga por nome)
        /// </summary>
        public static string BadgePrioridade(this string? txPrioridade)
        {
            if (string.IsNullOrWhiteSpace(txPrioridade))
                return "<span class='badge bg-secondary'>Indefinida</span>";

            return txPrioridade.ToLower() switch
            {
                "baixa" => "<span class='badge bg-secondary'>Baixa</span>",
                "normal" => "<span class='badge bg-primary'>Normal</span>",
                "alta" => "<span class='badge bg-warning text-dark'>Alta</span>",
                "urgente" => "<span class='badge bg-danger'>Urgente</span>",
                _ => $"<span class='badge bg-secondary'>{txPrioridade}</span>"
            };
        }

        /// <summary>
        /// Retorna badge HTML de acordo com o tipo de usuário
        /// </summary>
        public static string BadgeTipoUsuario(this string? snTipoUsuario)
        {
            if (string.IsNullOrWhiteSpace(snTipoUsuario))
                return "<span class='badge bg-secondary'>Indefinido</span>";

            return snTipoUsuario.ToUpper() switch
            {
                "A" => "<span class='badge bg-danger'>Administrador</span>",
                "U" => "<span class='badge bg-primary'>Atendente</span>",
                "C" => "<span class='badge bg-info'>Cliente</span>",
                _ => "<span class='badge bg-secondary'>Indefinido</span>"
            };
        }

        #endregion

        #region Ícones e Indicadores

        /// <summary>
        /// Retorna ícone de prioridade
        /// </summary>
        public static string IconePrioridade(this int cdPrioridade)
        {
            return cdPrioridade switch
            {
                1 => "<i class='fas fa-arrow-down text-secondary'></i>",
                2 => "<i class='fas fa-minus text-primary'></i>",
                3 => "<i class='fas fa-arrow-up text-warning'></i>",
                4 => "<i class='fas fa-exclamation-triangle text-danger'></i>",
                _ => "<i class='fas fa-question text-secondary'></i>"
            };
        }

        /// <summary>
        /// Retorna ícone de tipo de arquivo baseado na extensão
        /// </summary>
        public static string IconeArquivo(this string? nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "<i class='fas fa-file text-secondary'></i>";

            var extensao = Path.GetExtension(nomeArquivo).ToLower();

            return extensao switch
            {
                ".pdf" => "<i class='fas fa-file-pdf text-danger'></i>",
                ".doc" or ".docx" => "<i class='fas fa-file-word text-primary'></i>",
                ".xls" or ".xlsx" => "<i class='fas fa-file-excel text-success'></i>",
                ".ppt" or ".pptx" => "<i class='fas fa-file-powerpoint text-warning'></i>",
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "<i class='fas fa-file-image text-info'></i>",
                ".zip" or ".rar" or ".7z" => "<i class='fas fa-file-archive text-secondary'></i>",
                ".txt" => "<i class='fas fa-file-alt text-secondary'></i>",
                _ => "<i class='fas fa-file text-secondary'></i>"
            };
        }

        /// <summary>
        /// Retorna estrelas de avaliação
        /// </summary>
        public static string EstrelasAvaliacao(this int nota)
        {
            if (nota < 1 || nota > 5)
                return string.Empty;

            var estrelas = string.Empty;
            
            for (int i = 1; i <= 5; i++)
            {
                if (i <= nota)
                    estrelas += "<i class='fas fa-star text-warning'></i> ";
                else
                    estrelas += "<i class='far fa-star text-secondary'></i> ";
            }

            return estrelas.Trim();
        }

        #endregion

        #region Helpers de Tempo

        /// <summary>
        /// Retorna tempo decorrido de forma amigável
        /// </summary>
        public static string TempoDecorrido(this DateTime data)
        {
            var diferenca = DateTime.Now - data;

            if (diferenca.TotalMinutes < 1)
                return "agora mesmo";
            if (diferenca.TotalMinutes < 60)
                return $"há {(int)diferenca.TotalMinutes} min";
            if (diferenca.TotalHours < 24)
                return $"há {(int)diferenca.TotalHours}h";
            if (diferenca.TotalDays < 7)
                return $"há {(int)diferenca.TotalDays} dia(s)";
            if (diferenca.TotalDays < 30)
                return $"há {(int)(diferenca.TotalDays / 7)} semana(s)";
            if (diferenca.TotalDays < 365)
                return $"há {(int)(diferenca.TotalDays / 30)} mês(es)";
            
            return $"há {(int)(diferenca.TotalDays / 365)} ano(s)";
        }

        /// <summary>
        /// Formata horas para exibição amigável
        /// </summary>
        public static string FormatarHoras(this int horas)
        {
            if (horas < 1)
                return "menos de 1 hora";
            if (horas < 24)
                return $"{horas}h";
            
            int dias = horas / 24;
            int horasRestantes = horas % 24;

            if (horasRestantes == 0)
                return $"{dias} dia(s)";
            
            return $"{dias} dia(s) e {horasRestantes}h";
        }

        #endregion

        #region Helpers de Cor

        /// <summary>
        /// Retorna cor CSS baseada no status
        /// </summary>
        public static string CorStatus(this int cdStatus)
        {
            return cdStatus switch
            {
                1 => "secondary",
                2 => "info",
                3 => "primary",
                4 => "warning",
                5 => "success",
                6 => "dark",
                7 => "danger",
                _ => "secondary"
            };
        }

        /// <summary>
        /// Retorna cor CSS baseada na prioridade
        /// </summary>
        public static string CorPrioridade(this int cdPrioridade)
        {
            return cdPrioridade switch
            {
                1 => "secondary",
                2 => "primary",
                3 => "warning",
                4 => "danger",
                _ => "secondary"
            };
        }

        #endregion
    }
}
