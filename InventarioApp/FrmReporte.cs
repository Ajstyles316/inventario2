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
    public partial class FrmReporte : Form
    {
        private readonly ReporteService _reportes = new ReporteService();
        public FrmReporte()
        {
            InitializeComponent();
        }

        private void FrmReporte_Load(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor que 'Hasta'.");
                return;
            }

            var datos = _reportes.VentasPorRango(dtpDesde.Value, dtpHasta.Value);

            var rpt = new rptVentas();
            rpt.SetDataSource(datos);
            crvReporte.ReportSource = rpt;
        }
    }
}
