using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class HistorialInventarioSaldo
    {
        [Key]
        public int IDInventarioSaldo { get; set; }

        public string? TipoMovimiento { get; set; }
        public int? Saldo_Actual { get; set; }
        public DateTime? Fecha_Actualizacion { get; set; }

        public int? IDProducto { get; set; }
        [ForeignKey("IDProducto")]
        public Producto? Producto { get; set; }
    }
}