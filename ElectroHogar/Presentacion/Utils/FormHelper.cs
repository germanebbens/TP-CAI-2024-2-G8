using ElectroHogar.Config;
using ElectroHogar.Datos;
using System;
using System.Drawing;
using System.Threading.Tasks;
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
        public static int ANCHO_FORM = ConfigHelper.GetIntValueOrDefault("AnchoFormulario", 600);
        public static int MARGEN = ConfigHelper.GetIntValueOrDefault("MargenFormulario", 20);


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

        public static TextBox CrearCampoTexto(string etiqueta, string nombre, ref int currentY, Panel container)
        {
            var lbl = new Label
            {
                Text = etiqueta,
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };

            var txt = new TextBox
            {
                Name = nombre, // Asignar el nombre al control
                Location = new Point(FormHelper.MARGEN, lbl.Bottom + 5),
                Width = container.Width - (FormHelper.MARGEN * 2)
            };

            container.Controls.AddRange(new Control[] { lbl, txt });
            currentY = txt.Bottom + 10;
            return txt;
        }
        
        public static DateTimePicker CrearCampoFecha(string etiqueta, string nombre, ref int currentY, Panel container)
        {
            var lbl = new Label
            {
                Text = etiqueta,
                Location = new Point(MARGEN, currentY),
                AutoSize = true
            };

            var dtp = new DateTimePicker
            {
                Name = nombre,
                Location = new Point(MARGEN, lbl.Bottom + 5),
                Width = container.Width - (MARGEN * 2),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            container.Controls.AddRange(new Control[] { lbl, dtp });
            currentY = dtp.Bottom + 10;
            return dtp;
        }

        public static ComboBox CrearComboPerfil(string etiqueta, string nombre, ref int currentY, Panel container)
        {
            var lbl = new Label
            {
                Text = etiqueta,
                Location = new Point(MARGEN, currentY),
                AutoSize = true
            };

            var cmb = new ComboBox
            {
                Name = nombre,
                Location = new Point(MARGEN, lbl.Bottom + 5),
                Width = container.Width - (MARGEN * 2),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Agregar los perfiles permitidos (solo Vendedor y Supervisor según la lógica de negocio)
            cmb.DisplayMember = "Text";
            cmb.ValueMember = "Value";

            var items = new[] {
            new { Text = "Vendedor", Value = (int)PerfilUsuario.Vendedor },
            new { Text = "Supervisor", Value = (int)PerfilUsuario.Supervisor }
        };

            cmb.DataSource = items;

            container.Controls.AddRange(new Control[] { lbl, cmb });
            currentY = cmb.Bottom + 10;
            return cmb;
        }
        
        public static Panel CrearPanelBusqueda(string labelText, EventHandler onBuscar, out TextBox txtBusqueda)
        {
            var panel = new Panel
            {
                Width = ANCHO_FORM,
                Height = 100,
                Padding = new Padding(50)
            };

            var lblBuscar = CrearLabel(labelText);
            lblBuscar.Location = new Point(50, 10);

            txtBusqueda = CrearTextBox(200);
            txtBusqueda.Location = new Point(50, lblBuscar.Bottom + 10);

            var btnBuscar = CrearBotonPrimario("Buscar", 100);
            btnBuscar.Location = new Point(txtBusqueda.Right + 10, lblBuscar.Bottom + 10);
            btnBuscar.Click += onBuscar;

            panel.Controls.AddRange(new Control[] { lblBuscar, txtBusqueda, btnBuscar });
            return panel;
        }

        public static Panel CrearPanelResultadoBusqueda(EventHandler onDeshabilitar, out Label lblResultado)
        {
            var panel = new Panel
            {
                Width = ANCHO_FORM,
                Height = 120,
                Visible = false,
                Padding = new Padding(50)
            };

            lblResultado = CrearLabel("");
            lblResultado.Location = new Point(50, 20);
            lblResultado.AutoSize = true;

            var btnDeshabilitar = CrearBotonPrimario("Deshabilitar Usuario", 200);
            btnDeshabilitar.Location = new Point(50, 60);
            btnDeshabilitar.BackColor = Color.FromArgb(192, 0, 0);
            btnDeshabilitar.Click += onDeshabilitar;
            btnDeshabilitar.Visible = false;

            panel.Controls.AddRange(new Control[] { lblResultado, btnDeshabilitar });
            return panel;
        }

        public static void MostrarContraseñaTemporal(string password)
        {
            var form = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Contraseña Temporal",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblPassword = new Label()
            {
                Text = $"Contraseña temporal:",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = FuenteNormal
            };

            var txtPassword = new TextBox()
            {
                Text = password,
                ReadOnly = true,
                Location = new Point(20, 50),
                Width = 340,
                Font = FuenteNormal
            };

            var btnCopiar = new Button()
            {
                Text = "COPIAR AL PORTAPAPELES",
                Location = new Point(20, 90),
                Width = 340,
                Height = 35,
                BackColor = ColorPrimario,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(FuenteNormal, FontStyle.Bold)
            };

            var btnCerrar = new Button()
            {
                Text = "CERRAR",
                DialogResult = DialogResult.OK,
                Location = new Point(20, 130),
                Width = 340,
                Height = 35,
                BackColor = Color.White,
                ForeColor = ColorPrimario,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(FuenteNormal, FontStyle.Bold)
            };

            btnCopiar.Click += (s, e) =>
            {
                try
                {
                    Clipboard.SetText(password);
                    txtPassword.ForeColor = Color.Green;
                    btnCopiar.Text = "¡COPIADO!";
                    btnCopiar.BackColor = Color.Green;
                }
                catch
                {
                    txtPassword.ForeColor = Color.Red;
                    btnCopiar.Text = "ERROR AL COPIAR";
                    btnCopiar.BackColor = Color.Red;
                }
            };

            form.Controls.AddRange(new Control[] { lblPassword, txtPassword, btnCopiar, btnCerrar });
            form.ShowDialog();
        }

        public static TextBox CrearTextBoxBusqueda(string placeholder = "Escriba para filtrar...")
        {
            var textBox = new TextBox
            {
                Width = 300,
                ForeColor = Color.Gray,
                Text = placeholder,
                Font = FuenteNormal
            };

            textBox.GotFocus += (s, e) => {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };

            AgregarEfectoFoco(textBox);

            return textBox;
        }
    }
}