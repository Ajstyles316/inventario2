using System;
using System.Data;
using System.Linq;

namespace InventarioApp
{
    public class ReporteService
    {
        public DataSet VentasPorRango(DateTime desde, DateTime hasta)
        {
            DateTime inicio = desde.Date;
            DateTime fin = hasta.Date.AddDays(1); // incluye todo el día "hasta"

            using (var db = new InventarioModel())
            {
                var datos = db.DetalleVentas
                    .Where(d => d.Venta.Fecha >= inicio && d.Venta.Fecha < fin)
                    .OrderBy(d => d.Venta.Fecha)
                    .Select(d => new
                    {
                        d.VentaId,
                        d.Venta.Fecha,
                        Cliente = d.Venta.Cliente.Nombre,
                        Producto = d.Producto.Nombre,
                        d.Cantidad,
                        d.PrecioUnitario
                    })
                    .ToList();

                // Usamos el mismo nombre de tabla que definimos en el .xsd
                var ds = new dsVentas();
                var tabla = ds.Tables["DtVentas"];

                foreach (var x in datos)
                {
                    tabla.Rows.Add(
                        x.VentaId, x.Fecha, x.Cliente, x.Producto,
                        x.Cantidad, x.PrecioUnitario,
                        x.Cantidad * x.PrecioUnitario);
                }

                return ds;
            }
        }
    }
}