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
        if (_currentForm != null && !_currentForm.IsDisposed)
        {
            _currentForm.Hide();
        }

        _currentForm = newForm;
        _currentForm.FormClosed += (s, e) =>
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        };

        _currentForm.Show();
    }
}