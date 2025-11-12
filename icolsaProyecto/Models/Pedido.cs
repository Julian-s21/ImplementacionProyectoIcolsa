using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Pedido
    {
        [Key]
        public int IDPedido { get; set; }

        public DateTime? Fecha_Pedido { get; set; }
        public int? Cantidad_Pedido { get; set; }
        public decimal? TotalPago_Pedido { get; set; }

        public int? IDCliente { get; set; }
        [ForeignKey("IDCliente")]
        public Cliente? Cliente { get; set; }

        public int? IDProducto { get; set; }
        [ForeignKey("IDProducto")]
        public Producto? Producto { get; set; }

        public int? IDUsuario { get; set; }
        [ForeignKey("IDUsuario")]
        public Usuario? Usuario { get; set; }
         [NotMapped]
        public Pago? Pago { get; set; }

        // 🔹 Nueva relación
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    
         // Si un pedido puede tener varios pagos
            // (opcional) acceso rápido al primer pago
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    }
}