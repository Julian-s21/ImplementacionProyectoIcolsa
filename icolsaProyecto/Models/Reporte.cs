using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Reporte
    {
          [Key]
        public int IDReporte { get; set; }

        public string? Tipo_Reporte { get; set; }
        public DateTime? Fecha_Reporte { get; set; }

        public int? IDUsuario { get; set; }
        [ForeignKey("IDUsuario")]
        public Usuario? Usuario { get; set; }
    }
}