using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Pago
    {
       [Key]
        public int IDPago { get; set; }

        [Required]
        public DateTime Fecha_Pago { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto_Pago { get; set; }

        [Required]
        [ForeignKey("MetodoPago")]
        public int IDMetodoPago { get; set; }
        public MetodoPago? MetodoPago { get; set; }

        [Required]
        [ForeignKey("Pedido")]
        public int IDPedido { get; set; }
        public Pedido? Pedido { get; set; }

        [ForeignKey("EstadoPago")]
        public int? IDEstadoPago { get; set; }
        public EstadoPago? EstadoPago { get; set; }
    }
}