using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using icolsaProyecto.Data;
using icolsaProyecto.Models;

namespace icolsaProyecto.Controllers
{
    [Route("Pedido")]
    public class PedidoController : Controller
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly MyDbContext _context;

        public PedidoController(ILogger<PedidoController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // 📄 Mostrar lista de pedidos con sus detalles y pagos
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .Include(p => p.Pagos) // si en tu modelo Pedido tienes ICollection<Pago>
                .ToListAsync();

            return View(pedidos);
        }

        // 🆕 Crear nuevo pedido (GET)
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = _context.Productos.ToList();
            ViewBag.Usuarios = _context.Usuarios.ToList();
            ViewBag.MetodosPago = _context.MetodosPago.ToList();
            ViewBag.EstadosPago = _context.EstadosPago.ToList();

            return View();
        }

     [HttpPost("Create")]
[ValidateAntiForgeryToken]
public IActionResult Create(Pedido pedido, List<DetallePedido> detalles, int? IDMetodoPago, decimal? montoPago)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Clientes = _context.Clientes.ToList();
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.Usuarios = _context.Usuarios.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        ViewBag.EstadosPago = _context.EstadosPago.ToList();
        return View(pedido);
    }

    if (detalles == null || !detalles.Any())
    {
        ModelState.AddModelError("", "Debe agregar al menos un producto al pedido.");
        ViewBag.Clientes = _context.Clientes.ToList();
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.Usuarios = _context.Usuarios.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        ViewBag.EstadosPago = _context.EstadosPago.ToList();
        return View(pedido);
    }

    // 🧮 Validar stock antes de crear el pedido
    var productosInsuficientes = new List<string>();
    foreach (var detalle in detalles.Where(d => d.IDProducto.HasValue))
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == detalle.IDProducto);
        if (producto == null) continue;

        if (detalle.Cantidad > producto.Stock_Producto)
        {
            productosInsuficientes.Add($"{producto.Nombre_Producto}: stock insuficiente ({producto.Stock_Producto} disponibles)");
        }
    }

    if (productosInsuficientes.Any())
    {
        ModelState.AddModelError("", "Stock insuficiente:\n" + string.Join("\n", productosInsuficientes));
        return View(pedido);
    }

 // Procesar cada detalle, descontar stock y crear historial
    foreach (var det in detalles.Where(d => d.IDProducto.HasValue && d.Cantidad > 0))
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == det.IDProducto);
        if (producto == null) continue;

        var detalleNuevo = new DetallePedido
        {
            IDPedido = pedido.IDPedido,
            IDProducto = producto.IDProducto,
            Cantidad = det.Cantidad ?? 0,
            PrecioUnitario = producto.PrecioUnitario_Producto,
            Subtotal = (det.Cantidad ?? 0) * producto.PrecioUnitario_Producto
        };

        // Descontar stock
        producto.Stock_Producto -= (det.Cantidad ?? 0);
        _context.Productos.Update(producto);

        // Historial inventario
        var historial = new HistorialInventarioSaldo
        {
            IDProducto = producto.IDProducto,
            TipoMovimiento = "Salida",
            Saldo_Actual = producto.Stock_Producto,
            Fecha_Actualizacion = DateTime.Now
        };
        _context.HistorialInventarioSaldos.Add(historial);

    }


    
            // === Asignar detalles al pedido ===
            pedido.Detalles = detalles;
    pedido.Fecha_Pedido = DateTime.Now;
    pedido.Cantidad_Pedido = detalles.Sum(d => d.Cantidad ?? 0);
    pedido.TotalPago_Pedido = detalles.Sum(d => d.Subtotal ?? 0);

    // Guardar el pedido con sus detalles
    _context.Pedidos.Add(pedido);
    _context.SaveChanges();

    // === Registrar pago (si se seleccionó un método) ===
    if (IDMetodoPago.HasValue && montoPago.HasValue)
    {
        var pago = new Pago
        {
            Fecha_Pago = DateTime.Now,
            Monto_Pago = montoPago.Value,
            IDMetodoPago = IDMetodoPago.Value,
            IDPedido = pedido.IDPedido,
            IDEstadoPago = _context.EstadosPago.FirstOrDefault(e => e.NombreEstado == "Completado")?.IDEstadoPago
        };
        _context.Pagos.Add(pago);
        _context.SaveChanges();
    }

    TempData["SuccessMessage"] = "Pedido y pago registrados correctamente.";
    return RedirectToAction("Index");
}

[HttpGet("Edit")]
public IActionResult Edit(int id)
{
    var pedido = _context.Pedidos
        .Include(p => p.Cliente)
        .Include(p => p.Usuario)
        .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
        .Include(p => p.Pagos)
            .ThenInclude(p => p.MetodoPago)
        .Include(p => p.Pagos)
            .ThenInclude(p => p.EstadoPago)
        .FirstOrDefault(p => p.IDPedido == id);

    if (pedido == null)
        return NotFound();

    // Cargar datos para dropdowns
    ViewBag.Clientes = _context.Clientes.ToList();
    ViewBag.Productos = _context.Productos.ToList();
    ViewBag.Usuarios = _context.Usuarios.ToList();
    ViewBag.MetodosPago = _context.MetodosPago.ToList();
    ViewBag.EstadosPago = _context.EstadosPago.ToList();

    // Obtener el último pago o el pago más reciente
    var ultimoPago = pedido.Pagos?.OrderByDescending(p => p.Fecha_Pago).FirstOrDefault();
    ViewBag.UltimoPago = ultimoPago;

    return View(pedido);
}

/*
[HttpPost("Create")]
[ValidateAntiForgeryToken]
public IActionResult Create(Pedido pedido, List<DetallePedido> detalles, int? IDMetodoPago, decimal? montoPago)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Clientes = _context.Clientes.ToList();
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.Usuarios = _context.Usuarios.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        ViewBag.EstadosPago = _context.EstadosPago.ToList();
        return View(pedido);
    }

    if (detalles == null || !detalles.Any())
    {
        ModelState.AddModelError("", "Debe agregar al menos un producto al pedido.");
        return View(pedido);
    }

    // 🧮 Validar stock antes de crear el pedido
    var productosInsuficientes = new List<string>();
    foreach (var detalle in detalles.Where(d => d.IDProducto.HasValue))
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == detalle.IDProducto);
        if (producto == null) continue;

        if (detalle.Cantidad > producto.Stock_Producto)
        {
            productosInsuficientes.Add($"{producto.Nombre_Producto}: stock insuficiente ({producto.Stock_Producto} disponibles)");
        }
    }

    if (productosInsuficientes.Any())
    {
        ModelState.AddModelError("", "Stock insuficiente:\n" + string.Join("\n", productosInsuficientes));
        return View(pedido);
    }

    // === Crear pedido ===
    pedido.Fecha_Pedido = DateTime.Now;
    pedido.Detalles = new List<DetallePedido>();
    _context.Pedidos.Add(pedido);
    _context.SaveChanges();

    decimal totalPedido = 0m;

    // Procesar cada detalle, descontar stock y crear historial
    foreach (var det in detalles.Where(d => d.IDProducto.HasValue && d.Cantidad > 0))
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == det.IDProducto);
        if (producto == null) continue;

        var detalleNuevo = new DetallePedido
        {
            IDPedido = pedido.IDPedido,
            IDProducto = producto.IDProducto,
            Cantidad = det.Cantidad ?? 0,
            PrecioUnitario = producto.PrecioUnitario_Producto,
            Subtotal = (det.Cantidad ?? 0) * producto.PrecioUnitario_Producto
        };

        // Descontar stock
        producto.Stock_Producto -= (det.Cantidad ?? 0);
        _context.Productos.Update(producto);

        // Historial inventario
        var historial = new HistorialInventarioSaldo
        {
            IDProducto = producto.IDProducto,
            TipoMovimiento = "Salida",
            Saldo_Actual = producto.Stock_Producto,
            Fecha_Actualizacion = DateTime.Now
        };
        _context.HistorialInventarioSaldos.Add(historial);

        totalPedido += detalleNuevo.Subtotal ?? 0;
        _context.DetallePedidos.Add(detalleNuevo);
    }

    pedido.TotalPago_Pedido = totalPedido;
    pedido.Cantidad_Pedido = detalles.Sum(d => d.Cantidad ?? 0);
    _context.Update(pedido);
    _context.SaveChanges();

    // === Registrar pago (si corresponde) ===
    if (IDMetodoPago.HasValue && montoPago.HasValue)
    {
        var pago = new Pago
        {
            Fecha_Pago = DateTime.Now,
            Monto_Pago = montoPago.Value,
            IDMetodoPago = IDMetodoPago.Value,
            IDPedido = pedido.IDPedido,
            IDEstadoPago = _context.EstadosPago.FirstOrDefault(e => e.NombreEstado == "Completado")?.IDEstadoPago
        };
        _context.Pagos.Add(pago);
        _context.SaveChanges();
    }

    TempData["SuccessMessage"] = "Pedido creado correctamente con control de stock.";
    return RedirectToAction("Index");
}


[HttpPost("Edit")]
[ValidateAntiForgeryToken]
public IActionResult Edit(Pedido pedido, List<DetallePedido> detalles, int IDMetodoPago, int IDEstadoPago, decimal MontoPago, DateTime FechaPago)
{
    var existente = _context.Pedidos
        .Include(p => p.Detalles)
        .Include(p => p.Pagos)
        .FirstOrDefault(p => p.IDPedido == pedido.IDPedido);

    if (existente == null)
        return NotFound();

    // 🧩 Restaurar stock previo antes de actualizar
    foreach (var detAntiguo in existente.Detalles)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == detAntiguo.IDProducto);
        if (producto != null)
        {
            producto.Stock_Producto += (detAntiguo.Cantidad ?? 0); // devolver stock anterior
            _context.Productos.Update(producto);
        }
    }
    _context.SaveChanges();

    // 🧮 Validar stock nuevo
    var insuficientes = new List<string>();
    foreach (var det in detalles)
    {
        var prod = _context.Productos.FirstOrDefault(p => p.IDProducto == det.IDProducto);
        if (prod == null) continue;
        if (det.Cantidad > prod.Stock_Producto)
            insuficientes.Add($"{prod.Nombre_Producto} (stock actual {prod.Stock_Producto})");
    }

    if (insuficientes.Any())
    {
        ModelState.AddModelError("", "Stock insuficiente para los siguientes productos:\n" + string.Join("\n", insuficientes));
        return View(pedido);
    }

    // === Actualizar pedido ===
    existente.Fecha_Pedido = DateTime.Now;
    existente.IDCliente = pedido.IDCliente;
    existente.IDUsuario = pedido.IDUsuario;

    _context.DetallePedidos.RemoveRange(existente.Detalles);
    decimal total = 0m;

    foreach (var det in detalles)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == det.IDProducto);
        if (producto == null) continue;

        var detalleNuevo = new DetallePedido
        {
            IDPedido = existente.IDPedido,
            IDProducto = producto.IDProducto,
            Cantidad = det.Cantidad ?? 0,
            PrecioUnitario = producto.PrecioUnitario_Producto,
            Subtotal = (det.Cantidad ?? 0) * producto.PrecioUnitario_Producto
        };

        _context.DetallePedidos.Add(detalleNuevo);

        // Descontar stock
        producto.Stock_Producto -= (det.Cantidad ?? 0);
        _context.Productos.Update(producto);

        // Historial inventario
        var historial = new HistorialInventarioSaldo
        {
            IDProducto = producto.IDProducto,
            TipoMovimiento = "Salida",
            Saldo_Actual = producto.Stock_Producto,
            Fecha_Actualizacion = DateTime.Now
        };
        _context.HistorialInventarioSaldos.Add(historial);

        total += detalleNuevo.Subtotal ?? 0;
    }

    existente.TotalPago_Pedido = total;
    existente.Cantidad_Pedido = detalles.Sum(d => d.Cantidad ?? 0);

    // === Actualizar o crear pago ===
    var pago = existente.Pagos.FirstOrDefault();
    if (pago == null)
    {
        pago = new Pago { IDPedido = existente.IDPedido };
        _context.Pagos.Add(pago);
    }

    pago.IDMetodoPago = IDMetodoPago;
    pago.IDEstadoPago = IDEstadoPago;
    pago.Monto_Pago = MontoPago;
    pago.Fecha_Pago = FechaPago;

    _context.SaveChanges();

    TempData["SuccessMessage"] = "Pedido actualizado correctamente con control de stock.";
    return RedirectToAction("Index");
}
*/

        // 🧾 Crear pedido directo desde cliente (flujo web)
        [HttpGet("CreateCliente")]
public IActionResult CreateCliente(int? idProducto)
{
    ViewBag.Productos = _context.Productos.ToList();
    ViewBag.MetodosPago = _context.MetodosPago.ToList();

    if (idProducto.HasValue)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == idProducto.Value);
        ViewBag.ProductoSeleccionado = producto;
    }

    return View();
}

[HttpPost("CreateCliente")]
[ValidateAntiForgeryToken]
public IActionResult CreateCliente(Cliente cliente, List<DetallePedido> detalles, int IDMetodoPago)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        return View(cliente);
    }

    if (detalles == null || !detalles.Any())
    {
        ModelState.AddModelError("", "Debe seleccionar al menos un producto.");
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        return View(cliente);
    }

    // Validar stock disponible ANTES de crear el pedido
    var productosInsuficientes = new List<string>();
    foreach (var detalle in detalles.Where(d => d.IDProducto.HasValue))
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == detalle.IDProducto);
        
        if (producto == null)
        {
            productosInsuficientes.Add($"Producto ID {detalle.IDProducto} no encontrado.");
            continue;
        }

        if (detalle.Cantidad > producto.Stock_Producto)
        {
            productosInsuficientes.Add($"{producto.Nombre_Producto}: Solicita {detalle.Cantidad} pero solo hay {producto.Stock_Producto} disponibles.");
        }
    }

    if (productosInsuficientes.Any())
    {
        ModelState.AddModelError("", "Stock insuficiente:\n" + string.Join("\n", productosInsuficientes));
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        return View(cliente);
    }
            // Buscar o crear cliente
            var existente = _context.Clientes.FirstOrDefault(c =>
        c.Correo_Cliente == cliente.Correo_Cliente ||
        c.Telefono_Cliente == cliente.Telefono_Cliente);

    if (existente == null)
    {
        _context.Clientes.Add(cliente);
        _context.SaveChanges();
    }
    else
    {
        cliente = existente;
    }

    // Crear pedido
    var pedido = new Pedido
    {
        IDCliente = cliente.IDCliente,
        Fecha_Pedido = DateTime.Now,
        Detalles = detalles,
        Cantidad_Pedido = detalles.Sum(d => d.Cantidad ?? 0),
        TotalPago_Pedido = detalles.Sum(d => d.Subtotal ?? 0)
    };

    _context.Pedidos.Add(pedido);
    _context.SaveChanges();


    // Agregar detalles y DESCONTAR STOCK
decimal totalPedido = 0m;

// Copiar lista antes de recorrer
var detallesProcesar = detalles
    .Where(d => d.IDProducto.HasValue && d.Cantidad > 0)
    .ToList();

foreach (var detalle in detallesProcesar)
{
    var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == detalle.IDProducto);
    if (producto == null) continue;

    // Crear detalle del pedido
    var detallePedido = new DetallePedido
    {
        IDPedido = pedido.IDPedido,
        IDProducto = detalle.IDProducto.Value,
        Cantidad = detalle.Cantidad ?? 0,
        PrecioUnitario = producto.PrecioUnitario_Producto,
        Subtotal = (detalle.Cantidad ?? 0) * producto.PrecioUnitario_Producto
    };

    _context.DetallePedidos.Add(detallePedido);

    // Descontar stock
    producto.Stock_Producto -= (detalle.Cantidad ?? 0);
    _context.Productos.Update(producto);

    totalPedido += detallePedido.Subtotal ?? 0;

    // Historial inventario
    var historial = new HistorialInventarioSaldo
    {
        IDProducto = detalle.IDProducto.Value,
        TipoMovimiento = "Salida",
        Saldo_Actual = producto.Stock_Producto,
        Fecha_Actualizacion = DateTime.Now,
    };
    _context.HistorialInventarioSaldos.Add(historial);
}

        // Actualizar total del pedido
        pedido.TotalPago_Pedido = totalPedido;
        _context.Pedidos.Update(pedido);

            // Registrar pago
            var pago = new Pago
    {
        IDPedido = pedido.IDPedido,
        IDMetodoPago = IDMetodoPago,
        IDEstadoPago = _context.EstadosPago.FirstOrDefault(e => e.NombreEstado == "Pendiente")?.IDEstadoPago ?? 1, // Valor por defecto si no encuentra "Pendiente"
        Fecha_Pago = DateTime.Now,
        Monto_Pago = pedido.TotalPago_Pedido ?? 0m // Asegurarse que no sea null
    };
    
    // Validar que tengamos los datos necesarios
    if (pago.IDMetodoPago == 0)
    {
        ModelState.AddModelError("", "Debe seleccionar un método de pago válido.");
        // Recargar datos necesarios para la vista
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        return View(cliente);
    }
    
    _context.Pagos.Add(pago);
    
    try
    {
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Tu pedido ha sido registrado correctamente.";
        return RedirectToAction("Index","Home");
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", "Error al guardar el pago: " + ex.Message);
        // Recargar datos necesarios para la vista
        ViewBag.Productos = _context.Productos.ToList();
        ViewBag.MetodosPago = _context.MetodosPago.ToList();
        return View(cliente);
    }
}

        // Página de confirmación simple
        [HttpGet("ConfirmacionCliente")]
        public IActionResult ConfirmacionCliente()
        {
            return View();
        }




                /*[HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pedido pedido, List<DetallePedido> detalles, 
            int IDMetodoPago, int IDEstadoPago, decimal MontoPago, DateTime FechaPago)
        {
            var existente = _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Pagos)
                .FirstOrDefault(p => p.IDPedido == pedido.IDPedido);

            if (existente == null)
                return NotFound();
        
            // Actualizar pedido
            existente.Fecha_Pedido = pedido.Fecha_Pedido ?? DateTime.Now;
            existente.IDCliente = pedido.IDCliente;
            existente.IDUsuario = pedido.IDUsuario;
        
            // Actualizar detalles
            _context.DetallePedidos.RemoveRange(existente.Detalles);
            foreach (var det in detalles)
            {
                det.IDPedido = pedido.IDPedido;
                _context.DetallePedidos.Add(det);
            }
        
            // Actualizar o crear pago
            var pago = existente.Pagos.FirstOrDefault();
            if (pago == null)
            {
                pago = new Pago
                {
                    IDPedido = pedido.IDPedido
                };
                _context.Pagos.Add(pago);
            }

            pago.IDMetodoPago = IDMetodoPago;
            pago.IDEstadoPago = IDEstadoPago;
            pago.Monto_Pago = MontoPago;
            pago.Fecha_Pago = FechaPago;
        
            // Actualizar totales
            existente.Cantidad_Pedido = detalles.Sum(d => d.Cantidad ?? 0);
            existente.TotalPago_Pedido = detalles.Sum(d => d.Subtotal ?? 0);
        
            _context.SaveChanges();
        
            TempData["SuccessMessage"] = "Pedido actualizado correctamente.";
            return RedirectToAction("Index");
        }
       */
       [HttpPost("Edit")]
[ValidateAntiForgeryToken]
public IActionResult Edit(Pedido pedido, List<DetallePedido> detalles, 
    int IDMetodoPago, int IDEstadoPago, decimal MontoPago, DateTime FechaPago)
{
    var existente = _context.Pedidos
        .Include(p => p.Detalles)
        .Include(p => p.Pagos)
        .FirstOrDefault(p => p.IDPedido == pedido.IDPedido);

    if (existente == null)
        return NotFound();
        
    // Actualizar pedido
    existente.Fecha_Pedido = pedido.Fecha_Pedido ?? DateTime.Now;
    existente.IDCliente = pedido.IDCliente;
    existente.IDUsuario = pedido.IDUsuario;

    // 🟣 NUEVO: Revertir stock anterior antes de actualizar
    foreach (var detAnterior in existente.Detalles)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == detAnterior.IDProducto);
        if (producto != null)
        {
            producto.Stock_Producto += (detAnterior.Cantidad ?? 0); // devolver stock anterior
            _context.Productos.Update(producto);

            var historialEntrada = new HistorialInventarioSaldo
            {
                IDProducto = producto.IDProducto,
                TipoMovimiento = "Entrada (edición pedido)",
                Saldo_Actual = producto.Stock_Producto,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.HistorialInventarioSaldos.Add(historialEntrada);
        }
    }

    // Actualizar detalles
    _context.DetallePedidos.RemoveRange(existente.Detalles);
    foreach (var det in detalles)
    {
        det.IDPedido = pedido.IDPedido;
        _context.DetallePedidos.Add(det);

        // 🟣 NUEVO: Descontar stock nuevo y registrar historial
        if (det.IDProducto.HasValue && det.Cantidad > 0)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.IDProducto == det.IDProducto);
            if (producto != null)
            {
                producto.Stock_Producto -= (det.Cantidad ?? 0);
                _context.Productos.Update(producto);

                var historialSalida = new HistorialInventarioSaldo
                {
                    IDProducto = producto.IDProducto,
                    TipoMovimiento = "Salida (edición pedido)",
                    Saldo_Actual = producto.Stock_Producto,
                    Fecha_Actualizacion = DateTime.Now
                };
                _context.HistorialInventarioSaldos.Add(historialSalida);
            }
        }
    }

    // Actualizar o crear pago
    var pago = existente.Pagos.FirstOrDefault();
    if (pago == null)
    {
        pago = new Pago
        {
            IDPedido = pedido.IDPedido
        };
        _context.Pagos.Add(pago);
    }

    pago.IDMetodoPago = IDMetodoPago;
    pago.IDEstadoPago = IDEstadoPago;
    pago.Monto_Pago = MontoPago;
    pago.Fecha_Pago = FechaPago;

    // Actualizar totales
    existente.Cantidad_Pedido = detalles.Sum(d => d.Cantidad ?? 0);
    existente.TotalPago_Pedido = detalles.Sum(d => d.Subtotal ?? 0);

    _context.SaveChanges();

    TempData["SuccessMessage"] = "Pedido actualizado correctamente.";
    return RedirectToAction("Index");
}


        // 🗑️ Eliminar pedido
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefault(p => p.IDPedido == id);

            if (pedido == null)
                return NotFound();

            _context.DetallePedidos.RemoveRange(pedido.Detalles);
            _context.Pedidos.Remove(pedido);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Pedido eliminado correctamente.";
            return RedirectToAction("Index");
        }

        // 🔎 Buscar cliente (autocomplete)
        [HttpGet("Buscar")]
        public JsonResult Buscar(string term)
        {
            var resultados = _context.Clientes
                .Where(c => c.Nombre_Cliente.Contains(term) || c.NIT_Cliente.Contains(term))
                .Select(c => new
                {
                    id = c.IDCliente,
                    nombre = c.Nombre_Cliente + " " + c.Apellido_Cliente,
                    nit = c.NIT_Cliente
                }).ToList();

            return Json(resultados);
        }

        // ⚡ Crear cliente rápido (modal)
        [HttpPost("CrearRapido")]
        public JsonResult CrearRapido([FromBody] Cliente nuevoCliente)
        {
            if (string.IsNullOrWhiteSpace(nuevoCliente.Nombre_Cliente))
                return Json(new { error = "El nombre del cliente es obligatorio." });

            _context.Clientes.Add(nuevoCliente);
            _context.SaveChanges();

            return Json(new
            {
                id = nuevoCliente.IDCliente,
                nombre = $"{nuevoCliente.Nombre_Cliente} {nuevoCliente.Apellido_Cliente}"
            });
        }

        // ⚡ Crear método de pago rápido (modal)
        [HttpPost("CrearMetodoPago")]
        public JsonResult CrearMetodoPago([FromBody] MetodoPago nuevoMetodo)
        {
            if (string.IsNullOrWhiteSpace(nuevoMetodo.NombreMetodo))
                return Json(new { error = "El nombre del método de pago es obligatorio." });

            _context.MetodosPago.Add(nuevoMetodo);
            _context.SaveChanges();

            return Json(new
            {
                id = nuevoMetodo.IDMetodoPago,
                nombre = nuevoMetodo.NombreMetodo
            });
        }

        // ⚠️ Página de error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
