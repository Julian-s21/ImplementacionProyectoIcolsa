using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using icolsaProyecto.Data;
using icolsaProyecto.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Font.Constants;
using iText.IO.Font;


namespace icolsaProyecto.Controllers
{
    [Route("HistorialInventarioSaldo")]
    public class HistorialInventarioSaldoController : Controller
    {
        private readonly ILogger<HistorialInventarioSaldoController> _logger;
        private readonly MyDbContext _context;

        public HistorialInventarioSaldoController(ILogger<HistorialInventarioSaldoController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // 🔹 Mostrar lista completa
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            var historial = await _context.HistorialInventarioSaldos
                .Include(h => h.Producto)
                .OrderByDescending(h => h.Fecha_Actualizacion)
                .ToListAsync();

            return View(historial);
        }

        // 🔹 Formulario para crear un nuevo registro
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Productos = _context.Productos.ToList();
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HistorialInventarioSaldo historial)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Productos = _context.Productos.ToList();
                return View(historial);
            }

            historial.Fecha_Actualizacion = DateTime.Now;

            _context.HistorialInventarioSaldos.Add(historial);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Movimiento de inventario registrado correctamente.";
            return RedirectToAction("Index");
        }

        // 🔹 Cargar datos para editar
        [HttpPost("LoadEdit")]
        [ValidateAntiForgeryToken]
        public IActionResult LoadEdit(int id)
        {
            var historial = _context.HistorialInventarioSaldos
                .Include(h => h.Producto)
                .FirstOrDefault(h => h.IDInventarioSaldo == id);

            if (historial == null)
                return NotFound();

            ViewBag.Productos = _context.Productos.ToList();
            return View("Edit", historial);
        }

        // 🔹 Editar registro (POST)
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HistorialInventarioSaldo historial)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Productos = _context.Productos.ToList();
                return View(historial);
            }

            var existente = _context.HistorialInventarioSaldos
                .FirstOrDefault(h => h.IDInventarioSaldo == historial.IDInventarioSaldo);

            if (existente == null)
                return NotFound();

            existente.TipoMovimiento = historial.TipoMovimiento;
            existente.Saldo_Actual = historial.Saldo_Actual;
            existente.Fecha_Actualizacion = DateTime.Now;
            existente.IDProducto = historial.IDProducto;

            _context.Update(existente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Historial actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // 🔹 Eliminar registro
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var historial = _context.HistorialInventarioSaldos
                .FirstOrDefault(h => h.IDInventarioSaldo == id);

            if (historial == null)
                return NotFound();

            _context.HistorialInventarioSaldos.Remove(historial);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registro eliminado correctamente.";
            return RedirectToAction("Index");
        }




        
        [HttpGet("GenerarPDF")]
        public async Task<IActionResult> GenerarPDF(string tipoReporte, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // 🔍 Obtener datos según el tipo de reporte
            var historial = _context.HistorialInventarioSaldos
                .Select(h => new
                {
                    h.IDInventarioSaldo,
                    Producto = h.Producto.Nombre_Producto,
                    h.TipoMovimiento,
                    h.Saldo_Actual,
                    h.Fecha_Actualizacion
                })
                .AsQueryable();

            if (tipoReporte == "fechas" && fechaInicio.HasValue && fechaFin.HasValue)
            {
                historial = historial.Where(h =>
                    h.Fecha_Actualizacion >= fechaInicio &&
                    h.Fecha_Actualizacion <= fechaFin);
            }

            var lista = historial.ToList();

            // 📄 Crear PDF en memoria
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // 🔤 Fuentes
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // 🧾 Encabezado
                document.Add(new Paragraph("Reporte de Historial de Inventario")
                    .SetFont(boldFont)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16));

                document.Add(new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetFont(regularFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginBottom(10));

                if (tipoReporte == "fechas" && fechaInicio.HasValue && fechaFin.HasValue)
                {
                    document.Add(new Paragraph($"Rango de fechas: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
                        .SetFont(regularFont)
                        .SetFontSize(11)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(15));
                }

                // 📋 Tabla principal
                var table = new Table(5).UseAllAvailableWidth();
                Color headerColor = new DeviceRgb(106, 17, 203);

                string[] headers = { "ID", "Producto", "Tipo Movimiento", "Saldo Actual", "Fecha Actualización" };
                foreach (var header in headers)
                {
                    table.AddHeaderCell(new Cell()
                        .Add(new Paragraph(header)
                            .SetFont(boldFont)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetFontSize(10))
                        .SetBackgroundColor(headerColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(5));
                }

                foreach (var item in lista)
                {
                    table.AddCell(new Paragraph(item.IDInventarioSaldo.ToString())
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont));

                    table.AddCell(new Paragraph(item.Producto ?? "N/A")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont));

                    table.AddCell(new Paragraph(item.TipoMovimiento ?? "N/A")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont));

                    table.AddCell(new Paragraph(item.Saldo_Actual?.ToString() ?? "0")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont));

                    table.AddCell(new Paragraph(item.Fecha_Actualizacion?.ToString("dd/MM/yyyy HH:mm") ?? "N/A")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont));
                }

                document.Add(table);

                // 📈 Total de registros
                document.Add(new Paragraph($"\nTotal de registros: {lista.Count}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(11)
                    .SetFont(boldFont));

                // 📌 Pie de página opcional
                document.Add(new Paragraph("\nReporte generado por Icolsa S.A.")
                    .SetFont(regularFont)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.GRAY));

                document.Close();

                // 🧾 Enviar archivo al navegador
                var bytes = stream.ToArray();
                string nombreArchivo = tipoReporte == "fechas"
                    ? $"HistorialInventario_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf"
                    : $"HistorialInventarioGeneral_{DateTime.Now:yyyyMMddHHmm}.pdf";

                return File(bytes, "application/pdf", nombreArchivo);
            }
        }



        // 🔹 Manejo de errores
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
