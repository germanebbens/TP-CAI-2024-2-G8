using System.Drawing;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Utils
{
    public static class FormHelper
    {
        // Colores corporativos
        public static Color ColorPrimario = Color.FromArgb(0, 122, 204);
        public static Color ColorFondoTextBox = Color.FromArgb(240, 248, 255);
        public static Font FuenteNormal = new Font("Segoe UI", 9.75F);
        public static Font FuenteTitulo = new Font("Segoe UI", 18F, FontStyle.Bold);

        public static void ConfigurarFormularioBase(Form form)
        {
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.BackColor = Color.White;
        }

        public static Panel CrearPanelSuperior(string titulo)
        {
            var panel = new Panel
            {
                BackColor = ColorPrimario,
                Dock = DockStyle.Top,
                Height = 75
            };

            var label = new Label
            {
                AutoSize = true,
                Font = FuenteTitulo,
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Text = titulo
            };

            panel.Controls.Add(label);
            return panel;
        }

        public static Button CrearBotonPrimario(string texto, int width = 300)
        {
            return new Button
            {
                BackColor = ColorPrimario,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(FuenteNormal, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(width, 35),
                Text = texto.ToUpper(),
                UseVisualStyleBackColor = false
            };
        }

        public static Label CrearLabel(string texto)
        {
            return new Label
            {
                AutoSize = true,
                Font = FuenteNormal,
                Text = texto
            };
        }

        public static TextBox CrearTextBox(int width = 300)
        {
            return new TextBox
            {
                Font = FuenteNormal,
                Size = new Size(width, 25)
            };
        }

        public static void ConfigurarTextBoxPassword(TextBox textBox)
        {
            textBox.PasswordChar = '*';
            textBox.MaxLength = 15;
        }

        public static Label CrearLabelEstado()
        {
            return new Label
            {
                AutoSize = true,
                Font = new Font(FuenteNormal.FontFamily, 9F),
                ForeColor = Color.Gray,
                Visible = false
            };
        }

        public static void MostrarEstado(Label lblEstado, string mensaje, bool esError = false)
        {
            lblEstado.ForeColor = esError ? Color.Red : Color.Gray;
            lblEstado.Text = mensaje;
            lblEstado.Visible = true;
        }

        public static void MostrarCargando(Button boton, bool cargando, string textoNormal, string textoCargando)
        {
            boton.Text = cargando ? textoCargando : textoNormal;
            boton.Enabled = !cargando;
        }

        public static void AgregarEfectoFoco(TextBox textBox)
        {
            textBox.Enter += (s, e) => textBox.BackColor = ColorFondoTextBox;
            textBox.Leave += (s, e) => textBox.BackColor = Color.White;
        }

        public static void ConfigurarEnterTab(TextBox origen, Control destino)
        {
            origen.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    destino.Focus();
                }
            };
        }
    }
}