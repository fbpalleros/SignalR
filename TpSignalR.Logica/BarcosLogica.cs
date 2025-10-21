using TpSignalR.Entidades;
using TpSignalR.Repositorio;


namespace TpSignalR.Logica;
public interface IRegistroLogica
{
    void registrarBarco(Barco barco);

    List<Barco> ObtenerTodos();

}
public class RegistroLogica : IRegistroLogica
{

    private RegistroContext _context;
    public RegistroLogica(RegistroContext context)
    {
        _context = context;
    }

    private static List<Barco> _barcos = new List<Barco>()
        {
            new Barco() { Id= 1 , Nombre = "Niña", Antiguedad = 5, TripulacionMaxima = 10 },
            new Barco() { Id= 2, Nombre = "Pinta", Antiguedad = 10, TripulacionMaxima = 20 },
            new Barco() { Id= 3, Nombre = "Santa Maria", Antiguedad = 15, TripulacionMaxima = 30 }
        };


    public void registrarBarco(Barco barco)
    {


        barco.Impuesto = (barco.Antiguedad * 1000) + (barco.TripulacionMaxima * 500);
        //barco.Id = _barcos.Max(b => b.Id) + 1;
        _context.Barcos.Add(barco);
        _context.SaveChanges();
        _barcos.Add(barco);
    }

    public List<Barco> ObtenerTodos()
    {
        return _barcos = _barcos.OrderBy(b => b.Id).ToList();
    }

}

