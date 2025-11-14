using System.Linq;
using System.Threading.Tasks;
using icolsaProyecto.Data;
using icolsaProyecto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace icolsaProyecto.Controllers
{
    [Route("Usuario")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly MyDbContext _context;

        public UsuarioController(ILogger<UsuarioController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ======================================
        // LISTAR TODOS LOS USUARIOS
        // ======================================
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // ======================================
        // LOGIN (GET)
        // ======================================
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(); // Views/Usuario/Login.cshtml
        }

        // ======================================
        // LOGIN (POST)
        // ======================================
        /*[HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Usuario model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo_Usuario == model.Correo_Usuario &&
                                          u.Contrasena_Usuario == model.Contrasena_Usuario);

            if (usuario != null)
             {

                    if (usuario.Rol_Usuario == "Administrativo")
                    {
                        return RedirectToAction("IndexAdministrativo", "Home"); // Vistas del admin
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Vistas del usuario normal
                    }
              }

            // Guardar datos en sesión
            HttpContext.Session.SetInt32("UserId", usuario.IDUsuario);
            HttpContext.Session.SetString("UserName", usuario.Nombre_Usuario ?? string.Empty);
            HttpContext.Session.SetString("UserRole", usuario.Rol_Usuario ?? string.Empty);

            // Redirige al Home o Dashboard
            return RedirectToAction("Index", "Home");
        }
*/
                [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Usuario model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo_Usuario == model.Correo_Usuario &&
                                        u.Contrasena_Usuario == model.Contrasena_Usuario);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View(model);
            }


            // Guardar datos en sesión correctamente
            HttpContext.Session.SetInt32("UserId", usuario.IDUsuario);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre_Usuario ?? string.Empty);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol_Usuario ?? string.Empty);
            HttpContext.Session.SetString("UsuarioCorreo", usuario.Correo_Usuario ?? string.Empty);

            // Redirección según rol
    switch (usuario.Rol_Usuario)
    {
        case "Administrativo":
            return RedirectToAction("IndexAdministrativo", "Home");

        case "Secretario":
            return RedirectToAction("IndexAdministrativo", "Home");

        case "Empleado":
            return RedirectToAction("IndexAdministrativo", "Home");

        default:
            // Rol no reconocido, redirigir a vista genérica
            return RedirectToAction("Index", "Home");
    }
}

// ======================================
// PERFIL DE USUARIO
// ======================================
[HttpGet("Perfil")]
public IActionResult Perfil()
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var userName = HttpContext.Session.GetString("UsuarioNombre");
    var userCorreo = HttpContext.Session.GetString("UsuarioCorreo");
    var userRol = HttpContext.Session.GetString("UsuarioRol");

    if (userId == null)
    {
        // Si no hay sesión activa, redirigir al login
        return RedirectToAction("Login");
    }

    // Crear un objeto con los datos del usuario
    var perfil = new
    {
        IDUsuario = userId.Value,
        Nombre = userName,
        Correo = userCorreo,
        Rol = userRol
    };

    return View(perfil);
}

        // ======================================
        // REGISTER (GET)
        // ======================================
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View(); // Views/Usuario/Register.cshtml
        }

        // ======================================
        // REGISTER (POST)
        // ======================================
        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Verificar si ya existe el correo
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo_Usuario == model.Correo_Usuario);
            if (existe)
            {
                ViewBag.Error = "El correo ya está registrado.";
                return View(model);
            }

            // Rol por defecto
            model.Rol_Usuario = "Usuario";

            _context.Usuarios.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Usuario registrado correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

     // ============================================================
        // RECUPERAR CONTRASEÑA (GET)
        // ============================================================
        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ============================================================
        // RECUPERAR CONTRASEÑA (POST)
        // ============================================================
       [HttpPost("ForgotPassword")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ForgotPassword(string correo, string nueva, string confirmar)
{
    if (string.IsNullOrWhiteSpace(correo) ||
        string.IsNullOrWhiteSpace(nueva) ||
        string.IsNullOrWhiteSpace(confirmar))
    {
        ViewBag.Error = "Todos los campos son obligatorios.";
        return View("ForgotPassword");
    }

    if (nueva != confirmar)
    {
        ViewBag.Error = "Las contraseñas no coinciden.";
        return View("ForgotPassword");
    }

    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Correo_Usuario == correo);

    if (usuario == null)
    {
        ViewBag.Error = "No existe una cuenta con ese correo.";
        return View("ForgotPassword");
    }

    // Cambiar contraseña
    usuario.Contrasena_Usuario = nueva;
    _context.Update(usuario);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "contraseña actualizada";
            return RedirectToAction(nameof(Login));
}


        // Crear usuario (GET)
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // Crear usuario (POST)
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usuario creado correctamente.";
            return RedirectToAction("Index");
        }

        // Cargar datos de usuario para editar
        [HttpPost("LoadEdit")]
        [ValidateAntiForgeryToken]
        public IActionResult LoadEdit(int id)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Pedidos)
                .Include(u => u.Reportes)
                .FirstOrDefault(u => u.IDUsuario == id);

            if (usuario == null)
                return NotFound();

            return View("Edit", usuario);
        }

        // Editar usuario (POST)
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var existente = _context.Usuarios
                .FirstOrDefault(u => u.IDUsuario == usuario.IDUsuario);

            if (existente == null)
                return NotFound();

            existente.Nombre_Usuario = usuario.Nombre_Usuario;
            existente.Correo_Usuario = usuario.Correo_Usuario;
            existente.Contrasena_Usuario = usuario.Contrasena_Usuario;
            existente.Rol_Usuario = usuario.Rol_Usuario;

            _context.Update(existente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // Eliminar usuario
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.IDUsuario == id);

            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }
        
        // ======================================
        // LOGOUT
        // ======================================
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
