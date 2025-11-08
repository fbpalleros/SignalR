using System;

namespace TpSignalR.Web.Models
{
    public class ComercioPedidoViewModel
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public int UsuarioFinalId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCategoria { get; set; }
    }
}
