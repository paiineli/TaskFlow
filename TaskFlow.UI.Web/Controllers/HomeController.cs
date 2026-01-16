using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskFlow.Repository;
using TaskFlow.UI.Web.Models;

namespace TaskFlow.UI.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ChamadoREP _repositorioChamado;

        public HomeController(ChamadoREP repositorioChamado)
        {
            _repositorioChamado = repositorioChamado;
        }

        #region Index - Dashboard

        public IActionResult Index()
        {
            string nmFuncionario = User.FindFirst("NmFuncionario")?.Value ?? "";
            string nmEmpresa = User.FindFirst("NmEmpresa")?.Value ?? "";
            string tipoUsuario = User.FindFirst("SnTipoUsuario")?.Value ?? "C";

            ViewBag.NmFuncionario = nmFuncionario;
            ViewBag.NmEmpresa = nmEmpresa;
            ViewBag.TipoUsuario = tipoUsuario;

            return View();
        }

        #endregion

        #region Erro

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
