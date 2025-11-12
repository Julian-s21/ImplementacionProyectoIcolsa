using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace icolsaProyecto.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IDCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre_Categoria = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IDCategoria);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IDCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre_Cliente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellido_Cliente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion_Cliente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono_Cliente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo_Cliente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NIT_Cliente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IDCliente);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstadosPago",
                columns: table => new
                {
                    IDEstadoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreEstado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPago", x => x.IDEstadoPago);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    IDMetodoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreMetodo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.IDMetodoPago);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IDUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre_Usuario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo_Usuario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contrasena_Usuario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rol_Usuario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IDUsuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IDProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre_Producto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo_Producto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioUnitario_Producto = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Stock_Producto = table.Column<int>(type: "int", nullable: true),
                    ImagenUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IDCategoria = table.Column<int>(type: "int", nullable: true),
                    CategoriaIDCategoria = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IDProducto);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaIDCategoria",
                        column: x => x.CategoriaIDCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IDCategoria");
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_IDCategoria",
                        column: x => x.IDCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IDCategoria",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    IDReporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tipo_Reporte = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha_Reporte = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IDUsuario = table.Column<int>(type: "int", nullable: true),
                    UsuarioIDUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.IDReporte);
                    table.ForeignKey(
                        name: "FK_Reportes_Usuarios_IDUsuario",
                        column: x => x.IDUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IDUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportes_Usuarios_UsuarioIDUsuario",
                        column: x => x.UsuarioIDUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IDUsuario");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistorialInventarioSaldos",
                columns: table => new
                {
                    IDInventarioSaldo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoMovimiento = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Saldo_Actual = table.Column<int>(type: "int", nullable: true),
                    Fecha_Actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IDProducto = table.Column<int>(type: "int", nullable: true),
                    ProductoIDProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialInventarioSaldos", x => x.IDInventarioSaldo);
                    table.ForeignKey(
                        name: "FK_HistorialInventarioSaldos_Productos_IDProducto",
                        column: x => x.IDProducto,
                        principalTable: "Productos",
                        principalColumn: "IDProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialInventarioSaldos_Productos_ProductoIDProducto",
                        column: x => x.ProductoIDProducto,
                        principalTable: "Productos",
                        principalColumn: "IDProducto");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    IDPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fecha_Pedido = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Cantidad_Pedido = table.Column<int>(type: "int", nullable: true),
                    TotalPago_Pedido = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    IDCliente = table.Column<int>(type: "int", nullable: true),
                    IDProducto = table.Column<int>(type: "int", nullable: true),
                    IDUsuario = table.Column<int>(type: "int", nullable: true),
                    ClienteIDCliente = table.Column<int>(type: "int", nullable: true),
                    UsuarioIDUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.IDPedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_ClienteIDCliente",
                        column: x => x.ClienteIDCliente,
                        principalTable: "Clientes",
                        principalColumn: "IDCliente");
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_IDCliente",
                        column: x => x.IDCliente,
                        principalTable: "Clientes",
                        principalColumn: "IDCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Productos_IDProducto",
                        column: x => x.IDProducto,
                        principalTable: "Productos",
                        principalColumn: "IDProducto");
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios_IDUsuario",
                        column: x => x.IDUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IDUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios_UsuarioIDUsuario",
                        column: x => x.UsuarioIDUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IDUsuario");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetallePedidos",
                columns: table => new
                {
                    IDDetallePedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IDPedido = table.Column<int>(type: "int", nullable: true),
                    IDProducto = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallePedidos", x => x.IDDetallePedido);
                    table.ForeignKey(
                        name: "FK_DetallePedidos_Pedidos_IDPedido",
                        column: x => x.IDPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IDPedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallePedidos_Productos_IDProducto",
                        column: x => x.IDProducto,
                        principalTable: "Productos",
                        principalColumn: "IDProducto",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IDPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fecha_Pago = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Monto_Pago = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IDMetodoPago = table.Column<int>(type: "int", nullable: false),
                    IDPedido = table.Column<int>(type: "int", nullable: false),
                    IDEstadoPago = table.Column<int>(type: "int", nullable: true),
                    ClienteIDCliente = table.Column<int>(type: "int", nullable: true),
                    PedidoIDPedido = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IDPago);
                    table.ForeignKey(
                        name: "FK_Pagos_Clientes_ClienteIDCliente",
                        column: x => x.ClienteIDCliente,
                        principalTable: "Clientes",
                        principalColumn: "IDCliente");
                    table.ForeignKey(
                        name: "FK_Pagos_EstadosPago_IDEstadoPago",
                        column: x => x.IDEstadoPago,
                        principalTable: "EstadosPago",
                        principalColumn: "IDEstadoPago",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_MetodosPago_IDMetodoPago",
                        column: x => x.IDMetodoPago,
                        principalTable: "MetodosPago",
                        principalColumn: "IDMetodoPago",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_Pedidos_IDPedido",
                        column: x => x.IDPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IDPedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagos_Pedidos_PedidoIDPedido",
                        column: x => x.PedidoIDPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IDPedido");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePedidos_IDPedido",
                table: "DetallePedidos",
                column: "IDPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePedidos_IDProducto",
                table: "DetallePedidos",
                column: "IDProducto");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialInventarioSaldos_IDProducto",
                table: "HistorialInventarioSaldos",
                column: "IDProducto");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialInventarioSaldos_ProductoIDProducto",
                table: "HistorialInventarioSaldos",
                column: "ProductoIDProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ClienteIDCliente",
                table: "Pagos",
                column: "ClienteIDCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IDEstadoPago",
                table: "Pagos",
                column: "IDEstadoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IDMetodoPago",
                table: "Pagos",
                column: "IDMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IDPedido",
                table: "Pagos",
                column: "IDPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PedidoIDPedido",
                table: "Pagos",
                column: "PedidoIDPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteIDCliente",
                table: "Pedidos",
                column: "ClienteIDCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IDCliente",
                table: "Pedidos",
                column: "IDCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IDProducto",
                table: "Pedidos",
                column: "IDProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IDUsuario",
                table: "Pedidos",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_UsuarioIDUsuario",
                table: "Pedidos",
                column: "UsuarioIDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaIDCategoria",
                table: "Productos",
                column: "CategoriaIDCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IDCategoria",
                table: "Productos",
                column: "IDCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_IDUsuario",
                table: "Reportes",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_UsuarioIDUsuario",
                table: "Reportes",
                column: "UsuarioIDUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallePedidos");

            migrationBuilder.DropTable(
                name: "HistorialInventarioSaldos");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "EstadosPago");

            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
