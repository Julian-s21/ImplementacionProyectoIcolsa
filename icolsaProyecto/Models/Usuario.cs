using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Usuario
    {
[Key]
        public int IDUsuario { get; set; }

        public string? Nombre_Usuario { get; set; }
        public string? Correo_Usuario { get; set; }
        public string? Contrasena_Usuario { get; set; }
        public string? Rol_Usuario { get; set; }

        // Relaciones
        public ICollection<Pedido>? Pedidos { get; set; }
        public ICollection<Reporte>? Reportes { get; set; }
    }
}