namespace InventarioApp
{
    public static class Sesion
    {
        public static Usuario UsuarioActual { get; set; }

        public static bool EsAdmin
        {
            get { return UsuarioActual != null && UsuarioActual.Rol == "Admin"; }
        }

        public static void Cerrar()
        {
            UsuarioActual = null;
        }
    }
}