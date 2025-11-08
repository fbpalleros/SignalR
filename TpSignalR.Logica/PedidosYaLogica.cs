using System;
using System.Collections.Generic;
using System.Linq;
using TpSignalR.Entidades;
using TpSignalR.Logica;
using TpSignalR.Repositorio;

namespace TpSignalR.Logica
{
    public interface IPedidosYaLogica
    {
        // Mostrar los productos disponibles en un comercio
        List<Producto> ObtenerProductosPorComercio(int idComercio);

        // Realizar un pedido
        Pedido CrearPedido(int idCliente, int idComerciante, int idProducto);

        // Consultar el estado o seguimiento de un pedido
        Pedido ObtenerSeguimientoPedido(int idPedido);

        // Cambiar el estado del pedido (por ejemplo, cuando el comercio lo prepara o lo entrega)
        void ActualizarEstadoPedido(int idPedido, String nuevoEstado);

        // Listar pedidos de un cliente
        List<Pedido> ObtenerPedidosPorCliente(int idCliente);

        // Consultar información de un comercio
        Comercio ObtenerComercioPorId(int idComercio);

        // Listar todos los comercios (restaurantes)
        List<Comercio> ObtenerComercios();
    }


    public class PedidosYaLogica : IPedidosYaLogica
    {

        public ServicioRepartoContext _context;

        public PedidosYaLogica(ServicioRepartoContext context)
        {
            _context = context;
        }
        public void ActualizarEstadoPedido(int idPedido, string nuevoEstado)
        {
            throw new NotImplementedException();
        }

        public Pedido CrearPedido(int idCliente, int idComerciante, int idProducto)
        {
            // Intentar asignar un repartidor disponible para evitar conflicto FK
            var repartidor = _context.Repartidor.FirstOrDefault(r => r.Estado == "Disponible");
            if (repartidor == null)
            {
                // Si no hay repartidor disponible, crear uno placeholder
                repartidor = new Repartidor
                {
                    Nombre = "AutoAsignado",
                    Estado = "Disponible"
                };
                _context.Repartidor.Add(repartidor);
                _context.SaveChanges();
            }

            // Calcular total a partir del producto si está disponible
            decimal total = 0m;
            var producto = _context.Producto.Find(idProducto);
            if (producto != null)
            {
                total = producto.Precio;
            }

            Pedido pedido = new Pedido
            {
                UsuarioFinalId = idCliente,
                ComercioId = idComerciante,
                RepartidorId = repartidor.RepartidorId,
                ProductoId = idProducto,
                Total = total,
                Estado = "pendiente",
            };

            _context.Pedido.Add(pedido);
            _context.SaveChanges();
            return pedido;
        }

        public Comercio ObtenerComercioPorId(int idComercio)
        {
            throw new NotImplementedException();
        }

        public List<Pedido> ObtenerPedidosPorCliente(int idCliente)
        {
            throw new NotImplementedException();
        }

        public List<Producto> ObtenerProductosPorComercio(int idComercio)
        {
            return _context.Producto.ToList();
        }

        public Pedido ObtenerSeguimientoPedido(int idPedido)
        {
            throw new NotImplementedException();
        }

        public List<Comercio> ObtenerComercios()
        {
            return _context.Comercio.ToList();
        }

    }
}
