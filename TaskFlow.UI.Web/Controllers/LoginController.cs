using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Model;
using TaskFlow.Repository;
using TaskFlow.UI.Web.Models;

namespace TaskFlow.UI.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly FuncionarioREP _repositorioFuncionario;
        private readonly EmpresaREP _repositorioEmpresa;

        public LoginController(FuncionarioREP repositorioFuncionario, EmpresaREP repositorioEmpresa)
        {
            _repositorioFuncionario = repositorioFuncionario;
            _repositorioEmpresa = repositorioEmpresa;
        }

        #region Index

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Login

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> EfetuarLogin(UsuarioViewMOD dadosTela)
        {
            FuncionarioMOD funcionario = _repositorioFuncionario.BuscarLogin(dadosTela.Login, dadosTela.Password);

            if (funcionario != null)
            {
                string nomeEmpresa = _repositorioEmpresa.BuscarEmpresaPorCodigo(funcionario.CdEmpresa)?.NmEmpresa ?? "";

                List<Claim> Claims = new()
                {
                    new Claim(ClaimTypes.Name, dadosTela.Login),
                    new Claim("NmFuncionario", funcionario.NmFuncionario),
                    new Claim("NrCpf", funcionario.NrCpf),
                    new Claim("CdFuncionario", funcionario.CdFuncionario.ToString()),
                    new Claim("CdEmpresa", funcionario.CdEmpresa.ToString()),
                    new Claim("NmEmpresa", nomeEmpresa),
                    new Claim("SnTipoUsuario", funcionario.SnTipoUsuario)
                };

                ClaimsIdentity claimsIdentity = new(Claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authProperties = new()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    IssuedUtc = DateTime.Now,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            TempData.Add("Modal-Erro", "Login ou Senha inválidos");
            return RedirectToAction("Index");
        }

        #endregion

        #region Logout

        public async Task<ActionResult> Sair(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = "/Login/Index" });

            return RedirectToAction("Index");
        }

        #endregion
    }
}
