using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class MetodoPago
    {
         [Key]
        public int IDMetodoPago { get; set; }

        [Required]
        [MaxLength(50)]
        public required string NombreMetodo { get; set; }
    }
}