using Microsoft.AspNetCore.Mvc;
using icolsaProyecto.Data;
using icolsaProyecto.Models;
using System.Linq;

namespace icolsaProyecto.Controllers
{
    public class EstadoPagoController : Controller
    {
        private readonly MyDbContext _context;

        public EstadoPagoController(MyDbContext context)
        {
            _context = context;
        }

        // 📋 LISTAR ESTADOS
        public IActionResult Index()
        {
            var estados = _context.EstadosPago.ToList();
            return View(estados);
        }

        // 🧾 CREAR ESTADO (Vista normal)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EstadoPago estado)
        {
            if (ModelState.IsValid)
            {
                _context.EstadosPago.Add(estado);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Estado de pago agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // ⚡ CREACIÓN RÁPIDA (desde modal AJAX)
        [HttpPost]
        public IActionResult CrearRapido([FromBody] EstadoPago estado)
        {
            if (string.IsNullOrWhiteSpace(estado.NombreEstado))
                return Json(new { success = false, message = "El nombre del estado es obligatorio." });

            if (_context.EstadosPago.Any(e => e.NombreEstado == estado.NombreEstado))
                return Json(new { success = false, message = "Ya existe un estado con ese nombre." });

            _context.EstadosPago.Add(estado);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Estado de pago registrado correctamente.",
                id = estado.IDEstadoPago,
                nombre = estado.NombreEstado
            });
        }
    }
}
