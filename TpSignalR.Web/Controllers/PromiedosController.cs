using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TpSignalR.Entidades;
using TpSignalR.Logica;
using TpSignalR.Web.Hubs;

namespace TpSignalR.Web.Controllers
{
    public class PromiedosController: Controller
    {
        private readonly IPartidoLogica partidoLogica;
        private readonly IHubContext<PromiedosHub> _hubContext;

        public PromiedosController(IPartidoLogica partidoLogica, IHubContext<PromiedosHub> hubContext)
        {
            this.partidoLogica = partidoLogica;
            this._hubContext = hubContext;
        }

        public IActionResult Promiedos()
        {
            return View();
        }
        public IActionResult PromiedosLobby()
        {
            return View();
        }

        public IActionResult PromiedosUser()
        {
            var partidos = partidoLogica.ObtenerTodosLosPartidos();
            return View(partidos);
        }

        public IActionResult PromiedosAdmin()
        {
            var partidos = partidoLogica.ObtenerTodosLosPartidos();
            return View(partidos);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarTarjeta([FromBody] ActualizarTarjetaRequest request)
        {
            try
            {
                var partido = partidoLogica.ObtenerPartidoPorId(request.PartidoId);
                if (partido == null)
                    return NotFound();

                int nuevoValor = 0;
                int amarillas = 0;
                int rojas = 0;

                if (request.Equipo.ToLower() == "local")
                {
                    if (request.TipoTarjeta == "amarilla")
                    {
                        partido.AmarillasEquipoLocal = Math.Max(0, partido.AmarillasEquipoLocal + request.Cambio);
                        nuevoValor = partido.AmarillasEquipoLocal;
                    }
                    else if (request.TipoTarjeta == "roja")
                    {
                        partido.RojasEquipoLocal = Math.Max(0, partido.RojasEquipoLocal + request.Cambio);
                        nuevoValor = partido.RojasEquipoLocal;
                    }
                    else
                    {
                        return BadRequest("Invalid card type");
                    }
                    amarillas = partido.AmarillasEquipoLocal;
                    rojas = partido.RojasEquipoLocal;
                }
                else if (request.Equipo.ToLower() == "visitante")
                {
                    if (request.TipoTarjeta == "amarilla")
                    {
                        partido.AmarillasEquipoVisitante = Math.Max(0, partido.AmarillasEquipoVisitante + request.Cambio);
                        nuevoValor = partido.AmarillasEquipoVisitante;
                    }
                    else if (request.TipoTarjeta == "roja")
                    {
                        partido.RojasEquipoVisitante = Math.Max(0, partido.RojasEquipoVisitante + request.Cambio);
                        nuevoValor = partido.RojasEquipoVisitante;
                    }
                    else
                    {
                        return BadRequest("Invalid card type");
                    }
                    amarillas = partido.AmarillasEquipoVisitante;
                    rojas = partido.RojasEquipoVisitante;
                }
                else
                {
                    return BadRequest("Invalid team");
                }

                // Save changes to database
                partidoLogica.ActualizarPartido(partido);

                // Notify all clients via SignalR
                await _hubContext.Clients.All.SendAsync("TarjetasActualizadas", request.PartidoId, request.Equipo, amarillas, rojas);

                return Ok(new { nuevoValor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ObtenerGolesPartido(int partidoId)
        {
            try
            {
                var goles = partidoLogica.ObtenerGolesPorPartido(partidoId);
                var golesDto = goles.Select(g => new
                {
                    minuto = g.Minuto,
                    jugador = g.Jugador,
                    equipo = g.Equipo
                });
                return Ok(golesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarGol([FromBody] RegistrarGolRequest request)
        {
            try
            {
                var partido = partidoLogica.ObtenerPartidoPorId(request.PartidoId);
                if (partido == null)
                    return NotFound();

                var gol = new Gol
                {
                    PartidoId = request.PartidoId,
                    Jugador = request.Jugador,
                    Minuto = request.Minuto,
                    Equipo = request.Equipo.ToLower(),
                    FechaGol = DateTime.Now
                };

                // Save goal to database
                partidoLogica.RegistrarGol(gol);

                // Reload partido to get updated goal count
                partido = partidoLogica.ObtenerPartidoPorId(request.PartidoId);
                var golesLocal = partido.GolesLocal;
                var golesVisitante = partido.GolesVisitante;

                // Notify all clients via SignalR
                await _hubContext.Clients.All.SendAsync("GolRegistrado", 
                    request.PartidoId, 
                    request.Equipo.ToLower(), 
                    request.Jugador, 
                    request.Minuto,
                    golesLocal, 
                    golesVisitante);

                return Ok(new { golesLocal, golesVisitante });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public IActionResult DetallePartido()
        {
            return View();
        }

        public IActionResult Categoria()
        {
            return View();
        }

        public IActionResult DetalleEquipo()
        {
            return View();
        }
    }

    // Request models
    public class ActualizarTarjetaRequest
    {
        public int PartidoId { get; set; }
        public string Equipo { get; set; }
        public string TipoTarjeta { get; set; }
        public int Cambio { get; set; }
    }

    public class RegistrarGolRequest
    {
        public int PartidoId { get; set; }
        public string Equipo { get; set; }
        public string Jugador { get; set; }
        public int Minuto { get; set; }
    }
}
