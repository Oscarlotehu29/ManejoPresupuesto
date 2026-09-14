using ManejoPresupuesto.Models;
using ManejoPresupuesto.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IUsuarioRepository _repositorioUsuarios;
        private readonly IRepositoryCuentas _repositoryCuentas;

        public CuentasController(
            IRepositorioTiposCuentas repositorioTiposCuentas, 
            IUsuarioRepository repositorioUsuarios,
            IRepositoryCuentas repositorioCuentas)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _repositorioUsuarios = repositorioUsuarios;
            _repositoryCuentas = repositorioCuentas;
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _repositorioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);

            var modelo = new CuentaCreacionViewModel();

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);


            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = _repositorioUsuarios.ObtenerUsuarioId();

            var tipoCuenta = _repositorioTiposCuentas.ObtenerPorId(cuenta.Id, usuarioId);

            if (tipoCuenta is null)
                return RedirectToAction("NoEncontrado", "Home");


            if(!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await _repositoryCuentas.Crear(cuenta);

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId )
        {
            var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);


            return tiposCuentas
                .Select(x =>
                    new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
