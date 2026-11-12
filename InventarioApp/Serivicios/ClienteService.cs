using System.Collections.Generic;
using System.Linq;

namespace InventarioApp
{
    public class ClienteService
    {
        public List<Cliente> Listar()
        {
            using (var db = new InventarioModel())
            {
                return db.Clientes.OrderBy(c => c.Nombre).ToList();
            }
        }

        public void Guardar(Cliente c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre))
                throw new ReglaNegocioException("El nombre del cliente es obligatorio.");

            using (var db = new InventarioModel())
            {
                if (c.Id == 0)
                {
                    db.Clientes.Add(c);
                }
                else
                {
                    var existente = db.Clientes.Find(c.Id);
                    if (existente == null)
                        throw new ReglaNegocioException("El cliente ya no existe.");
                    existente.Nombre = c.Nombre;
                    existente.Telefono = c.Telefono;
                    existente.Email = c.Email;
                }
                db.SaveChanges();
            }
        }
    }
}