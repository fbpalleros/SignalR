namespace TpSignalR.Web.Models
{
    public class FinalizarCompraViewModel
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string UsuarioNombre { get; set; }
        public ProductoViewModel Producto { get; set; }
        public string ComercioNombre { get; set; }
        public decimal? ComercioLat { get; set; }
        public decimal? ComercioLng { get; set; }

        public class ProductoViewModel
        {
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public decimal? Precio { get; set; }
            public string ImagenUrl { get; set; }
        }
    }
}
