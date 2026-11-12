using System;
using System.Linq;
using System.Security.Cryptography;

namespace InventarioApp
{
    public class UsuarioService
    {
        // ---------- Hash de contraseñas ----------
        private static string GenerarSalt()
        {
            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string CalcularHash(string clave, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                clave, Convert.FromBase64String(salt), 100000, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(pbkdf2.GetBytes(32));
            }
        }

        // ---------- Login ----------
        // Devuelve el usuario si las credenciales son correctas, o null
        public Usuario Autenticar(string nombreUsuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrEmpty(clave))
                return null;

            using (var db = new InventarioModel())
            {
                var u = db.Usuarios.FirstOrDefault(x =>
                    x.NombreUsuario == nombreUsuario && x.Activo);
                if (u == null) return null;

                string hash = CalcularHash(clave, u.Salt);
                return hash == u.PasswordHash ? u : null;
            }
        }

        // ---------- Crear usuarios ----------
        public void Crear(string nombreUsuario, string clave, string rol)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ReglaNegocioException("El nombre de usuario es obligatorio.");
            if (string.IsNullOrEmpty(clave) || clave.Length < 6)
                throw new ReglaNegocioException("La contraseña debe tener al menos 6 caracteres.");
            if (rol != "Admin" && rol != "Vendedor")
                throw new ReglaNegocioException("Rol no válido.");

            using (var db = new InventarioModel())
            {
                if (db.Usuarios.Any(x => x.NombreUsuario == nombreUsuario))
                    throw new ReglaNegocioException("Ese nombre de usuario ya existe.");

                string salt = GenerarSalt();
                db.Usuarios.Add(new Usuario
                {
                    NombreUsuario = nombreUsuario.Trim(),
                    Salt = salt,
                    PasswordHash = CalcularHash(clave, salt),
                    Rol = rol,
                    Activo = true
                });
                db.SaveChanges();
            }
        }

        // Primer arranque: si no hay usuarios, crea un admin inicial
        public void CrearAdminInicialSiNoExiste()
        {
            using (var db = new InventarioModel())
            {
                if (db.Usuarios.Any()) return;
            }
            Crear("admin", "admin123", "Admin");
        }
    }
}