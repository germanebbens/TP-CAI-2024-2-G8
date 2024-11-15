using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class VentasManagerForm : Form
    {
        private readonly Ventas _ventasService;
        private readonly Label lblEstado;
        private readonly Panel panelCliente;
        private readonly Panel panelProductos;
        private readonly Panel panelResumen;
        private ClienteList _clienteSeleccionado;
        private List<ItemVenta> _items = new List<ItemVenta>();
        private readonly int ANCHO;
        private VentaCompuesta _venta;

        public VentasManagerForm()
        {
            _venta = new VentaCompuesta();
            ANCHO = FormHelper.ANCHO_FORM + 50;
            InitializeComponent();
            _ventasService = new Ventas();
            lblEstado = FormHelper.CrearLabelEstado();
            panelCliente = CrearPanelCliente();
            panelProductos = CrearPanelProductos();
            panelResumen = CrearPanelResumen();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Nueva Venta";
            this.ClientSize = new Size(ANCHO, 800);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Nueva Venta");
            this.Controls.Add(panelSuperior);

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            panelCliente.Location = new Point(0, btnVolver.Bottom + 10);
            panelProductos.Location = new Point(0, panelCliente.Bottom + 40);
            panelResumen.Location = new Point(0, panelProductos.Bottom + 10);

            this.Controls.AddRange(new Control[] {
                btnVolver,
                panelCliente,
                panelProductos,
                panelResumen
            });
        }

        private Panel CrearPanelCliente()
        {
            var panel = new Panel
            {
                Name = "panelCliente",
                Width = ANCHO,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Título
            var lblTitulo = FormHelper.CrearLabel("Datos del Cliente");
            lblTitulo.Font = new Font(lblTitulo.Font, FontStyle.Bold);
            lblTitulo.Location = new Point(FormHelper.MARGEN, 10);
            lblTitulo.MaximumSize = new Size(150, 0);

            // Botón seleccionar cliente
            var btnSeleccionarCliente = FormHelper.CrearBotonPrimario("Seleccionar Cliente", 200);
            btnSeleccionarCliente.Location = new Point(300, 10);
            btnSeleccionarCliente.Click += (s, e) => SeleccionarCliente();

            // Panel de datos del cliente
            var panelDatos = new Panel
            {
                Name = "panelDatosCliente",
                Width = ANCHO - (FormHelper.MARGEN * 2),
                Location = new Point(FormHelper.MARGEN, btnSeleccionarCliente.Bottom + 10),
                AutoSize = true
            };
            int currentY = 10;

            // Labels para mostrar datos (readonly)
            var lblClienteInfo = new Label
            {
                Name = "lblClienteInfo",
                Text = "Ningún cliente seleccionado",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true,
                Font = new Font(FormHelper.FuenteNormal, FontStyle.Bold)
            };
            currentY = lblClienteInfo.Bottom + 10;

            var lblDireccion = new Label
            {
                Name = "lblDireccion",
                Text = "",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };
            currentY = lblDireccion.Bottom + 5;

            var lblTelefono = new Label
            {
                Name = "lblTelefono",
                Text = "",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };
            currentY = lblTelefono.Bottom + 5;

            var lblEmail = new Label
            {
                Name = "lblEmail",
                Text = "",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };

            panelDatos.Controls.AddRange(new Control[] {
                lblClienteInfo,
                lblDireccion,
                lblTelefono,
                lblEmail
            });
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(btnSeleccionarCliente);
            panel.Controls.Add(panelDatos);
            return panel;
        }

        private void SeleccionarCliente()
        {
            var config = new ClienteSeleccionListadoConfig(cliente =>
            {
                _clienteSeleccionado = cliente;
            });

            var formClientes = new BaseListForm(config);
            if (formClientes.ShowDialog() == DialogResult.OK)
            {
                ActualizarDatosCliente();
                ActualizarResumen();
            }
        }

        private void ActualizarDatosCliente()
        {
            var panelDatos = (Panel)panelCliente.Controls["panelDatosCliente"];

            if (_clienteSeleccionado != null)
            {
                var lblClienteInfo = (Label)panelDatos.Controls["lblClienteInfo"];
                var lblDireccion = (Label)panelDatos.Controls["lblDireccion"];
                var lblTelefono = (Label)panelDatos.Controls["lblTelefono"];
                var lblEmail = (Label)panelDatos.Controls["lblEmail"];

                lblClienteInfo.Text = $"{_clienteSeleccionado.Nombre} {_clienteSeleccionado.Apellido} (DNI: {_clienteSeleccionado.Dni})";
                lblDireccion.Text = $"Dirección: {_clienteSeleccionado.Direccion}";
                lblTelefono.Text = $"Teléfono: {_clienteSeleccionado.Telefono}";
                lblEmail.Text = $"Email: {_clienteSeleccionado.Email}";

            }
        }

        private Panel CrearPanelProductos()
        {
            var panel = new Panel
            {
                Width = ANCHO,
                Height = 350
            };

            // Título
            var lblTitulo = FormHelper.CrearLabel("Productos");
            lblTitulo.Font = new Font(lblTitulo.Font, FontStyle.Bold);
            lblTitulo.Location = new Point(20, 10);

            // Botón
            var btnAgregarProducto = FormHelper.CrearBotonPrimario("+ Agregar Producto", 200);
            btnAgregarProducto.Name = "btnAgregarProducto";
            btnAgregarProducto.Location = new Point(20, lblTitulo.Bottom + 10);
            btnAgregarProducto.Click += (s, e) => SeleccionarProducto();

            // Grid
            var dgvProductos = new DataGridView
            {
                Name = "dgvProductos",
                Location = new Point(20, btnAgregarProducto.Bottom + 10),
                Width = panel.Width - 40,
                Height = 200,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvProductos.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "Producto",
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
                    Name = "Precio",
                    HeaderText = "Precio",
                    DataPropertyName = "Precio",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Subtotal",
                    HeaderText = "Subtotal",
                    DataPropertyName = "Subtotal",
                    Width = 100
                },
                new DataGridViewButtonColumn
                {
                    Name = "Quitar",
                    HeaderText = "Acción",
                    Text = "Quitar",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                }
            });

            dgvProductos.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvProductos.Columns["Quitar"].Index)
                {
                    _items.RemoveAt(e.RowIndex);
                    ActualizarGridProductos();
                    ActualizarResumen();
                }
            };

            panel.Controls.AddRange(new Control[] { lblTitulo, btnAgregarProducto, dgvProductos });
            return panel;
        }

        private Panel CrearPanelResumen()
        {
            var panel = new Panel
            {
                Width = ANCHO,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Subtotal
            var lblSubtotal = FormHelper.CrearLabel("Subtotal:");
            lblSubtotal.Font = new Font(lblSubtotal.Font, FontStyle.Bold);
            lblSubtotal.Location = new Point(FormHelper.MARGEN, 10);

            var lblSubtotalValue = FormHelper.CrearLabel("$0.00");
            lblSubtotalValue.Name = "lblSubtotal";
            lblSubtotalValue.Location = new Point(lblSubtotal.Right + FormHelper.MARGEN, 10);

            // Discounts
            var lblDescuentos = FormHelper.CrearLabel("Descuentos:");
            lblDescuentos.Font = new Font(lblDescuentos.Font, FontStyle.Bold);
            lblDescuentos.Location = new Point(FormHelper.MARGEN, lblSubtotal.Bottom + 10);

            var lstDescuentos = new ListBox
            {
                Name = "lstDescuentos",
                Location = new Point(FormHelper.MARGEN, lblDescuentos.Bottom + 5),
                Width = ANCHO - (FormHelper.MARGEN * 2),
                Height = 60
            };

            // Total
            var lblTotal = FormHelper.CrearLabel("Total:");
            lblTotal.Font = new Font(lblTotal.Font, FontStyle.Bold);
            lblTotal.Location = new Point(FormHelper.MARGEN, lstDescuentos.Bottom + 10);

            var lblTotalValue = FormHelper.CrearLabel("$0.00");
            lblTotalValue.Name = "lblTotal";
            lblTotalValue.Location = new Point(lblTotal.Right + FormHelper.MARGEN, lblTotal.Top);

            // Confirm Button
            var btnConfirmar = FormHelper.CrearBotonPrimario("Confirmar Venta", ANCHO - (FormHelper.MARGEN * 2));
            btnConfirmar.Location = new Point(FormHelper.MARGEN, lblTotalValue.Bottom + 20);
            btnConfirmar.Click += (s, e) => ConfirmarVenta();

            // Estado Label
            lblEstado.Location = new Point(FormHelper.MARGEN, btnConfirmar.Bottom + FormHelper.MARGEN);

            panel.Controls.AddRange(new Control[] {
                lblSubtotal, lblSubtotalValue,
                lblDescuentos, lstDescuentos,
                lblTotal, lblTotalValue,
                btnConfirmar, lblEstado
            });

            return panel;
        }

        private void SeleccionarProducto()
        {
            if (_items.Count >= 10)
            {
                MessageBox.Show("No se pueden agregar más de 10 productos", "Límite alcanzado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var config = new ProductoSeleccionListadoConfig(producto => {
                if (producto != null)
                {
                    var cantidadForm = new Form
                    {
                        Text = "Cantidad",
                        Size = new Size(300, 150),
                        StartPosition = FormStartPosition.CenterScreen,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var numCantidad = new NumericUpDown
                    {
                        Location = new Point(20, 20),
                        Width = 250,
                        Minimum = 1,
                        Maximum = 99999999999,
                        Value = 1
                    };

                    var btnAceptar = new Button
                    {
                        Text = "Aceptar",
                        Location = new Point(20, 60),
                        DialogResult = DialogResult.OK
                    };

                    cantidadForm.Controls.AddRange(new Control[] { numCantidad, btnAceptar });

                    if (cantidadForm.ShowDialog() == DialogResult.OK)
                    {
                        int cantidadSolicitada = (int)numCantidad.Value;
                        int cantidadFinal = cantidadSolicitada;

                        if (cantidadSolicitada > producto.Stock)
                        {
                            cantidadFinal = producto.Stock;
                            MessageBox.Show(
                                $"No hay suficiente stock. Se agregarán {cantidadFinal} unidades que es el máximo disponible.",
                                "Stock limitado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }

                        var item = new ItemVenta
                        {
                            IdProducto = producto.Id,
                            NombreProducto = producto.Nombre,
                            Cantidad = cantidadFinal,
                            Precio = producto.Precio,
                            Categoria = producto.IdCategoria
                        };

                        _items.Add(item);
                        ActualizarGridProductos();
                        ActualizarResumen();
                    }
                }
            });

            var formProductos = new BaseListForm(config);
            formProductos.ShowDialog();
        }

        private void ActualizarGridProductos()
        {
            var dgvProductos = (DataGridView)panelProductos.Controls["dgvProductos"];
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = _items;

            // Actualizar visibilidad del botón Agregar Producto
            var btnAgregarProducto = (Button)panelProductos.Controls["btnAgregarProducto"];
            btnAgregarProducto.Visible = _items.Count < 10;
        }

        private void ActualizarResumen()
        {
            var lstDescuentos = (ListBox)panelResumen.Controls["lstDescuentos"];
            var lblSubtotal = (Label)panelResumen.Controls["lblSubtotal"];
            var lblDescuentos = (Label)panelResumen.Controls["lblDescuentos"];
            var lblTotal = (Label)panelResumen.Controls["lblTotal"];

            lstDescuentos.Items.Clear();
            _venta.Descuentos.Clear();
            double totalDescuento = 0;

            // Electro Hogar Discount
            var montoElectroHogar = _items
                .Where(i => i.Categoria == Categoria.ElectroHogar)
                .Sum(i => i.Subtotal);

            if (montoElectroHogar > 100000 && !_venta.Descuentos.Contains("5% Descuento Electro Hogar"))
            {
                var descuentoElectroHogar = montoElectroHogar * 0.05;
                var textoDescuento = $"5% Descuento Electro Hogar: ${descuentoElectroHogar:N2}";
                lstDescuentos.Items.Add(textoDescuento);
                _venta.Descuentos.Add(textoDescuento);
                totalDescuento += descuentoElectroHogar;
            }

            // New Customer Discount
            if (_clienteSeleccionado != null && !_venta.Descuentos.Contains("5% Descuento Cliente Nuevo"))
            {
                var clientesService = new Clientes();
                if (clientesService.EsClienteNuevo(_clienteSeleccionado.Id))
                {
                    var montoTotal = _items.Sum(i => i.Subtotal);
                    var descuentoClienteNuevo = montoTotal * 0.05;
                    var textoDescuento = $"5% Descuento Cliente Nuevo: ${descuentoClienteNuevo:N2}";
                    lstDescuentos.Items.Add(textoDescuento);
                    _venta.Descuentos.Add(textoDescuento);
                    totalDescuento += descuentoClienteNuevo;
                }
            }

            double subtotal = _items.Sum(i => i.Subtotal);
            double total = subtotal - totalDescuento;

            // Update labels
            lblSubtotal.Text = $"Subtotal: ${subtotal:N2}";
            lblTotal.Text = $"Total: ${total:N2}";

            // Update venta
            _venta.Subtotal = subtotal;
            _venta.TotalDescuentos = totalDescuento;
            _venta.MontoTotal = total;
            _venta.Items = new List<ItemVenta>(_items);
        }
        
        private void ConfirmarVenta()
        {
            try
            {
                if (_clienteSeleccionado == null)
                    throw new Exception("Debe seleccionar un cliente");

                if (_items.Count == 0)
                    throw new Exception("Debe agregar al menos un producto");

                if (MessageBox.Show("¿Confirma la venta?", "Confirmar Venta",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _venta.IdCliente = _clienteSeleccionado.Id;
                _ventasService.RegistrarVenta(_venta);
                FormHelper.MostrarEstado(lblEstado, "Venta registrada exitosamente", false);
                var remitoForm = new RemitoForm(_venta, _clienteSeleccionado);
                remitoForm.ShowDialog();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void LimpiarFormulario()
        {
            _clienteSeleccionado = null;
            _items.Clear();

            // Actualizar los labels del cliente en lugar de ocultar el panel
            var panelDatos = (Panel)panelCliente.Controls["panelDatosCliente"];
            var lblClienteInfo = (Label)panelDatos.Controls["lblClienteInfo"];
            var lblDireccion = (Label)panelDatos.Controls["lblDireccion"];
            var lblTelefono = (Label)panelDatos.Controls["lblTelefono"];
            var lblEmail = (Label)panelDatos.Controls["lblEmail"];

            lblClienteInfo.Text = "Ningún cliente seleccionado";
            lblDireccion.Text = "";
            lblTelefono.Text = "";
            lblEmail.Text = "";

            ActualizarGridProductos();
            ActualizarResumen();
        }
    }
}
