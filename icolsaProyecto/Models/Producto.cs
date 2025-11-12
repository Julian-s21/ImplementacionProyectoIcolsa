using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace icolsaProyecto.Models
{
    public class Producto
    {
        [Key]
        public int IDProducto { get; set; }

        public string? Nombre_Producto { get; set; }
        public string? Codigo_Producto { get; set; }
         [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? PrecioUnitario_Producto { get; set; }

        public int? Stock_Producto { get; set; }

        [Display(Name = "Imagen del producto")]
        public string? ImagenUrl { get; set; }


        public int? IDCategoria { get; set; }
        [ForeignKey("IDCategoria")]
        public Categoria? Categoria { get; set; }


        public ICollection<Pedido>? Pedidos { get; set; }
        public ICollection<HistorialInventarioSaldo>? HistorialInventarioSaldos { get; set; }
    }
}