using System.Windows;
using System.Windows.Controls;
using NotationTB.BusinessLogic;
//using NotationTB.SqlTables;

namespace NotationTB;

/// <summary>
///     Логика взаимодействия для LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    //private readonly NotationTbContext db = new();

    public LoginWindow()
    {
        InitializeComponent();
#if DEBUG
        
        LoginTextBox.Text = "test";
        PasswordBox.Password = "test";

        var mainWindow = new MainWindow(0);
        mainWindow.Show();
        Close();
#endif
    }
    /// <summary>
    /// обработчик события нажатия кнопки авторизация
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(0);
        mainWindow.Show();
        Close();


        var login = LoginTextBox.Text;
        var password = PasswordBox.Password;
        string message;
        int id;
        using (UserLogic userLogic = new())
        {
            if (userLogic.Authorization(login, password, out id, out message))
            {
                mainWindow = new MainWindow(id);
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show
                (
                    message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}