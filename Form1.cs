using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NegocioApp
{
    public partial class frmLogin : Form
    {
        private int indexEditar = -1;

        private decimal totalGeneral = 0;
        public frmLogin()
        {
            InitializeComponent();
        }
        private void btn_Agregar_Click(object sender, EventArgs e)
        {
            //calcular total
            decimal cantidad = Convert.ToDecimal(txt_Cantidad.Text);
            decimal precio = Convert.ToDecimal(txt_Precio.Text);
            decimal total = cantidad * precio;

            //sumar al total general
            totalGeneral += total;

            //agregar al ticket
            string linea = txt_Producto.Text + " | " + cantidad + " | $" + precio + " | $" + total;

            TICKET.Items.Add(linea);

            if (indexEditar != -1)
            {
                TICKET.Items[indexEditar] = linea;
                indexEditar = -1;
            }
            else
            {
                //agregar al dgv
                dgv_Productos.Rows.Add(
                    txt_Producto.Text,
                    txt_Descripcion.Text,
                    txt_Cantidad.Text,
                    txt_Precio.Text,
                    txt_Categoria.Text,
                    DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToShortTimeString()
                    );
            }

            //limpiar campos
            txt_Producto.Clear();
            txt_Descripcion.Clear();
            txt_Cantidad.Clear();
            txt_Precio.Clear();
            txt_Categoria.Clear();

            //volver al primer campo
            txt_Producto.Focus();
        }

        private void txt_Producto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Descripcion.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Descripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Cantidad.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Cantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Precio.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Precio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_Categoria.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txt_Categoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Agregar.Focus();
                e.SuppressKeyPress = true;
            }
        }
        private void TICKET_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_Finalizar_Click(object sender, EventArgs e)
        {
            string fecha = DateTime.Now.ToShortDateString();
            string hora = DateTime.Now.ToShortTimeString();
            dgv_Productos.Rows.Add(fecha, hora);
            
            //decimal total = 0;
            foreach (var item in TICKET.Items)
            {
                string texto = item.ToString();
                string[] partes = item.ToString().Split('=');

                if (partes.Length > 1)
                {
                    decimal subtotal;
                    if (decimal.TryParse(partes[1], out subtotal))
                    {
                        totalGeneral += subtotal;
                    }
                }
            }
            MessageBox.Show("Compra Finalizada. total: " + totalGeneral);

            //agregar TOTAL al ticket (abajo de todo)
            TICKET.Items.Add("____________________________");
            TICKET.Items.Add("TOTAL: $" + totalGeneral);
        }

        private void btn_NuevaVenta_Click(object sender, EventArgs e)
        {
            //limpiar ticket
            TICKET.Items.Clear();

            //limpiar campos
            txt_Producto.Clear();
            txt_Descripcion.Clear();
            txt_Cantidad.Clear();
            txt_Precio.Clear();
            txt_Categoria.Clear();

            //volver al primer campo
            txt_Producto.Focus();

            MessageBox.Show("Nueva venta iniciada");
        }

        private void btn_Eliminar_Click(object sender, EventArgs e)
        {
            //verificar que haya algo seleccionado
            if (TICKET.SelectedIndex != -1)
            {
                //preguntar confirmacion
                DialogResult resultado = MessageBox.Show(
                    "¿Seguro que queres eliminar este producto?", "Confirmar eliminacion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );
                //si elige SI
                if (resultado == DialogResult.Yes)
                {
                    int index = TICKET.SelectedIndex;
                    string item = TICKET.Items[index].ToString();

                    //restar del total
                    string[] partes = item.Split('=');
                    if (partes.Length > 1)
                    {
                        decimal subtotal;
                        if (decimal.TryParse(partes[1], out subtotal))
                        {
                            totalGeneral -= subtotal;
                        }   
                    }
                    TICKET.Items.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto del ticket");
            }
        }

        private void btn_Editar_Click(object sender, EventArgs e)
        {
            if (TICKET.SelectedIndex != -1)
            {
                indexEditar = TICKET.SelectedIndex;
                string item = TICKET.SelectedItem.ToString();

                string[] partes = item.Split('-');

                txt_Producto.Text = partes[0].Trim();

                string[] datos = partes[1].Split('x');

                txt_Cantidad.Text = datos[0].Trim();
                txt_Precio.Text = datos[1].Split('=')[0].Trim();
            }
            else
            {
                MessageBox.Show("Selecciona un producto del ticket");
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            txt_Producto.Text = "Producto";
            txt_Descripcion.Text = "Descripcion";
            txt_Cantidad.Text = "Cantidad";
            txt_Precio.Text = " ";
            txt_Categoria.Text = "Varios";
        }

        private void txt_Producto_Leave(object sender, EventArgs e)
        {
            if (txt_Producto.Text == " ")
            {
                txt_Producto.Text = "Producto";
            }
        }

        private void txt_Cantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txt_Precio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
    }
}
