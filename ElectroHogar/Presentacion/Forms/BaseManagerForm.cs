using ElectroHogar.Presentacion.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Forms
{
    public abstract partial class BaseManagerForm : Form
    {
        protected readonly Label lblEstado;
        protected readonly Panel panelAlta;
        private bool acordeonAbierto = false;
        protected readonly object _service;

        protected BaseManagerForm(object service)
        {
            InitializeComponent();
            _service = service;
            lblEstado = FormHelper.CrearLabelEstado();
            panelAlta = CrearPanelAlta();
            ConfigurarFormulario();
        }

        protected virtual void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = $"ElectroHogar - {GetTitulo()}";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 600);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior(GetTitulo());
            this.Controls.Add(panelSuperior);

            // Botón Ver Items Activos
            var btnItemsActivos = FormHelper.CrearBotonPrimario($"Ver {GetTituloItems()} Activos");
            btnItemsActivos.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 20);
            btnItemsActivos.Click += (s, e) => MostrarListado();

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(btnItemsActivos.Right + 20, panelSuperior.Bottom + 20);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnItemsActivos.Bottom + 10);
            panelAlta.Location = new Point(0, lblEstado.Bottom + 10);

            this.Controls.AddRange(new Control[] {
            btnVolver,
            btnItemsActivos,
            lblEstado,
            panelAlta
        });
        }

        private Panel CrearPanelAlta()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Header del acordeón
            var btnAcordeon = FormHelper.CrearBotonPrimario($"+ Agregar Nuevo {GetTituloItem()}", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2));
            btnAcordeon.Location = new Point(FormHelper.MARGEN, 10);
            btnAcordeon.Click += (s, e) => ToggleAcordeon();

            var panelContenido = new Panel
            {
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Location = new Point(FormHelper.MARGEN, btnAcordeon.Bottom + 10),
                AutoSize = true,
                Visible = false
            };

            CrearCamposFormulario(panelContenido);

            // Botón guardar
            var btnGuardar = FormHelper.CrearBotonPrimario($"Guardar {GetTituloItem()}", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 3));
            btnGuardar.Location = new Point(FormHelper.MARGEN, panelContenido.Controls[panelContenido.Controls.Count - 1].Bottom + 20);
            btnGuardar.Click += (s, e) => Guardar();
            panelContenido.Controls.Add(btnGuardar);

            panel.Controls.AddRange(new Control[] { btnAcordeon, panelContenido });
            return panel;
        }

        protected void ToggleAcordeon()
        {
            acordeonAbierto = !acordeonAbierto;
            panelAlta.Controls[1].Visible = acordeonAbierto;
            var btnAcordeon = (Button)panelAlta.Controls[0];
            btnAcordeon.Text = acordeonAbierto ? "- Cerrar" : $"+ Agregar Nuevo {GetTituloItem()}";
        }

        protected void LimpiarFormulario()
        {
            var panelContenido = (Panel)panelAlta.Controls[1];

            foreach (Control control in panelContenido.Controls)
            {
                if (control is TextBox textBox)
                    textBox.Text = string.Empty;
                else if (control is DateTimePicker dateTimePicker)
                    dateTimePicker.Value = DateTime.Today;
                else if (control is ComboBox comboBox)
                    comboBox.SelectedIndex = -1;
            }
        }

        // Métodos abstractos que deben implementar las clases derivadas
        protected abstract string GetTitulo();
        protected abstract string GetTituloItems();
        protected abstract string GetTituloItem();
        protected abstract void CrearCamposFormulario(Panel panelContenido);
        protected abstract void Guardar();
        protected abstract void MostrarListado();
    }
}
