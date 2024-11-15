using ElectroHogar.Datos;
using ElectroHogar.Presentacion.Utils;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class RemitoForm : Form
    {
        public RemitoForm(VentaCompuesta venta, ClienteList cliente)
        {
            InitializeForm(venta, cliente);
        }

        private void InitializeForm(VentaCompuesta venta, ClienteList cliente)
        {
            this.Text = "Remito de Venta";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panel principal
            var panel = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Título
            var lblTitulo = new Label
            {
                Text = "ElectroHogar",
                Font = new Font(FormHelper.FuenteNormal.FontFamily, 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Fecha
            var lblFecha = new Label
            {
                Text = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}",
                Location = new Point(20, lblTitulo.Bottom + 10),
                AutoSize = true
            };

            // Datos del cliente
            var lblClienteTitulo = new Label
            {
                Text = "Datos del Cliente:",
                Font = new Font(FormHelper.FuenteNormal.FontFamily, 10, FontStyle.Bold),
                Location = new Point(20, lblFecha.Bottom + 20),
                AutoSize = true
            };

            var lblClienteInfo = new Label
            {
                Text = $"{cliente.Nombre} {cliente.Apellido}\n" +
                      $"DNI: {cliente.Dni}\n" +
                      $"Dirección: {cliente.Direccion}\n" +
                      $"Teléfono: {cliente.Telefono}",
                Location = new Point(20, lblClienteTitulo.Bottom + 5),
                AutoSize = true
            };

            // Grilla
            var lblDetalle = new Label
            {
                Text = "Detalle:",
                Font = new Font(FormHelper.FuenteNormal.FontFamily, 10, FontStyle.Bold),
                Location = new Point(20, lblClienteInfo.Bottom + 40),
                AutoSize = true
            };

            var dgvProductos = new DataGridView
            {
                Location = new Point(20, lblDetalle.Bottom + 10),
                Width = 740,
                Height = 200,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font(FormHelper.FuenteNormal.FontFamily, 9, FontStyle.Bold),
                    BackColor = Color.LightGray
                }
            };

            dgvProductos.Columns.AddRange(new[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "ID",
                    DataPropertyName = "IdProducto",
                    Width = 140
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Descripcion",
                    HeaderText = "Descripción",
                    DataPropertyName = "NombreProducto",
                    Width = 200
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Cantidad",
                    HeaderText = "Cantidad",
                    DataPropertyName = "Cantidad",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "MontoUnitario",
                    HeaderText = "Monto Unitario",
                    DataPropertyName = "Precio",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "MontoTotal",
                    HeaderText = "Monto Total",
                    DataPropertyName = "Subtotal",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                }
            });

            dgvProductos.DataSource = venta.Items;

            // Descuentos
            var lblPromociones = new Label
            {
                Text = "Promociones:",
                Font = new Font(FormHelper.FuenteNormal.FontFamily, 10, FontStyle.Bold),
                Location = new Point(20, dgvProductos.Bottom + 20),
                AutoSize = true
            };

            var lstDescuentos = new ListBox
            {
                Location = new Point(20, lblPromociones.Bottom + 5),
                Width = 740,
                Height = 60,
                BorderStyle = BorderStyle.FixedSingle
            };

            foreach (var descuento in venta.Descuentos)
            {
                lstDescuentos.Items.Add(descuento);
            }

            // Total
            var lblSubtotal = new Label
            {
                Text = $"Subtotal: ${venta.Items.Sum(i => i.Subtotal):N2}",
                Font = new Font(FormHelper.FuenteNormal.FontFamily, 10, FontStyle.Bold),
                Location = new Point(580, lstDescuentos.Bottom + 20),
                AutoSize = true
            };

            var lblTotal = new Label
            {
                Text = $"Total a pagar: ${venta.MontoTotal:N2}",
                Font = new Font(FormHelper.FuenteNormal.FontFamily, 10, FontStyle.Bold),
                Location = new Point(580, lblSubtotal.Bottom + 20),
                AutoSize = true
            };

            // Botón cerrar
            var btnCerrar = FormHelper.CrearBotonPrimario("CERRAR", 100);
            btnCerrar.Location = new Point(350, lblTotal.Bottom + 20);
            btnCerrar.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[] {
                lblTitulo,
                lblFecha,
                lblClienteTitulo,
                lblClienteInfo,
                lblDetalle,
                dgvProductos,
                lblPromociones,
                lstDescuentos,
                lblSubtotal,
                lblTotal,
                btnCerrar
            });

            this.Controls.Add(panel);
        }
    }
}
