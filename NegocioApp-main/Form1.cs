
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace NegocioApp
{
    public partial class frmLogin : Form
    {
        private List<ProductoVenta> productosVenta =
            new List<ProductoVenta>();

        private int indexEditar = -1;
        private bool ventaFinalizada = false;

        private class ProductoVenta
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public decimal Cantidad { get; set; }
            public decimal Precio { get; set; }
            public string Categoria { get; set; }
            public DateTime Fecha { get; set; }
            public DataGridViewRow FilaDgv { get; set; }

            public decimal Subtotal
            {
                get { return Cantidad * Precio; }
            }
        }

        public frmLogin()
        {
            InitializeComponent();
        }

        private decimal CalcularTotal()
        {
            decimal total = 0;

            foreach (ProductoVenta producto in productosVenta)
            {
                total += producto.Subtotal;
            }

            return total;
        }

        private string FormatearPrecio(decimal importe)
        {
            return "$ " + importe.ToString(
                "N2",
                new CultureInfo("es-AR")
            );
        }

        private bool IntentarConvertirDecimal(
            string texto,
            out decimal valor)
        {
            texto = texto.Trim().Replace(",", ".");

            return decimal.TryParse(
                texto,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out valor
            );
        }

        private bool ValidarCampos(
            out decimal cantidad,
            out decimal precio)
        {
            cantidad = 0;
            precio = 0;

            if (string.IsNullOrWhiteSpace(txt_Producto.Text))
            {
                MessageBox.Show("Ingresá el nombre del producto.");
                txt_Producto.Focus();
                return false;
            }

            if (!IntentarConvertirDecimal(
                txt_Cantidad.Text, out cantidad)
                || cantidad <= 0)
            {
                MessageBox.Show("Ingresá una cantidad válida.");
                txt_Cantidad.Focus();
                return false;
            }

            if (!IntentarConvertirDecimal(
                txt_Precio.Text, out precio)
                || precio <= 0)
            {
                MessageBox.Show("Ingresá un precio válido.");
                txt_Precio.Focus();
                return false;
            }

            return true;
        }

        private void btn_Agregar_Click(object sender, EventArgs e)
        {
            if (ventaFinalizada)
            {
                MessageBox.Show(
                    "La venta ya fue finalizada. Iniciá una nueva venta."
                );
                return;
            }

            decimal cantidad;
            decimal precio;

            if (!ValidarCampos(out cantidad, out precio))
                return;

            if (indexEditar != -1)
            {
                ProductoVenta producto = productosVenta[indexEditar];

                producto.Nombre = txt_Producto.Text.Trim();
                producto.Descripcion = txt_Descripcion.Text.Trim();
                producto.Cantidad = cantidad;
                producto.Precio = precio;
                producto.Categoria = string.IsNullOrWhiteSpace(
                    txt_Categoria.Text)
                    ? "Varios"
                    : txt_Categoria.Text.Trim();

                ActualizarFilaDgv(producto);

                indexEditar = -1;
                btn_Agregar.Text = "AGREGAR";
            }
            else
            {
                ProductoVenta producto = new ProductoVenta();

                producto.Nombre = txt_Producto.Text.Trim();
                producto.Descripcion = txt_Descripcion.Text.Trim();
                producto.Cantidad = cantidad;
                producto.Precio = precio;
                producto.Categoria = string.IsNullOrWhiteSpace(
                    txt_Categoria.Text)
                    ? "Varios"
                    : txt_Categoria.Text.Trim();

                producto.Fecha = DateTime.Now;

                int fila = dgv_Productos.Rows.Add(
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Cantidad,
                    FormatearPrecio(producto.Precio),
                    producto.Categoria,
                    producto.Fecha.ToShortDateString(),
                    producto.Fecha.ToShortTimeString()
                );

                producto.FilaDgv = dgv_Productos.Rows[fila];

                productosVenta.Add(producto);
            }

            ActualizarTicket();
            LimpiarCampos();
        }

        private void ActualizarFilaDgv(ProductoVenta producto)
        {
            if (producto.FilaDgv == null ||
                producto.FilaDgv.IsNewRow)
                return;

            producto.FilaDgv.Cells["Producto"].Value =
                producto.Nombre;

            producto.FilaDgv.Cells["Descripcion"].Value =
                producto.Descripcion;

            producto.FilaDgv.Cells["Cantidad"].Value =
                producto.Cantidad;

            producto.FilaDgv.Cells["Precio"].Value =
                FormatearPrecio(producto.Precio);

            producto.FilaDgv.Cells["Categoria"].Value =
                producto.Categoria;
        }

        private void ActualizarTicket()
        {
            TICKET.Items.Clear();

            foreach (ProductoVenta producto in productosVenta)
            {
                string linea =
                    producto.Nombre +
                    " | " + producto.Cantidad +
                    " x " + FormatearPrecio(producto.Precio) +
                    " = " + FormatearPrecio(producto.Subtotal);

                TICKET.Items.Add(linea);
            }
        }

        private void LimpiarCampos()
        {
            txt_Producto.Clear();
            txt_Descripcion.Clear();
            txt_Cantidad.Clear();
            txt_Precio.Clear();
            txt_Categoria.Text = "Varios";

            txt_Producto.Focus();
        }

        private void btn_Editar_Click(object sender, EventArgs e)
        {
            if (ventaFinalizada)
            {
                MessageBox.Show("La venta ya fue finalizada.");
                return;
            }

            int indice = TICKET.SelectedIndex;

            if (indice < 0 || indice >= productosVenta.Count)
            {
                MessageBox.Show(
                    "Seleccioná un producto del ticket."
                );
                return;
            }

            indexEditar = indice;

            ProductoVenta producto = productosVenta[indexEditar];

            txt_Producto.Text = producto.Nombre;
            txt_Descripcion.Text = producto.Descripcion;
            txt_Cantidad.Text = producto.Cantidad.ToString(
                CultureInfo.InvariantCulture);
            txt_Precio.Text = producto.Precio.ToString(
                CultureInfo.InvariantCulture);
            txt_Categoria.Text = producto.Categoria;

            btn_Agregar.Text = "GUARDAR CAMBIOS";

            txt_Producto.Focus();
        }

        private void btn_Eliminar_Click(object sender, EventArgs e)
        {
            if (ventaFinalizada)
            {
                MessageBox.Show("La venta ya fue finalizada.");
                return;
            }

            int indice = TICKET.SelectedIndex;

            if (indice < 0 || indice >= productosVenta.Count)
            {
                MessageBox.Show(
                    "Seleccioná un producto del ticket."
                );
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "¿Seguro que querés eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                ProductoVenta producto = productosVenta[indice];

                if (producto.FilaDgv != null &&
                    !producto.FilaDgv.IsNewRow)
                {
                    dgv_Productos.Rows.Remove(producto.FilaDgv);
                }

                productosVenta.RemoveAt(indice);

                indexEditar = -1;
                btn_Agregar.Text = "AGREGAR";

                LimpiarCampos();
                ActualizarTicket();
            }
        }

        private void btn_Finalizar_Click(object sender, EventArgs e)
        {
            if (ventaFinalizada)
            {
                MessageBox.Show("Esta venta ya fue finalizada.");
                return;
            }

            if (productosVenta.Count == 0)
            {
                MessageBox.Show(
                    "No hay productos para finalizar la venta."
                );
                return;
            }

            decimal total = CalcularTotal();

            ventaFinalizada = true;
            indexEditar = -1;

            btn_Agregar.Text = "AGREGAR";

            TICKET.Items.Add("--------------------------");
            TICKET.Items.Add("TOTAL: " + FormatearPrecio(total));

            MessageBox.Show(
                "¡Compra finalizada correctamente!\n\n" +
                "Total: " + FormatearPrecio(total),
                "Venta finalizada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btn_NuevaVenta_Click(object sender, EventArgs e)
        {
            productosVenta.Clear();

            TICKET.Items.Clear();

            indexEditar = -1;
            ventaFinalizada = false;

            btn_Agregar.Text = "AGREGAR";

            LimpiarCampos();

            MessageBox.Show("Nueva venta iniciada.");
        }

        private void txt_Producto_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Descripcion.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Descripcion_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Cantidad.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Cantidad_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Precio.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Precio_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Categoria.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Categoria_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Agregar.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Cantidad_KeyPress(
            object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) &&
                !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txt_Precio_KeyPress(
            object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) ||
                char.IsDigit(e.KeyChar))
                return;

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (txt_Precio.Text.Contains(".") ||
                    txt_Precio.Text.Contains(","))
                {
                    e.Handled = true;
                }

                return;
            }

            e.Handled = true;
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            LimpiarCampos();

            dgv_Productos.AllowUserToAddRows = false;
        }

        private void TICKET_SelectedIndexChanged(
            object sender, EventArgs e)
        {
        }

        private void txt_Producto_Leave(
            object sender, EventArgs e)
        {
        }
    }
}
