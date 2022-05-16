using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportCar.BLL.Models;
using SportCar.DAL.Interface;
using SportCar.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SportCar.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public UsuariosController(IUsuarioRepositorio usuarioRepositorio, IWebHostEnvironment webHostEnvironment)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _webHostEnviroment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View( await _usuarioRepositorio.PegarTodos());
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model, IFormFile foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null)
                {
                    string diretorioPasta = Path.Combine(_webHostEnviroment.WebRootPath, "Imagens");
                    string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                    using (FileStream fileStream = new FileStream(Path.Combine(diretorioPasta, nomeFoto), FileMode.Create))
                    {
                        await foto.CopyToAsync(fileStream);
                        model.Foto = "~/imagens/" + nomeFoto;
                    }
                }
                else
                {
                    ViewData["foto"] = TempData["foto"] = "Escolha uma foto!";
                    return View(model);
                }
                Usuario usuario = new Usuario();
                IdentityResult usuarioCriado;
                var primeiroUsuario = _usuarioRepositorio.VerificarSeExisteRegistro();
                var pegarUsuarioCPF = _usuarioRepositorio.PegarUsuarioPeloCPF(model.CPF);
                if (primeiroUsuario == 0)
                {
                    usuario.UserName = model.Nome;
                    usuario.CPF = model.CPF;
                    usuario.PhoneNumber = model.Telefone;
                    usuario.Foto = model.Foto;
                    usuario.PrimeiroAcesso = false;
                    usuario.Status = StatusConta.Aprovado;
                    usuario.Email = model.Email;


                    usuarioCriado = await _usuarioRepositorio.CriarUsuario(usuario, model.Senha);

                    if (usuarioCriado.Succeeded)
                    {
                        await _usuarioRepositorio.IncluirUsuarioFuncao(usuario, "Administrador");
                        await _usuarioRepositorio.LogarUsuario(usuario, false);
                        return RedirectToAction("Index", "Usuarios");
                    }
                }
                else if (pegarUsuarioCPF == null)
                {
                    usuario.UserName = model.Nome;
                    usuario.CPF = model.CPF;
                    usuario.PhoneNumber = model.Telefone;
                    usuario.Foto = model.Foto;
                    usuario.PrimeiroAcesso = true;
                    usuario.Status = StatusConta.Analizando;
                    usuario.Email = model.Email;

                    usuarioCriado = await _usuarioRepositorio.CriarUsuario(usuario, model.Senha);
                    if (usuarioCriado.Succeeded)
                    {
                        return View("Analise", usuario.UserName);
                    }
                    else
                    {
                        foreach (IdentityError erro in usuarioCriado.Errors)
                        {
                            if (erro.Code == "DuplicateUserName")
                                ViewData["DuplicateUserName"] = TempData["DuplicateUserName"] = $"O nome de usuario {model.Nome}, já está em uso.";
                            else if (erro.Code == "DuplicateEmail")
                                ViewData["DuplicateEmail"] = TempData["DuplicateEmail"] = $"O E-mail: {model.Email}, já está em uso.";
                            else if (erro.Code == "PasswordRequiresNonAlphanumeric")
                                ViewData["PasswordRequiresNonAlphanumeric"] = TempData["PasswordRequiresNonAlphanumeric"] = $"As senhas devem ter pelo menos um caractere não alfanumérico. Ex: @*!.";
                            else if (erro.Code == "PasswordRequiresDigit")
                                ViewData["PasswordRequiresDigit"] = TempData["PasswordRequiresDigit"] = $"As senhas devem ter pelo menos um dígito ('0'-'9').";
                            else if (erro.Code == "PasswordRequiresUpper")
                                ViewData["PasswordRequiresUpper"] = TempData["PasswordRequiresUpper"] = $"As senhas devem ter pelo menos uma letra maiúscula ('A'-'Z').";

                        }
                        return View(model);
                    }
                }
                else
                {
                    ViewData["error"] = TempData["error"] = "CPF já cadastrado em nosso sistema!";
                    return View(model);
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloEmail(model.Email);
                if (usuario != null)
                {
                    if (usuario.Status == StatusConta.Analizando)
                    {
                        return View("Analise", usuario.UserName);
                    }
                    else if (usuario.Status == StatusConta.Reprovado)
                    {
                        return View("Reprovado", usuario.UserName);
                    }
                    else if (usuario.PrimeiroAcesso == true)
                    {
                        return View("RedefinirSenha", usuario);
                    }
                    else
                    {
                        PasswordHasher<Usuario> passwordHasher = new PasswordHasher<Usuario>();
                        if (passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, model.Senha) != PasswordVerificationResult.Failed)
                        {
                            await _usuarioRepositorio.LogarUsuario(usuario, false);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Senha invalida!");
                            return View(model);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Usuario invalido!");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _usuarioRepositorio.DeslogarUsuario();
            return RedirectToAction("Login");
        }
        public IActionResult Analise(string nome)
        {
            return View(nome);
        }
        public IActionResult Reprovado(string nome)
        {
            return View(nome);
        }
        public async Task<JsonResult> AprovarUsuario(string usuarioId)
        {
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);
            usuario.Status = StatusConta.Aprovado;
            await _usuarioRepositorio.IncluirUsuarioFuncao(usuario, "Morador");
            await _usuarioRepositorio.AtualizarUsuario(usuario);

            return Json(true);
        }
        public async Task<JsonResult> ReprovarUsuario(string usuarioId)
        {
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);
            usuario.Status = StatusConta.Reprovado;
            await _usuarioRepositorio.AtualizarUsuario(usuario);

            return Json(true);
        }
        public async Task<IActionResult> GerenciarUsuario(string usuarioId, string nome)
        {
            if (usuarioId == null)
                return NotFound();

            TempData["usuarioId"] = usuarioId;
            ViewBag.Nome = nome;
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);
            if (usuarioId == null)
                return NotFound();

            List<FuncaoUsuarioViewModel> viewModel = new List<FuncaoUsuarioViewModel>();
            return await Task.FromResult<IActionResult>(null);
        }
    }
}
