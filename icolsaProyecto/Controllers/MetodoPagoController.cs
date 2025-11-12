using Microsoft.AspNetCore.Mvc;
using icolsaProyecto.Data;
using icolsaProyecto.Models;
using System.Linq;

namespace icolsaProyecto.Controllers
{
    public class MetodoPagoController : Controller
    {
        private readonly MyDbContext _context;

        public MetodoPagoController(MyDbContext context)
        {
            _context = context;
        }

        // 📋 LISTAR MÉTODOS
        public IActionResult Index()
        {
            var metodos = _context.MetodosPago.ToList();
            return View(metodos);
        }

        // 🧾 CREAR MÉTODO DE PAGO (Vista normal)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MetodoPago metodo)
        {
            if (ModelState.IsValid)
            {
                _context.MetodosPago.Add(metodo);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Método de pago agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(metodo);
        }

        // ⚡ CREACIÓN RÁPIDA (desde modal AJAX)
        [HttpPost]
        public IActionResult CrearRapido([FromBody] MetodoPago metodo)
        {
            if (string.IsNullOrWhiteSpace(metodo.NombreMetodo))
                return Json(new { success = false, message = "El nombre del método de pago es obligatorio." });

            if (_context.MetodosPago.Any(m => m.NombreMetodo == metodo.NombreMetodo))
                return Json(new { success = false, message = "Ya existe un método con ese nombre." });

            _context.MetodosPago.Add(metodo);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Método de pago registrado correctamente.",
                id = metodo.IDMetodoPago,
                nombre = metodo.NombreMetodo
            });
        }
    }
}
