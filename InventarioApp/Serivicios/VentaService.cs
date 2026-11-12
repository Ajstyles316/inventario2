using System;
using System.Collections.Generic;
using System.Linq;

namespace InventarioApp
{
    // Una línea del carrito (todavía no guardada)
    public class LineaVenta
    {
        public int ProductoId { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get { return Cantidad * PrecioUnitario; } }
    }

    public class VentaService
    {
        public int RegistrarVenta(int clienteId, List<LineaVenta> lineas)
        {
            if (clienteId <= 0)
                throw new ReglaNegocioException("Selecciona un cliente.");
            if (lineas == null || lineas.Count == 0)
                throw new ReglaNegocioException("Agrega al menos un producto.");

            using (var db = new InventarioModel())
            {
                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    ClienteId = clienteId,
                    Detalles = new List<DetalleVenta>()
                };

                foreach (var linea in lineas)
                {
                    // Releemos el producto de la BD: el stock real manda
                    var producto = db.Productos.Find(linea.ProductoId);
                    if (producto == null)
                        throw new ReglaNegocioException("Un producto ya no existe.");
                    if (linea.Cantidad <= 0)
                        throw new ReglaNegocioException("Las cantidades deben ser mayores a 0.");
                    if (producto.Stock < linea.Cantidad)
                        throw new ReglaNegocioException(
                            $"Stock insuficiente de {producto.Nombre}. Disponible: {producto.Stock}.");

                    producto.Stock -= linea.Cantidad; // descuento automático

                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = producto.Id,
                        Cantidad = linea.Cantidad,
                        PrecioUnitario = producto.Precio // precio de la BD, no el de la pantalla
                    });
                }

                venta.Total = venta.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

                db.Ventas.Add(venta);
                db.SaveChanges(); // venta + detalles + stock: todo o nada

                return venta.Id;
            }
        }
    }
}