using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TemplateTPIntegrador
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            String usuario = txtUsuario.Text;
            String password = txtPassword.Text;

            try
            {
                LoginNegocio loginNegocio = new LoginNegocio();
                loginNegocio.login(usuario, password);

                Form form = new InitForm();
                this.Hide();
                form.ShowDialog();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
