﻿using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class VentasPorVendedorForm : Form
    {
        private readonly Usuarios _usuarioService;
        private readonly Label lblEstado;
        private DataGridView dgvVentas;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;

        public VentasPorVendedorForm()
        {
            InitializeComponent();
            _usuarioService = new Usuarios();
            lblEstado = FormHelper.CrearLabelEstado();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Reporte de Ventas por Vendedor";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 600);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Reporte de Ventas por Vendedor");
            this.Controls.Add(panelSuperior);

            var lblNota = new Label
            {
                Text = "Nota: Los datos de ventas mostrados en este reporte son generados aleatoriamente, \nno se cuenta con un método en el servicio web que devuelva esta información.",
                AutoSize = true,
                Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 10),
                Font = new Font(FormHelper.FuenteNormal, FontStyle.Regular)
            };
            this.Controls.Add(lblNota);

            var lblDesde = FormHelper.CrearLabel("Desde:");
            lblDesde.Location = new Point(FormHelper.MARGEN, lblNota.Bottom + 10);

            dtpDesde = new DateTimePicker
            {
                Location = new Point(lblDesde.Right + FormHelper.MARGEN, lblDesde.Top),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            var lblHasta = FormHelper.CrearLabel("Hasta:");
            lblHasta.Location = new Point(dtpDesde.Right + FormHelper.MARGEN, lblDesde.Top);

            dtpHasta = new DateTimePicker
            {
                Location = new Point(lblHasta.Right + FormHelper.MARGEN, lblHasta.Top),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            var btnGenerar = FormHelper.CrearBotonPrimario("Generar Reporte", 150);
            btnGenerar.Location = new Point(FormHelper.MARGEN, lblHasta.Bottom + FormHelper.MARGEN);
            btnGenerar.Click += BtnGenerar_Click;

            dgvVentas = new DataGridView
            {
                Location = new Point(FormHelper.MARGEN, btnGenerar.Bottom + 10),
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Height = 400,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvVentas.Columns.AddRange(new DataGridViewColumn[]
            {
            new DataGridViewTextBoxColumn { Name = "Vendedor", HeaderText = "Vendedor", DataPropertyName = "Nombre", Width = 150 },
            new DataGridViewTextBoxColumn { Name = "CantidadVentas", HeaderText = "Cantidad de Ventas", DataPropertyName = "CantidadVentas", Width = 150 },
            new DataGridViewTextBoxColumn { Name = "MontoTotal", HeaderText = "Monto Total", DataPropertyName = "MontoTotal", Width = 150 }
            });

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, dgvVentas.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);

            this.Controls.AddRange(new Control[] {
            panelSuperior,
            lblNota,
            lblDesde, dtpDesde,
            lblHasta, dtpHasta,
            btnGenerar,
            dgvVentas,
            btnVolver,
            lblEstado
        });
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                var vendedores = _usuarioService.ObtenerVendedoresActivos();
                dgvVentas.DataSource = vendedores;
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }
    }
}