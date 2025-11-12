using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using icolsaProyecto.Data;
using icolsaProyecto.Models;

namespace icolsaProyecto.Controllers
{
    [Route("Cliente")]
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly MyDbContext _context;

        public ClienteController(ILogger<ClienteController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ===== LISTAR CLIENTES =====
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Pedidos)
                .Include(c => c.Pagos)
                .ToListAsync();

            return View(clientes);
        }

        // ===== CREAR CLIENTE =====
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cliente creado correctamente.";
            return RedirectToAction("Index");
        }

        // ===== CARGAR CLIENTE PARA EDITAR =====
        [HttpPost("LoadEdit")]
        [ValidateAntiForgeryToken]
        public IActionResult LoadEdit(int id)
        {
            var cliente = _context.Clientes
                .FirstOrDefault(c => c.IDCliente == id);

            if (cliente == null)
                return NotFound();

            return View("Edit", cliente);
        }

        // ===== EDITAR CLIENTE =====
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            var existente = _context.Clientes
                .FirstOrDefault(c => c.IDCliente == cliente.IDCliente);

            if (existente == null)
                return NotFound();

            existente.Nombre_Cliente = cliente.Nombre_Cliente;
            existente.Apellido_Cliente = cliente.Apellido_Cliente;
            existente.Direccion_Cliente = cliente.Direccion_Cliente;
            existente.Telefono_Cliente = cliente.Telefono_Cliente;
            existente.Correo_Cliente = cliente.Correo_Cliente;
            existente.NIT_Cliente = cliente.NIT_Cliente;

            _context.Update(existente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cliente actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // ===== ELIMINAR CLIENTE =====
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.IDCliente == id);

            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
            return RedirectToAction("Index");
        }

        // ===== ERROR =====
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
