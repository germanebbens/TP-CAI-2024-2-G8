using System;
using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class ClienteEdicionForm : Form
    {
        private readonly Clientes _clientesService;
        private readonly ClienteList _cliente;
        private readonly Label lblEstado;

        public ClienteEdicionForm(Guid clienteId)
        {
            InitializeComponent();
            _clientesService = new Clientes();
            _cliente = _clientesService.ObtenerClientePorId(clienteId);
            lblEstado = FormHelper.CrearLabelEstado();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "Editar Cliente";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            var panel = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };

            int currentY = 10;

            var txtId = FormHelper.CrearCampoTexto("ID:", "txtId", ref currentY, panel);
            txtId.ReadOnly = true;
            txtId.Text = _cliente.Id.ToString();

            var txtNombre = FormHelper.CrearCampoTexto("Nombre:", "txtNombre", ref currentY, panel);
            txtNombre.ReadOnly = true;
            txtNombre.Text = $"{_cliente.Nombre} {_cliente.Apellido}";

            var txtDireccion = FormHelper.CrearCampoTexto("Dirección:", "txtDireccion", ref currentY, panel);
            txtDireccion.Text = _cliente.Direccion;

            var txtTelefono = FormHelper.CrearCampoTexto("Teléfono:", "txtTelefono", ref currentY, panel);
            txtTelefono.Text = _cliente.Telefono;

            var txtEmail = FormHelper.CrearCampoTexto("Email:", "txtEmail", ref currentY, panel);
            txtEmail.Text = _cliente.Email;

            var btnGuardar = FormHelper.CrearBotonPrimario("Guardar Cambios", 200);
            btnGuardar.Location = new Point(FormHelper.MARGEN, currentY + 20);
            btnGuardar.Click += (s, e) => GuardarCambios();

            var btnCancelar = FormHelper.CrearBotonPrimario("Cancelar", 200);
            btnCancelar.Location = new Point(FormHelper.MARGEN, btnGuardar.Bottom + 10);
            btnCancelar.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnCancelar.Bottom + 10);

            panel.Controls.AddRange(new Control[] {
            txtId, txtNombre, txtDireccion, txtTelefono, txtEmail,
            btnGuardar, btnCancelar, lblEstado
        });

            this.Controls.Add(panel);
        }

        private void GuardarCambios()
        {
            try
            {
                var txtDireccion = (TextBox)this.Controls[0].Controls["txtDireccion"];
                var txtTelefono = (TextBox)this.Controls[0].Controls["txtTelefono"];
                var txtEmail = (TextBox)this.Controls[0].Controls["txtEmail"];
                _clientesService.ModificarCliente(
                    _cliente.Id,
                    txtDireccion.Text.Trim(),
                    txtTelefono.Text.Trim(),
                    txtEmail.Text.Trim()
                );
                FormHelper.MostrarEstado(lblEstado, "Cliente actualizado exitosamente", false);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }
    }
}
