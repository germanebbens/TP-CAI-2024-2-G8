using System.Windows.Forms;


public class ApplicationManager
{
    private static ApplicationManager _instance;
    private Form _currentForm;

    private ApplicationManager() { }

    public static ApplicationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ApplicationManager();
            }
            return _instance;
        }
    }

    public void ShowNewForm(Form newForm)
    {
        // Si hay un formulario actual, lo ocultamos
        if (_currentForm != null && !_currentForm.IsDisposed)
        {
            _currentForm.Hide();
        }

        // Configuramos el nuevo formulario
        _currentForm = newForm;
        _currentForm.FormClosed += (s, e) =>
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        };

        // Mostramos el nuevo formulario
        _currentForm.Show();
    }
}