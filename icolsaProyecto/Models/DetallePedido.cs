using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class DetallePedido
    {
        [Key]
        public int IDDetallePedido { get; set; }

        public int? IDPedido { get; set; }
        [ForeignKey("IDPedido")]
        public Pedido? Pedido { get; set; }

        public int? IDProducto { get; set; }
        [ForeignKey("IDProducto")]
        public Producto? Producto { get; set; }

        public int? Cantidad { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? Subtotal { get; set; }
    }
}