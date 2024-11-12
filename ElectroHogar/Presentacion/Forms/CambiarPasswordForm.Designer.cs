using ElectroHogar.Presentacion.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Forms
{
    partial class CambiarPasswordForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTitulo = new System.Windows.Forms.Label();
            this.txtPasswordActual = new System.Windows.Forms.TextBox();
            this.txtPasswordNueva = new System.Windows.Forms.TextBox();
            this.txtConfirmarPassword = new System.Windows.Forms.TextBox();
            this.btnCambiar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblPasswordActual = new System.Windows.Forms.Label();
            this.lblPasswordNueva = new System.Windows.Forms.Label();
            this.lblConfirmarPassword = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();

            // Panel superior
            this.panel1.BackColor = FormHelper.ColorPrimario;
            this.panel1.Controls.Add(this.labelTitulo);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(FormHelper.ANCHO_FORM, 75);

            // Label título
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = FormHelper.FuenteTitulo;
            this.labelTitulo.ForeColor = Color.White;
            this.labelTitulo.Location = new System.Drawing.Point(20, 20);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Text = "Cambiar Contraseña";

            // Labels
            this.lblPasswordActual.AutoSize = true;
            this.lblPasswordActual.Font = FormHelper.FuenteNormal;
            this.lblPasswordActual.Location = new System.Drawing.Point(50, 100);
            this.lblPasswordActual.Text = "Contraseña Actual:";

            this.lblPasswordNueva.AutoSize = true;
            this.lblPasswordNueva.Font = FormHelper.FuenteNormal;
            this.lblPasswordNueva.Location = new System.Drawing.Point(50, 160);
            this.lblPasswordNueva.Text = "Nueva Contraseña:";

            this.lblConfirmarPassword.AutoSize = true;
            this.lblConfirmarPassword.Font = FormHelper.FuenteNormal;
            this.lblConfirmarPassword.Location = new System.Drawing.Point(50, 220);
            this.lblConfirmarPassword.Text = "Confirmar Contraseña:";

            // TextBoxes
            this.txtPasswordActual.Font = FormHelper.FuenteNormal;
            this.txtPasswordActual.Location = new System.Drawing.Point(50, 120);
            this.txtPasswordActual.Size = new System.Drawing.Size(300, 25);

            this.txtPasswordNueva.Font = FormHelper.FuenteNormal;
            this.txtPasswordNueva.Location = new System.Drawing.Point(50, 180);
            this.txtPasswordNueva.Size = new System.Drawing.Size(300, 25);

            this.txtConfirmarPassword.Font = FormHelper.FuenteNormal;
            this.txtConfirmarPassword.Location = new System.Drawing.Point(50, 240);
            this.txtConfirmarPassword.Size = new System.Drawing.Size(300, 25);

            // Buttons
            this.btnCambiar.Location = new System.Drawing.Point(50, 290);
            this.btnCambiar.Size = new System.Drawing.Size(300, 35);
            this.btnCambiar.Text = "CAMBIAR CONTRASEÑA";
            this.btnCambiar.Click += new System.EventHandler(this.btnCambiar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(50, 335);
            this.btnCancelar.Size = new System.Drawing.Size(300, 35);
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new System.Drawing.Size(FormHelper.ANCHO_FORM, 450);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.panel1,
                this.lblPasswordActual,
                this.txtPasswordActual,
                this.lblPasswordNueva,
                this.txtPasswordNueva,
                this.lblConfirmarPassword,
                this.txtConfirmarPassword,
                this.btnCambiar,
                this.btnCancelar
            });

            this.Name = "CambiarPasswordForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.TextBox txtPasswordActual;
        private System.Windows.Forms.TextBox txtPasswordNueva;
        private System.Windows.Forms.TextBox txtConfirmarPassword;
        private System.Windows.Forms.Button btnCambiar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblPasswordActual;
        private System.Windows.Forms.Label lblPasswordNueva;
        private System.Windows.Forms.Label lblConfirmarPassword;
    }

}