using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using icolsaProyecto.Data;
using icolsaProyecto.Models;

namespace icolsaProyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ===============================
        // INDEX - CATÁLOGO DE PRODUCTOS
        // ===============================
        public async Task<IActionResult> Index(string? search)
        {
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                productos = productos.Where(p =>
                    p.Nombre_Producto!.Contains(search) ||
                    p.Codigo_Producto!.Contains(search));
            }

            var lista = await productos.OrderBy(p => p.Nombre_Producto).ToListAsync();
            return View(lista);
        }




        public IActionResult IndexAdministrativo()
        {
            
            return View();
        }


        // ===============================
        // PRIVACIDAD
        // ===============================
        public IActionResult Privacy()
        {
            return View();
        }

          // ===============================
        // ACERCA DE (NUEVA SECCIÓN)
        // ===============================
        public IActionResult Acerca()
        {
            return View();
        }
        // ===============================
        // ERROR
        // ===============================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
