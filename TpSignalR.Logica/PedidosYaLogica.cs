using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TpSignalR.Entidades;
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
        Task<ComercioPedidoDto> ActualizarEstadoPedidoAsync(int idPedido, String nuevoEstado);

        // Listar pedidos de un cliente
        List<Pedido> ObtenerPedidosPorCliente(int idCliente);

        // Consultar información de un comercio
        Comercio ObtenerComercioPorId(int idComercio);

        // Listar todos los comercios (restaurantes)
        List<Comercio> ObtenerComercios();

        // Obtener pedidos de un comercio con datos de producto (DTO)
        List<ComercioPedidoDto> ObtenerPedidosPorComercio(int comercioId);
    }


    public class PedidosYaLogica : IPedidosYaLogica
    {

        public ServicioRepartoContext _context;

        public PedidosYaLogica(ServicioRepartoContext context)
        {
            _context = context;
        }
        public async Task<ComercioPedidoDto> ActualizarEstadoPedidoAsync(int idPedido, string nuevoEstado)
        {
            var pedido = _context.Pedido.FirstOrDefault(p => p.PedidoId == idPedido);
            if (pedido == null) throw new ArgumentException("Pedido not found", nameof(idPedido));

            pedido.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            // Obtener nombre de producto y comercio
            var producto = _context.Producto.FirstOrDefault(prod => prod.Id == pedido.ProductoId);
            var comercio = _context.Comercio.FirstOrDefault(c => c.Id == pedido.ComercioId);

            var dto = new ComercioPedidoDto
            {
                PedidoId = pedido.PedidoId,
                Total = pedido.Total,
                Estado = pedido.Estado,
                UsuarioFinalId = pedido.UsuarioFinalId,
                ProductoId = pedido.ProductoId,
                ProductoNombre = producto?.Nombre,
                ProductoCategoria = producto?.Categoria,
                ComercioId = pedido.ComercioId,
                ComercioNombre = comercio?.Nombre,
                ComercioLat = comercio?.Latitud,
                ComercioLng = comercio?.Longitud
            };

            return dto;
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
            return _context.Producto.Where(p => p.ComercioId == idComercio).ToList();
        }

        public Pedido ObtenerSeguimientoPedido(int idPedido)
        {
            throw new NotImplementedException();
        }

        public List<Comercio> ObtenerComercios()
        {
            return _context.Comercio.ToList();
        }

        public List<ComercioPedidoDto> ObtenerPedidosPorComercio(int comercioId)
        {
            // Use a left join to include product data in one SQL query
            var query = from p in _context.Pedido
                        join prod in _context.Producto on p.ProductoId equals prod.Id into prodg
                        from prod in prodg.DefaultIfEmpty()
                        join com in _context.Comercio on p.ComercioId equals com.Id into comg
                        from com in comg.DefaultIfEmpty()
                        where p.ComercioId == comercioId
                        orderby p.PedidoId
                        select new ComercioPedidoDto
                        {
                            PedidoId = p.PedidoId,
                            Total = p.Total,
                            Estado = p.Estado,
                            UsuarioFinalId = p.UsuarioFinalId,
                            ProductoId = p.ProductoId,
                            ProductoNombre = prod != null ? prod.Nombre : null,
                            ProductoCategoria = prod != null ? prod.Categoria : null,
                            ComercioId = p.ComercioId,
                            ComercioNombre = com != null ? com.Nombre : null,
                            ComercioLat = com != null ? (decimal?)com.Latitud : null,
                            ComercioLng = com != null ? (decimal?)com.Longitud : null
                        };

            return query.ToList();
        }

        // Obtener un pedido por id
        public ComercioPedidoDto ObtenerPedidoPorId(int pedidoId)
        {
            var query = from p in _context.Pedido
                        join prod in _context.Producto on p.ProductoId equals prod.Id into prodg
                        from prod in prodg.DefaultIfEmpty()
                        join com in _context.Comercio on p.ComercioId equals com.Id into comg
                        from com in comg.DefaultIfEmpty()
                        where p.PedidoId == pedidoId
                        select new ComercioPedidoDto
                        {
                            PedidoId = p.PedidoId,
                            Total = p.Total,
                            Estado = p.Estado,
                            UsuarioFinalId = p.UsuarioFinalId,
                            ProductoId = p.ProductoId,
                            ProductoNombre = prod != null ? prod.Nombre : null,
                            ProductoCategoria = prod != null ? prod.Categoria : null,
                            ComercioId = p.ComercioId,
                            ComercioNombre = com != null ? com.Nombre : null,
                            ComercioLat = com != null ? (decimal?)com.Latitud : null,
                            ComercioLng = com != null ? (decimal?)com.Longitud : null
                        };

            return query.FirstOrDefault();
        }

    }
}
