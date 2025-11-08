using Microsoft.AspNetCore.Mvc;
using PedidosYa.Web.Controllers;
using TpSignalR.Repositorio;
using TpSignalR.Web.Models;
using System.Linq;
using System.Collections.Generic;

namespace TpSignalR.Web.Controllers
{
    public class ComercioController : Controller
    {
        private readonly ServicioRepartoContext _context;

        public ComercioController(ServicioRepartoContext context)
        {
            _context = context;
        }

        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult DetallePedido()
        {
            // Redirige a la acción "PruebaIngresoDeDatosDelivery" del controlador "SeguimientoController"
            // Nota: el nombre del controlador se pasa sin el sufijo "Controller"
            return RedirectToAction("PruebaIngresoDeDatosDelivery", "Seguimiento");
        }

        public IActionResult ComercioLobby()
        {
            int comercioId = 1;
            var pedidos = _context.Pedido
                .Where(p => p.ComercioId == comercioId)
                .Select(p => new ComercioPedidoViewModel
                {
                    PedidoId = p.PedidoId,
                    Total = p.Total,
                    Estado = p.Estado,
                    UsuarioFinalId = p.UsuarioFinalId,
                    ProductoId = p.ProductoId,
                    ProductoNombre = _context.Producto.Where(prod => prod.Id == p.ProductoId).Select(prod => prod.Nombre).FirstOrDefault(),
                    ProductoCategoria = _context.Producto.Where(prod => prod.Id == p.ProductoId).Select(prod => prod.Categoria).FirstOrDefault()
                })
                .ToList();

            ViewBag.Pedidos = pedidos;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEstadoPedido([FromBody] EstadoUpdateModel model)
        {
            if (model == null) return BadRequest();

            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == model.PedidoId);
            if (pedido == null) return NotFound();

            pedido.Estado = model.NuevoEstado;
            _context.SaveChanges();

            return Ok();
        }

    }
}
