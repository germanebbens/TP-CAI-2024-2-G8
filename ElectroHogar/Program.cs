using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectroHogar.Presentacion.Forms;

namespace ElectroHogar
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Iniciamos con el LoginForm
            var loginForm = new LoginForm();
            ApplicationManager.Instance.ShowNewForm(loginForm);

            // Mantenemos la aplicación corriendo
            Application.Run();
        }
    }
}
