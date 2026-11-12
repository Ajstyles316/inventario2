using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioApp
{
    [Table("Categorias")]
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Una categoría tiene muchos productos
        public virtual ICollection<Producto> Productos { get; set; }
    }

    [Table("Productos")]
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public int CategoriaId { get; set; }           // la clave foránea
        public virtual Categoria Categoria { get; set; } // el objeto relacionado
    }

    [Table("Clientes")]
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

    [Table("Ventas")]
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public decimal Total { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<DetalleVenta> Detalles { get; set; }
    }

    [Table("DetalleVentas")]
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public virtual Venta Venta { get; set; }
        public virtual Producto Producto { get; set; }
    }
    [Table("Usuarios")]
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
    }
}
