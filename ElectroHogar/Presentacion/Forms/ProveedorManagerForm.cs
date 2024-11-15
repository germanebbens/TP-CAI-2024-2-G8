using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class ProveedorManagerForm : Form
    {
        private readonly Proveedores _proveedorService;
        private readonly Label lblEstado;
        private readonly Panel panelAltaProveedor;
        private bool acordeonAbierto = false;

        public ProveedorManagerForm()
        {
            InitializeComponent();
            _proveedorService = new Proveedores();
            lblEstado = FormHelper.CrearLabelEstado();
            panelAltaProveedor = CrearPanelAltaProveedor();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Gestión de Proveedores";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 600);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Gestión de Proveedores");
            this.Controls.Add(panelSuperior);

            // Botón Ver Proveedores Activos
            var btnProveedoresActivos = FormHelper.CrearBotonPrimario("Ver Proveedores Activos");
            btnProveedoresActivos.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 20);
            btnProveedoresActivos.Click += (s, e) => {
                var configProveedores = new ProveedoresListadoConfig();
                var formProveedores = new GenericListForm(configProveedores);
                formProveedores.ShowDialog();
            };

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(btnProveedoresActivos.Right + 20, panelSuperior.Bottom + 20);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnProveedoresActivos.Bottom + 10);
            panelAltaProveedor.Location = new Point(0, lblEstado.Bottom + 10);

            this.Controls.AddRange(new Control[] {
            btnVolver,
            btnProveedoresActivos,
            lblEstado,
            panelAltaProveedor
        });
        }

        private Panel CrearPanelAltaProveedor()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Header del acordeón
            var btnAcordeon = FormHelper.CrearBotonPrimario("+ Agregar Nuevo Proveedor", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2));
            btnAcordeon.Location = new Point(FormHelper.MARGEN, 10);
            btnAcordeon.Click += (s, e) => ToggleAcordeon();

            // Contenido del acordeón
            var panelContenido = new Panel
            {
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Location = new Point(FormHelper.MARGEN, btnAcordeon.Bottom + 10),
                AutoSize = true,
                Visible = false
            };

            int currentY = 10;

            // Campos del formulario
            var txtNombre = FormHelper.CrearCampoTexto("Nombre:", "txtNombre", ref currentY, panelContenido);
            var txtApellido = FormHelper.CrearCampoTexto("Apellido:", "txtApellido", ref currentY, panelContenido);
            var txtEmail = FormHelper.CrearCampoTexto("Email:", "txtEmail", ref currentY, panelContenido);
            var txtCuit = FormHelper.CrearCampoTexto("CUIT:", "txtCuit", ref currentY, panelContenido);


            // Botón guardar
            var btnGuardar = FormHelper.CrearBotonPrimario("Guardar Proveedor", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 3));
            btnGuardar.Location = new Point(FormHelper.MARGEN, currentY + 20);
            btnGuardar.Click += (s, e) => GuardarProveedor();

            panelContenido.Controls.AddRange(new Control[] {
            btnGuardar
        });

            panel.Controls.AddRange(new Control[] {
            btnAcordeon,
            panelContenido
        });

            return panel;
        }

        private void ToggleAcordeon()
        {
            acordeonAbierto = !acordeonAbierto;
            panelAltaProveedor.Controls[1].Visible = acordeonAbierto;
            var btnAcordeon = (Button)panelAltaProveedor.Controls[0];
            btnAcordeon.Text = acordeonAbierto ? "- Cerrar" : "+ Agregar Nuevo Proveedor";
        }

        private void GuardarProveedor()
        {
            try
            {
                var panelContenido = (Panel)panelAltaProveedor.Controls[1];

                var nombre = ((TextBox)panelContenido.Controls["txtNombre"]).Text.Trim();
                var apellido = ((TextBox)panelContenido.Controls["txtApellido"]).Text.Trim();
                var email = ((TextBox)panelContenido.Controls["txtEmail"]).Text.Trim();
                var cuit = ((TextBox)panelContenido.Controls["txtCuit"]).Text.Trim();

                _proveedorService.RegistrarProveedor(
                    nombre,
                    apellido,
                    email,
                    cuit
                );

                FormHelper.MostrarEstado(lblEstado, "Proveedor registrado exitosamente", false);
                ToggleAcordeon();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void LimpiarFormulario()
        {
            var panelContenido = (Panel)panelAltaProveedor.Controls[1];

            foreach (Control control in panelContenido.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
            }
        }
    }
}