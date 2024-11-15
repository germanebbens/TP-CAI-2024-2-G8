using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class ProductoManagerForm : Form
    {
        private readonly Productos _productoService;
        private readonly Label lblEstado;
        private readonly Panel panelAltaProducto;
        private ProveedorList _proveedorSeleccionado;

        public ProductoManagerForm()
        {
            InitializeComponent();
            _productoService = new Productos();
            lblEstado = FormHelper.CrearLabelEstado();
            panelAltaProducto = CrearPanelAltaProducto();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Gestión de Productos";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 600);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Gestión de Productos");
            this.Controls.Add(panelSuperior);

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 20);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);
            panelAltaProducto.Location = new Point(0, lblEstado.Bottom + 10);

            this.Controls.AddRange(new Control[] {
                btnVolver,
                lblEstado,
                panelAltaProducto
            });
        }

        private Panel CrearPanelAltaProducto()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            int currentY = 10;

            // Panel de proveedor seleccionado
            var lblProveedorTitulo = FormHelper.CrearLabel("Proveedor:");
            lblProveedorTitulo.Location = new Point(FormHelper.MARGEN, currentY);
            currentY = lblProveedorTitulo.Bottom + 5;

            var lblProveedorInfo = new Label
            {
                Name = "lblProveedorInfo",
                Text = "Ningún proveedor seleccionado",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            currentY = lblProveedorInfo.Bottom + 5;

            var btnSeleccionarProveedor = FormHelper.CrearBotonPrimario("Seleccionar Proveedor", 200);
            btnSeleccionarProveedor.Location = new Point(FormHelper.MARGEN, currentY);
            btnSeleccionarProveedor.Click += (s, e) => SeleccionarProveedor();
            currentY = btnSeleccionarProveedor.Bottom + 20;

            // Campos del producto
            var txtNombre = FormHelper.CrearCampoTexto("Nombre del Producto:", "txtNombre", ref currentY, panel);

            var lblCategoria = FormHelper.CrearLabel("Categoría:");
            lblCategoria.Location = new Point(FormHelper.MARGEN, currentY);
            currentY = lblCategoria.Bottom + 5;

            var cmbCategoria = new ComboBox
            {
                Name = "cmbCategoria",
                Location = new Point(FormHelper.MARGEN, currentY),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategoria.DisplayMember = "Text";
            cmbCategoria.ValueMember = "Value";
            cmbCategoria.DataSource = Enum.GetValues(typeof(Categoria))
                .Cast<Categoria>()
                .Select(c => new { Text = c.ToString(), Value = c })
                .ToList();
            currentY = cmbCategoria.Bottom + 10;

            var txtPrecio = FormHelper.CrearCampoTexto("Precio:", "txtPrecio", ref currentY, panel);
            var txtStock = FormHelper.CrearCampoTexto("Stock:", "txtStock", ref currentY, panel);

            var btnGuardar = FormHelper.CrearBotonPrimario("Guardar Producto", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 3));
            btnGuardar.Location = new Point(FormHelper.MARGEN, currentY + 20);
            btnGuardar.Click += (s, e) => GuardarProducto();

            panel.Controls.AddRange(new Control[] {
                lblProveedorTitulo,
                lblProveedorInfo,
                btnSeleccionarProveedor,
                txtNombre,
                lblCategoria,
                cmbCategoria,
                txtPrecio,
                txtStock,
                btnGuardar
            });

            return panel;
        }

        private void SeleccionarProveedor()
        {
            var configProveedores = new ProveedoresSeleccionListadoConfig(proveedor => {
                _proveedorSeleccionado = proveedor;
            });

            var formProveedores = new BaseListForm(configProveedores);
            if (formProveedores.ShowDialog() == DialogResult.OK)
            {
                ActualizarInfoProveedor();
            }
        }

        private void ActualizarInfoProveedor()
        {
            if (_proveedorSeleccionado != null)
            {
                var lblProveedorInfo = (Label)panelAltaProducto.Controls["lblProveedorInfo"];
                lblProveedorInfo.Text = $"{_proveedorSeleccionado.Nombre} {_proveedorSeleccionado.Apellido} - CUIT: {_proveedorSeleccionado.Cuit}";
                lblProveedorInfo.ForeColor = Color.Black;
            }
        }

        private void GuardarProducto()
        {
            try
            {
                if (_proveedorSeleccionado == null)
                    throw new Exception("Debe seleccionar un proveedor");

                var txtNombre = (TextBox)panelAltaProducto.Controls["txtNombre"];
                var cmbCategoria = (ComboBox)panelAltaProducto.Controls["cmbCategoria"];
                var txtPrecio = (TextBox)panelAltaProducto.Controls["txtPrecio"];
                var txtStock = (TextBox)panelAltaProducto.Controls["txtStock"];

                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                    throw new Exception("El nombre del producto es requerido");

                if (!double.TryParse(txtPrecio.Text, out double precio) || precio <= 0)
                    throw new Exception("El precio debe ser un número válido mayor a cero");

                if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
                    throw new Exception("El stock debe ser un número válido no negativo");

                _productoService.RegistrarProducto(
                    _proveedorSeleccionado.Id,
                    (Categoria)cmbCategoria.SelectedValue,
                    txtNombre.Text.Trim(),
                    precio,
                    stock
                );

                FormHelper.MostrarEstado(lblEstado, "Producto registrado exitosamente", false);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void LimpiarFormulario()
        {
            _proveedorSeleccionado = null;
            var lblProveedorInfo = (Label)panelAltaProducto.Controls["lblProveedorInfo"];
            lblProveedorInfo.Text = "Ningún proveedor seleccionado";
            lblProveedorInfo.ForeColor = Color.Gray;

            foreach (Control control in panelAltaProducto.Controls)
            {
                if (control is TextBox textBox)
                    textBox.Text = string.Empty;
                else if (control is ComboBox comboBox)
                    comboBox.SelectedIndex = 0;
            }
        }
    }
}