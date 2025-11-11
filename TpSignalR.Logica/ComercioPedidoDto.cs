namespace TpSignalR.Logica
{
    public class ComercioPedidoDto
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public int UsuarioFinalId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCategoria { get; set; }
        public int ComercioId { get; set; }
        public string ComercioNombre { get; set; }
        public decimal? ComercioLat { get; set; }
        public decimal? ComercioLng { get; set; }
    }
}
