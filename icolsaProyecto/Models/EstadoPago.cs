using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class EstadoPago
    {
        [Key]
        public int IDEstadoPago { get; set; }

        [Required]
        [MaxLength(50)]
        public required string NombreEstado { get; set; }
    }
}