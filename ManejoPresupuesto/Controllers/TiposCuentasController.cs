using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly string connectionString;
        private readonly IUsuarioRepository _usuarioRepository;

        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        public TiposCuentasController(IConfiguration configuration, IRepositorioTiposCuentas repositorioTiposCuentas,
            IUsuarioRepository usuarioRepository)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            _usuarioRepository = usuarioRepository;
            _repositorioTiposCuentas = repositorioTiposCuentas;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();

            var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);

            return View(tiposCuentas);
        }


        public IActionResult Crear()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = connection.Query("Select 1").FirstOrDefault();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();
            var tipoCuentaExiste = await _repositorioTiposCuentas.Existe(tipoCuenta.Nombre, usuarioId);

            if (tipoCuentaExiste)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTiposCuentas.Actualizar(tipoCuenta);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = _usuarioRepository.ObtenerUsuarioId();

            var existeTipoCuenta = await _repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (existeTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {nameof(tipoCuenta.Nombre)} ya existe.");

                return View(tipoCuenta);
            }


            await _repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();

            var existeTipoCuenta = await _repositorioTiposCuentas.Existe(nombre, usuarioId);

            if (existeTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe.");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();

            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();

            var tipoCuenta = _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTiposCuentas.Borrar(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _usuarioRepository.ObtenerUsuarioId();
            var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);

            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenUsuario.Count > 0)
                return Forbid();


            var tiposCuentasOrdenados = ids.Select((valor, indice)
                => new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await _repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }

    }
}
