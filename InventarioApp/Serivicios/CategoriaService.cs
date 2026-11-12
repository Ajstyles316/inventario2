using System.Collections.Generic;
using System.Linq;

namespace InventarioApp
{
    public class CategoriaService
    {
        public List<Categoria> Listar()
        {
            using (var db = new InventarioModel())
            {
                return db.Categorias.OrderBy(c => c.Nombre).ToList();
            }
        }
    }
}