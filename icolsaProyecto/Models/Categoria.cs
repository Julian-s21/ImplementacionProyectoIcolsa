using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Categoria
    {
         [Key]
        public int IDCategoria { get; set; }

        public string? Nombre_Categoria { get; set; }

        // Relación
        public ICollection<Producto>? Productos { get; set; }
    }
}