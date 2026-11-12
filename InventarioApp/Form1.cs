using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows.Forms;

namespace InventarioApp
{
    public partial class Form1 : Form
    {
        private readonly ProductoService _productos = new ProductoService();
        private readonly CategoriaService _categorias = new CategoriaService();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboCategoria.DisplayMember = "Nombre";
            cboCategoria.ValueMember = "Id";
            cboCategoria.DataSource = _categorias.Listar();
            CargarProductos();
            Text = $"Inventario - {Sesion.UsuarioActual.NombreUsuario} ({Sesion.UsuarioActual.Rol})";
            if (!Sesion.EsAdmin)
            {
                btnNuevo.Enabled = false;
                btnGuardar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        // ---------- READ ----------
        private void CargarCategorias()
        {
            using (var db = new InventarioModel())
            {
                var categorias = db.Categorias
                    .Select(c => new { c.Id, c.Nombre })
                    .ToList();

                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "Id";
                cboCategoria.DataSource = categorias;
            }
        }

        private void CargarProductos()
        {
            dgvProductos.DataSource = _productos.Listar();
            dgvProductos.Columns["Id"].Visible = false;
        }


        private void LimpiarFormulario()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            txtNombre.Focus();
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["Id"].Value);
            var p = _productos.ObtenerPorId(id);
            if (p == null) return;

            txtId.Text = p.Id.ToString();
            txtNombre.Text = p.Nombre;
            txtPrecio.Text = p.Precio.ToString();
            txtStock.Text = p.Stock.ToString();
            cboCategoria.SelectedValue = p.CategoriaId;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Sesion.EsAdmin)
                throw new ReglaNegocioException("No tienes permiso para esta acción.");
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio debe ser un número.");
                return;
            }
            if (!int.TryParse(txtStock.Text, out int stock))
            {
                MessageBox.Show("El stock debe ser un número entero.");
                return;
            }
            if (cboCategoria.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una categoría.");
                return;
            }

            var p = new Producto
            {
                Id = string.IsNullOrEmpty(txtId.Text) ? 0 : int.Parse(txtId.Text),
                Nombre = txtNombre.Text.Trim(),
                Precio = precio,
                Stock = stock,
                CategoriaId = (int)cboCategoria.SelectedValue
            };

            try
            {
                _productos.Guardar(p);
                CargarProductos();
                LimpiarFormulario();
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!Sesion.EsAdmin)
                throw new ReglaNegocioException("No tienes permiso para esta acción.");
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Primero selecciona un producto de la tabla.");
                return;
            }

            var respuesta = MessageBox.Show("¿Seguro que quieres eliminar este producto?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes) return;

            try
            {
                _productos.Eliminar(int.Parse(txtId.Text));
                CargarProductos();
                LimpiarFormulario();
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmVentas())
            {
                frm.ShowDialog();
            }
            CargarProductos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmReporte())
            {
                frm.ShowDialog();
            }
        }
    }
}