using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using icolsaProyecto.Data;
using icolsaProyecto.Models;
using Microsoft.AspNetCore.Http;

namespace icolsaProyecto.Controllers
{
    [Route("Producto")]
    public class ProductoController : Controller
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly MyDbContext _context;

        public ProductoController(ILogger<ProductoController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ===========================
        // LISTADO DE PRODUCTOS
        // ===========================
        [HttpGet("Index")]
        public async Task<IActionResult> IndexAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre_Producto)
                .ToListAsync();

            return View(productos);
        }

        // ===========================
        // CREAR PRODUCTO (GET)
        // ===========================
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias?.ToList() ?? new List<Categoria>();
            return View();
        }

        // ===========================
        // CREAR PRODUCTO (POST)
        // ===========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto, IFormFile? imagenFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _context.Categorias.ToList();
                return View(producto);
            }

            try
            {
                if (imagenFile != null)
                {
                    var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imagesPath))
                        Directory.CreateDirectory(imagesPath);

                    var fileName = Path.GetFileName(imagenFile.FileName);
                    var filePath = Path.Combine(imagesPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagenFile.CopyToAsync(stream);
                    }

                    producto.ImagenUrl = "/images/" + fileName;
                }
                

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                 // ✅ REGISTRAR EN HISTORIAL DE INVENTARIO (CREATE)
                if (producto.Stock_Producto > 0)
                {
                    var historial = new HistorialInventarioSaldo
                    {
                        IDProducto = producto.IDProducto,
                        TipoMovimiento = "Entrada (creación producto)",
                        Saldo_Actual = producto.Stock_Producto,
                        Fecha_Actualizacion = DateTime.Now
                    };
                    _context.HistorialInventarioSaldos.Add(historial);
                    await _context.SaveChangesAsync();
                }

                TempData["MensajeExito"] = "Producto creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                ModelState.AddModelError("", "Ocurrió un error al guardar el producto.");
                ViewBag.Categorias = _context.Categorias.ToList();
                return View(producto);
            }
        }

        // ===========================
        // CARGAR PRODUCTO PARA EDITAR (GET)
        // ===========================
        [HttpPost("LoadEdit")]
        public async Task<IActionResult> LoadEdit(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            ViewBag.Categorias = _context.Categorias.ToList();
            return View("Edit", producto);
        }

        // ===========================
        // EDITAR PRODUCTO (POST)
        // ===========================
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto producto, IFormFile? imagenFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _context.Categorias.ToList();
                return View(producto);
            }

            try
            {
                var existente = await _context.Productos.FindAsync(producto.IDProducto);
                if (existente == null)
                    return NotFound();

                
                
                // Actualiza los campos
                existente.Nombre_Producto = producto.Nombre_Producto;
                existente.Codigo_Producto = producto.Codigo_Producto;
                existente.PrecioUnitario_Producto = producto.PrecioUnitario_Producto;
                existente.Stock_Producto = producto.Stock_Producto;
                existente.IDCategoria = producto.IDCategoria;

                // Manejo de imagen
                if (imagenFile != null)
                {
                    var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imagesPath))
                        Directory.CreateDirectory(imagesPath);

                    var fileName = Path.GetFileName(imagenFile.FileName);
                    var filePath = Path.Combine(imagesPath, fileName);

                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(existente.ImagenUrl))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existente.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagenFile.CopyToAsync(stream);
                    }

                    existente.ImagenUrl = "/images/" + fileName;
                }

                _context.Update(existente);
                await _context.SaveChangesAsync();

                // ✅ REGISTRAR EN HISTORIAL DE INVENTARIO (EDIT) - SOLO NUEVO STOCK
                if (producto.Stock_Producto > 0)
                {
                    var historial = new HistorialInventarioSaldo
                    {
                        IDProducto = existente.IDProducto,
                        TipoMovimiento = "Entrada (edición producto)",
                        Saldo_Actual = existente.Stock_Producto,
                        Fecha_Actualizacion = DateTime.Now
                    };
                    _context.HistorialInventarioSaldos.Add(historial);
                    await _context.SaveChangesAsync();
                }

                TempData["MensajeExito"] = "Producto actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                ModelState.AddModelError("", "Ocurrió un error al actualizar el producto.");
                ViewBag.Categorias = _context.Categorias.ToList();
                return View(producto);
            }
        }

        // ===========================
        // ELIMINAR PRODUCTO (POST)
        // ===========================
        [HttpPost("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                    return NotFound();

                // Eliminar imagen asociada
                if (!string.IsNullOrEmpty(producto.ImagenUrl))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", producto.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Producto eliminado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                TempData["MensajeError"] = "Ocurrió un error al eliminar el producto.";
                return RedirectToAction("Index");
            }
        }
    }
}
