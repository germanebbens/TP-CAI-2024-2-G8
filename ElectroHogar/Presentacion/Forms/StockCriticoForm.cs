using System;
using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;


namespace ElectroHogar.Presentacion.Forms
{
    public partial class StockCriticoForm : Form
    {
        private readonly Productos _productosService;
        private readonly Label lblEstado;
        private DataGridView dgvProductos;

        public StockCriticoForm()
        {
            InitializeComponent();
            _productosService = new Productos();
            lblEstado = FormHelper.CrearLabelEstado();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Reporte de Stock Crítico";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 660);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Reporte de Stock Crítico");
            this.Controls.Add(panelSuperior);

            var lblNota = new Label
            {
                Text = "Nota: \nSe considera stock crítico a los productos que tienen cantidad menor de 25 unidades.\nPara manejar un stock crítico por producto, se debería solicitar el stock ideal \nal momento de la creación (o modificación) del producto. \nDe modo de poder validar contra algún valor establecido.",
                AutoSize = true,
                Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 10),
                Font = new Font(FormHelper.FuenteNormal, FontStyle.Regular)
            };
            this.Controls.Add(lblNota);

            dgvProductos = new DataGridView
            {
                Location = new Point(FormHelper.MARGEN, lblNota.Bottom + 10),
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Height = 400,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvProductos.Columns.AddRange(new DataGridViewColumn[]
            {
            new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = "Producto",
                DataPropertyName = "Nombre",
                Width = 200
            },
            new DataGridViewTextBoxColumn
            {
                Name = "IdCategoria",
                HeaderText = "Categoría",
                DataPropertyName = "IdCategoria",
                Width = 100
            },
            new DataGridViewTextBoxColumn
            {
                Name = "Stock",
                HeaderText = "Stock",
                DataPropertyName = "Stock",
                Width = 100
            }
            });

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, dgvProductos.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);

            this.Controls.AddRange(new Control[] {
            panelSuperior,
            dgvProductos,
            btnVolver,
            lblEstado
        });

            CargarProductosStockCritico();
        }

        private void CargarProductosStockCritico()
        {
            try
            {
                var productosStockCritico = _productosService.ObtenerProductosStockCritico();
                dgvProductos.DataSource = productosStockCritico;
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }
    }
}
