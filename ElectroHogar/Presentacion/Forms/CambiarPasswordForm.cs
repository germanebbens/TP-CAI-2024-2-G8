using System;
using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Negocio;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class CambiarPasswordForm : Form
    {
        private readonly Label lblEstado;
        private readonly LoginNegocio _loginNegocio;
        private readonly string _username;
        private readonly bool _esClaveTemporal;

        public CambiarPasswordForm(string username, bool esClaveTemporal = false)
        {
            InitializeComponent();
            _loginNegocio = LoginNegocio.Instance;
            _username = username;
            _esClaveTemporal = esClaveTemporal;

            lblEstado = FormHelper.CrearLabelEstado();
            lblEstado.Location = new Point(50, btnCancelar.Bottom + 20);
            this.Controls.Add(lblEstado);

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Cambiar Contraseña";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 450);

            FormHelper.ConfigurarTextBoxPassword(txtPasswordActual);
            FormHelper.ConfigurarTextBoxPassword(txtPasswordNueva);
            FormHelper.ConfigurarTextBoxPassword(txtConfirmarPassword);

            btnCambiar.BackColor = FormHelper.ColorPrimario;
            btnCambiar.FlatStyle = FlatStyle.Flat;
            btnCambiar.Font = new Font(FormHelper.FuenteNormal, FontStyle.Bold);
            btnCambiar.ForeColor = Color.White;
            btnCambiar.Size = new Size(300, 35);

            btnCancelar.BackColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font(FormHelper.FuenteNormal, FontStyle.Bold);
            btnCancelar.ForeColor = FormHelper.ColorPrimario;
            btnCancelar.Size = new Size(300, 35);

            if (_esClaveTemporal)
            {
                labelTitulo.Text = "Debe cambiar su contraseña temporal";
                btnCancelar.Visible = false;
            }

            FormHelper.ConfigurarEnterTab(txtPasswordActual, txtPasswordNueva);
            FormHelper.ConfigurarEnterTab(txtPasswordNueva, txtConfirmarPassword);
            FormHelper.ConfigurarEnterTab(txtConfirmarPassword, btnCambiar);

            FormHelper.AgregarEfectoFoco(txtPasswordActual);
            FormHelper.AgregarEfectoFoco(txtPasswordNueva);
            FormHelper.AgregarEfectoFoco(txtConfirmarPassword);
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            lblEstado.Visible = false;

            try
            {
                if (!ValidarFormulario()) return;

                FormHelper.MostrarCargando(btnCambiar, true, "CAMBIAR CONTRASEÑA", "CAMBIANDO...");
                DeshabilitarControles(true);

                var resultado = _loginNegocio.CambiarContraseña(
                    _username,
                    txtPasswordActual.Text,
                    txtPasswordNueva.Text
                );

                if (resultado)
                {
                    MessageBox.Show("Contraseña cambiada exitosamente. Por favor, inicie sesión nuevamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
            finally
            {
                FormHelper.MostrarCargando(btnCambiar, false, "CAMBIAR CONTRASEÑA", "CAMBIANDO...");
                DeshabilitarControles(false);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (!_esClaveTemporal)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtPasswordActual.Text))
            {
                FormHelper.MostrarEstado(lblEstado, "Debe ingresar su contraseña actual", true);
                txtPasswordActual.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPasswordNueva.Text))
            {
                FormHelper.MostrarEstado(lblEstado, "Debe ingresar la nueva contraseña", true);
                txtPasswordNueva.Focus();
                return false;
            }

            if (txtPasswordNueva.Text != txtConfirmarPassword.Text)
            {
                FormHelper.MostrarEstado(lblEstado, "Las contraseñas nuevas no coinciden", true);
                txtConfirmarPassword.Focus();
                return false;
            }

            var (isValid, message) = UsuariosUtils.ValidarCambioContraseña(
                txtPasswordActual.Text,
                txtPasswordNueva.Text
            );

            if (!isValid)
            {
                FormHelper.MostrarEstado(lblEstado, message, true);
                txtPasswordNueva.Focus();
                return false;
            }

            return true;
        }

        private void DeshabilitarControles(bool deshabilitar)
        {
            txtPasswordActual.Enabled = !deshabilitar;
            txtPasswordNueva.Enabled = !deshabilitar;
            txtConfirmarPassword.Enabled = !deshabilitar;
            btnCambiar.Enabled = !deshabilitar;
            btnCancelar.Enabled = !deshabilitar;
        }
    }
}