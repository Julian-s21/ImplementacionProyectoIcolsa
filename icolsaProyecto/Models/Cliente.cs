using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Cliente
    {
         [Key]
        public int IDCliente { get; set; }

        public string? Nombre_Cliente { get; set; }
        public string? Apellido_Cliente { get; set; }
        public string? Direccion_Cliente { get; set; }
        public string? Telefono_Cliente { get; set; }
        public string? Correo_Cliente { get; set; }
        public string? NIT_Cliente { get; set; }

        // Relaciones
        public ICollection<Pedido>? Pedidos { get; set; }
        public ICollection<Pago>? Pagos { get; set; }
    }
}