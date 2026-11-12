using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventarioApp
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarios = new UsuarioService();
        private int _intentos = 0;
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            var u = _usuarios.Autenticar(txtUsuario.Text.Trim(), txtClave.Text);

            if (u == null)
            {
                _intentos++;
                if (_intentos >= 3)
                {
                    MessageBox.Show("Demasiados intentos fallidos. La aplicación se cerrará.");
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                // Mensaje genérico: no revelamos si falló el usuario o la clave
                MessageBox.Show("Usuario o contraseña incorrectos.");
                txtClave.Clear();
                txtClave.Focus();
                return;
            }

            Sesion.UsuarioActual = u;
            DialogResult = DialogResult.OK;
        }
    }
}
