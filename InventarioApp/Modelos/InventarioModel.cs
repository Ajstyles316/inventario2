using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioApp
{
    public class InventarioModel : DbContext
    {
        public InventarioModel() : base("name=InventarioModel")
        {
            // Importante: la BD ya existe, EF no debe intentar crearla ni modificarla
            Database.SetInitializer<InventarioModel>(null);
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
