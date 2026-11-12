using System;

namespace InventarioApp
{
    // Error "esperado": algo que el usuario hizo mal y debemos explicarle
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }
}