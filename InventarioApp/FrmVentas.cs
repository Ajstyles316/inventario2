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
    public partial class FrmVentas : Form
    {
        private readonly ClienteService _clientes = new ClienteService();
        private readonly ProductoService _productos = new ProductoService();
        private readonly VentaService _ventas = new VentaService();

        // BindingList: el grid se actualiza solo al agregar o quitar
        private readonly BindingList<LineaVenta> _carrito = new BindingList<LineaVenta>();
        public FrmVentas()
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var item = cboProducto.SelectedItem as ProductoListaItem;
            if (item == null)
            {
                MessageBox.Show("No hay productos disponibles.");
                return;
            }

            int cantidad = (int)nudCantidad.Value;

            // Si ya está en el carrito, sumamos la cantidad
            var existente = _carrito.FirstOrDefault(l => l.ProductoId == item.Id);
            int yaEnCarrito = existente == null ? 0 : existente.Cantidad;

            if (yaEnCarrito + cantidad > item.Stock)
            {
                MessageBox.Show($"Stock insuficiente. Disponible: {item.Stock}.");
                return;
            }

            if (existente != null)
            {
                existente.Cantidad += cantidad;
                _carrito.ResetBindings(); // refresca el grid
            }
            else
            {
                _carrito.Add(new LineaVenta
                {
                    ProductoId = item.Id,
                    Producto = item.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = item.Precio
                });
            }

            ActualizarTotal();
        }

        private void FrmVentas_Load(object sender, EventArgs e)
        {
            cboCliente.DisplayMember = "Nombre";
            cboCliente.ValueMember = "Id";
            cboCliente.DataSource = _clientes.Listar();

            CargarProductosCombo();

            dgvCarrito.DataSource = _carrito;
            dgvCarrito.Columns["ProductoId"].Visible = false;
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ActualizarTotal();
        }
        private void CargarProductosCombo()
        {
            // Solo productos con stock disponible
            var lista = _productos.Listar().Where(p => p.Stock > 0).ToList();
            cboProducto.DisplayMember = "Nombre";
            cboProducto.ValueMember = "Id";
            cboProducto.DataSource = lista;
        }
        private void ActualizarTotal()
        {
            lblTotal.Text = "Total: " + _carrito.Sum(l => l.Subtotal).ToString("N2");
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow == null) return;
            _carrito.RemoveAt(dgvCarrito.CurrentRow.Index);
            ActualizarTotal();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (cboCliente.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }

            try
            {
                int idVenta = _ventas.RegistrarVenta(
                    (int)cboCliente.SelectedValue, _carrito.ToList());

                MessageBox.Show($"Venta #{idVenta} registrada correctamente.");

                _carrito.Clear();
                ActualizarTotal();
                CargarProductosCombo(); // el stock cambió
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
