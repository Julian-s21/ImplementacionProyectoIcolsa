using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using icolsaProyecto.Data;
using icolsaProyecto.Models;
using Microsoft.AspNetCore.Http;

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
    public class ReporteController : Controller
    {
        private readonly ILogger<ReporteController> _logger;
        private readonly MyDbContext _context;

        public ReporteController(ILogger<ReporteController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Tipos = new[] {
                "VentasPorPeriodo",
                "DetallePedidos",
                "PagosPendientes",
                "TopClientes",
                "ProductosBajoStock",
                "InventarioValorado"
            };
            ViewBag.Usuarios = _context.Usuarios.ToList();
            ViewBag.MetodosPago = _context.MetodosPago.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(string tipo, DateTime? desde, DateTime? hasta, int? usuarioId, int? topN, int? umbralStock, string export = null)
        {
            desde ??= DateTime.Today.AddMonths(-1);
            hasta ??= DateTime.Today.AddDays(1).AddSeconds(-1);

            object result = null;

            // ======= REPORTES =======
            if (tipo == "VentasPorPeriodo")
            {
                result = await _context.Pedidos
                    .Where(p => p.Fecha_Pedido >= desde && p.Fecha_Pedido <= hasta)
                    .GroupBy(p => new { Year = p.Fecha_Pedido.Value.Year, Month = p.Fecha_Pedido.Value.Month })
                    .Select(g => new {
                        Periodo = $"{g.Key.Month}/{g.Key.Year}",
                        TotalPedidos = g.Count(),
                        TotalVenta = g.Sum(x => x.TotalPago_Pedido)
                    }).ToListAsync();
            }
            else if (tipo == "DetallePedidos")
            {
                result = await _context.Pedidos
                    .Where(p => p.Fecha_Pedido >= desde && p.Fecha_Pedido <= hasta)
                    .Include(p => p.Cliente)
                    .Include(p => p.Usuario)
                    .Select(p => new {
                        p.IDPedido,
                        Fecha = p.Fecha_Pedido,
                        Cliente = p.Cliente != null ? (p.Cliente.Nombre_Cliente + " " + p.Cliente.Apellido_Cliente) : "",
                        Vendedor = p.Usuario != null ? p.Usuario.Nombre_Usuario : "",
                        Total = p.TotalPago_Pedido,
                        CantidadProductos = p.Cantidad_Pedido
                    }).ToListAsync();
            }
            else if (tipo == "PagosPendientes")
            {
                result = await _context.Pagos
                    .Where(p => p.IDEstadoPago != null && p.IDEstadoPago != 1)
                    .Include(p => p.Pedido).ThenInclude(x => x.Cliente)
                    .Include(p => p.MetodoPago)
                    .Select(p => new {
                        p.IDPago,
                        Pedido = p.IDPedido,
                        Cliente = p.Pedido != null ? (p.Pedido.Cliente != null ? (p.Pedido.Cliente.Nombre_Cliente + " " + p.Pedido.Cliente.Apellido_Cliente) : "") : "",
                        p.Monto_Pago,
                        FechaPago = p.Fecha_Pago,
                        Metodo = p.MetodoPago != null ? p.MetodoPago.NombreMetodo : "",
                        EstadoId = p.IDEstadoPago
                    }).ToListAsync();
            }
            else if (tipo == "TopClientes")
            {
                result = await _context.Pedidos
                    .Where(p => p.Fecha_Pedido >= desde && p.Fecha_Pedido <= hasta)
                    .Include(p => p.Cliente)
                    .GroupBy(p => new { p.IDCliente, p.Cliente!.Nombre_Cliente, p.Cliente!.Apellido_Cliente })
                    .Select(g => new {
                        Cliente = g.Key.Nombre_Cliente + " " + g.Key.Apellido_Cliente,
                        TotalComprado = g.Sum(x => x.TotalPago_Pedido),
                        Pedidos = g.Count()
                    })
                    .OrderByDescending(x => x.TotalComprado)
                    .Take(topN ?? 10)
                    .ToListAsync();
            }
            else if (tipo == "ProductosBajoStock")
            {
                result = await _context.Productos
                    .Where(p => p.Stock_Producto <= (umbralStock ?? 5))
                    .Select(p => new {
                        p.IDProducto,
                        p.Nombre_Producto,
                        p.Stock_Producto,
                        p.PrecioUnitario_Producto
                    }).ToListAsync();
            }
            else if (tipo == "InventarioValorado")
            {
                result = await _context.Productos
                    .Select(p => new {
                        p.IDProducto,
                        p.Nombre_Producto,
                        p.Stock_Producto,
                        p.PrecioUnitario_Producto,
                        Valor = p.Stock_Producto * p.PrecioUnitario_Producto
                    }).ToListAsync();
            }

            // ✅ EXPORTAR PDF
            if (!string.IsNullOrEmpty(export) && export.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = ExportToPdf(result, tipo, desde.Value, hasta.Value);
                return File(bytes, "application/pdf", $"{tipo}_{DateTime.Now:yyyyMMddHHmm}.pdf");
            }

            ViewBag.Tipo = tipo;
            ViewBag.Data = result;
            return View("Results");
        }

            private byte[] ExportToPdf(object data, string titulo, DateTime desde, DateTime hasta)
{
    using (var stream = new MemoryStream())
    {
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // 🖋 Fuentes
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        // 🧾 Encabezado
        document.Add(new Paragraph($"Reporte: {titulo}")
            .SetFont(boldFont)
            .SetFontSize(16)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Desde: {desde:dd/MM/yyyy}   Hasta: {hasta:dd/MM/yyyy}")
            .SetFont(regularFont)
            .SetFontSize(11)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(15));

        // Convierte el resultado dinámico en lista
        var list = (data as IEnumerable<object>)?.ToList();

        if (list == null || !list.Any())
        {
            document.Add(new Paragraph("⚠️ No hay datos disponibles para este reporte.")
                .SetFont(regularFont)
                .SetFontSize(11)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(ColorConstants.GRAY));
            
            document.Close();
            return stream.ToArray();
        }

        // Obtiene propiedades de los objetos
        var first = list.First();
        var props = first.GetType().GetProperties();

        // 📋 Crear tabla
        var table = new Table(props.Length).UseAllAvailableWidth();
        Color headerColor = new DeviceRgb(106, 17, 203);

        // Encabezados
        foreach (var p in props)
        {
            table.AddHeaderCell(new Cell()
                .Add(new Paragraph(p.Name)
                    .SetFont(boldFont)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetFontSize(10))
                .SetBackgroundColor(headerColor)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(5));
        }

        // Filas
        foreach (var item in list)
        {
            foreach (var p in props)
            {
                var val = p.GetValue(item)?.ToString() ?? "";
                table.AddCell(new Cell()
                    .Add(new Paragraph(val)
                        .SetFont(regularFont)
                        .SetFontSize(9))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(5));
            }
        }

        document.Add(table);

        // Totales
        document.Add(new Paragraph($"\nTotal de registros: {list.Count}")
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetFontSize(11)
            .SetFont(boldFont));

        // Pie de página
        document.Add(new Paragraph("\nReporte generado por Icolsa S.A.")
            .SetFont(regularFont)
            .SetFontSize(9)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.GRAY));

        document.Close();
        return stream.ToArray();
    }
}

    }
}
