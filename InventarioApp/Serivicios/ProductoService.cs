using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace InventarioApp
{
    // Objeto liviano solo para mostrar en la tabla
    public class ProductoListaItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }
    }

    public class ProductoService
    {
        public List<ProductoListaItem> Listar(string filtro = null)
        {
            using (var db = new InventarioModel())
            {
                var consulta = db.Productos.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filtro))
                    consulta = consulta.Where(p => p.Nombre.Contains(filtro));

                return consulta
                    .OrderBy(p => p.Nombre)
                    .Select(p => new ProductoListaItem
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        Categoria = p.Categoria.Nombre
                    })
                    .ToList();
            }
        }

        public Producto ObtenerPorId(int id)
        {
            using (var db = new InventarioModel())
            {
                return db.Productos.Find(id);
            }
        }

        // Si p.Id == 0 crea; si no, actualiza
        public void Guardar(Producto p)
        {
            // Reglas de negocio: viven aquí, no en el formulario
            if (string.IsNullOrWhiteSpace(p.Nombre))
                throw new ReglaNegocioException("El nombre es obligatorio.");
            if (p.Precio < 0)
                throw new ReglaNegocioException("El precio no puede ser negativo.");
            if (p.Stock < 0)
                throw new ReglaNegocioException("El stock no puede ser negativo.");

            using (var db = new InventarioModel())
            {
                bool duplicado = db.Productos.Any(x =>
                    x.Nombre == p.Nombre && x.Id != p.Id);
                if (duplicado)
                    throw new ReglaNegocioException("Ya existe un producto con ese nombre.");

                if (p.Id == 0)
                {
                    db.Productos.Add(p);
                }
                else
                {
                    var existente = db.Productos.Find(p.Id);
                    if (existente == null)
                        throw new ReglaNegocioException("El producto ya no existe.");

                    existente.Nombre = p.Nombre;
                    existente.Precio = p.Precio;
                    existente.Stock = p.Stock;
                    existente.CategoriaId = p.CategoriaId;
                }

                db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            try
            {
                using (var db = new InventarioModel())
                {
                    var p = db.Productos.Find(id);
                    if (p == null) return;

                    db.Productos.Remove(p);
                    db.SaveChanges();
                }
            }
            catch (DbUpdateException)
            {
                throw new ReglaNegocioException(
                    "No se puede eliminar: el producto ya tiene ventas registradas.");
            }
        }
    }
}