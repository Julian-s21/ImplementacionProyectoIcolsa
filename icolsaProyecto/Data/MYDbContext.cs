/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using icolsaProyecto.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace icolsaProyecto.Data
{
    public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
    {

        // Tablas
        // Tablas (DbSets)d
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<HistorialInventarioSaldo> HistorialInventarioSaldos { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;database=icolsaDatabase;User=root;Password=$54K38k5";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }



        // ======================
        // RELACIONES ENTRE TABLAS
        // ======================
        // ======================
        // RELACIONES ENTRE TABLAS
        // ======================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- USUARIO ----------
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.IDUsuario);

            // ---------- CLIENTE ----------
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.IDCliente);

            // ---------- CATEGORIA ----------
            modelBuilder.Entity<Categoria>()
                .HasKey(ca => ca.IDCategoria);

            // ---------- PRODUCTO ----------
            modelBuilder.Entity<Producto>()
                .HasKey(p => p.IDProducto);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.IDCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- PEDIDO ----------
            modelBuilder.Entity<Pedido>()
                .HasKey(pe => pe.IDPedido);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Cliente)
                .WithMany()
                .HasForeignKey(pe => pe.IDCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Producto)
                .WithMany()
                .HasForeignKey(pe => pe.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Usuario)
                .WithMany()
                .HasForeignKey(pe => pe.IDUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- PAGO ----------
            modelBuilder.Entity<Pago>()
                .HasKey(pa => pa.IDPago);

            modelBuilder.Entity<Pago>()
                .HasOne(pa => pa.Cliente)
                .WithMany()
                .HasForeignKey(pa => pa.IDCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- REPORTE ----------
            modelBuilder.Entity<Reporte>()
                .HasKey(r => r.IDReporte);

            modelBuilder.Entity<Reporte>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.IDUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- HISTORIAL INVENTARIO SALDO ----------
            modelBuilder.Entity<HistorialInventarioSaldo>()
                .HasKey(h => h.IDInventarioSaldo);

            modelBuilder.Entity<HistorialInventarioSaldo>()
                .HasOne(h => h.Producto)
                .WithMany()
                .HasForeignKey(h => h.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


        

}


*/
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using icolsaProyecto.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace icolsaProyecto.Data
{
    public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
    {

        // ======================
        // TABLAS (DbSets)
        // ======================
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<HistorialInventarioSaldo> HistorialInventarioSaldos { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;database=icolsaDatabase;User=root;Password=$54K38k5";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }


        // ======================
        // RELACIONES ENTRE TABLAS
        // ======================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- USUARIO ----------
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.IDUsuario);

            // ---------- CLIENTE ----------
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.IDCliente);

            // ---------- CATEGORIA ----------
            modelBuilder.Entity<Categoria>()
                .HasKey(ca => ca.IDCategoria);

            // ---------- PRODUCTO ----------
            modelBuilder.Entity<Producto>()
                .HasKey(p => p.IDProducto);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.IDCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- PEDIDO ----------
            modelBuilder.Entity<Pedido>()
                .HasKey(pe => pe.IDPedido);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Cliente)
                .WithMany()
                .HasForeignKey(pe => pe.IDCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Producto)
                .WithMany()
                .HasForeignKey(pe => pe.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Usuario)
                .WithMany()
                .HasForeignKey(pe => pe.IDUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- DETALLE PEDIDO ----------
            modelBuilder.Entity<DetallePedido>()
                .HasKey(dp => dp.IDDetallePedido);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(dp => dp.IDPedido)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Producto)
                .WithMany()
                .HasForeignKey(dp => dp.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- PAGO ----------
            modelBuilder.Entity<Pago>()
                .HasKey(pa => pa.IDPago);

            modelBuilder.Entity<Pago>()
                .HasOne(pa => pa.Cliente)
                .WithMany()
                .HasForeignKey(pa => pa.IDCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- REPORTE ----------
            modelBuilder.Entity<Reporte>()
                .HasKey(r => r.IDReporte);

            modelBuilder.Entity<Reporte>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.IDUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- HISTORIAL INVENTARIO SALDO ----------
            modelBuilder.Entity<HistorialInventarioSaldo>()
                .HasKey(h => h.IDInventarioSaldo);

            modelBuilder.Entity<HistorialInventarioSaldo>()
                .HasOne(h => h.Producto)
                .WithMany()
                .HasForeignKey(h => h.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using icolsaProyecto.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;


namespace icolsaProyecto.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        // ======================
        // TABLAS (DbSets)
        // ======================
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<EstadoPago> EstadosPago { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<HistorialInventarioSaldo> HistorialInventarioSaldos { get; set; }

        // ======================
        // CONFIGURACIÓN
        // ======================
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "server=localhost;database=icolsaDatabase;User=root;Password=$54K38k5";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        // ======================
        // RELACIONES ENTRE TABLAS
        // ======================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- USUARIO ----------
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.IDUsuario);

            // ---------- CLIENTE ----------
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.IDCliente);

            // ---------- CATEGORIA ----------
            modelBuilder.Entity<Categoria>()
                .HasKey(ca => ca.IDCategoria);

            // ---------- PRODUCTO ----------
            modelBuilder.Entity<Producto>()
                .HasKey(p => p.IDProducto);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.IDCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- PEDIDO ----------
            modelBuilder.Entity<Pedido>()
                .HasKey(pe => pe.IDPedido);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Cliente)
                .WithMany()
                .HasForeignKey(pe => pe.IDCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(pe => pe.Usuario)
                .WithMany()
                .HasForeignKey(pe => pe.IDUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- DETALLE PEDIDO ----------
            modelBuilder.Entity<DetallePedido>()
                .HasKey(dp => dp.IDDetallePedido);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(dp => dp.IDPedido)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Producto)
                .WithMany()
                .HasForeignKey(dp => dp.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- METODO PAGO ----------
            modelBuilder.Entity<MetodoPago>()
                .HasKey(mp => mp.IDMetodoPago);

            modelBuilder.Entity<MetodoPago>()
                .Property(mp => mp.NombreMetodo)
                .HasMaxLength(50)
                .IsRequired();

            // ---------- ESTADO PAGO ----------
            modelBuilder.Entity<EstadoPago>()
                .HasKey(ep => ep.IDEstadoPago);

            modelBuilder.Entity<EstadoPago>()
                .Property(ep => ep.NombreEstado)
                .HasMaxLength(50)
                .IsRequired();

            // ---------- PAGO ----------
            modelBuilder.Entity<Pago>()
                .HasKey(pa => pa.IDPago);

            modelBuilder.Entity<Pago>()
                .HasOne(pa => pa.MetodoPago)
                .WithMany()
                .HasForeignKey(pa => pa.IDMetodoPago)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(pa => pa.Pedido)
                .WithMany()
                .HasForeignKey(pa => pa.IDPedido)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pago>()
                .HasOne(pa => pa.EstadoPago)
                .WithMany()
                .HasForeignKey(pa => pa.IDEstadoPago)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- REPORTE ----------
            modelBuilder.Entity<Reporte>()
                .HasKey(r => r.IDReporte);

            modelBuilder.Entity<Reporte>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.IDUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- HISTORIAL INVENTARIO SALDO ----------
            modelBuilder.Entity<HistorialInventarioSaldo>()
                .HasKey(h => h.IDInventarioSaldo);

            modelBuilder.Entity<HistorialInventarioSaldo>()
                .HasOne(h => h.Producto)
                .WithMany()
                .HasForeignKey(h => h.IDProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
