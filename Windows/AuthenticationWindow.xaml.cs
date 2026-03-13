using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CSS_MagacinControl_App
{
    public partial class AuthenticationWindow : Window
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IRobaService _robaService;
        private readonly IFileParser _fileParser;
        private readonly DialogHandler dialogHandler;
        private readonly IServiceProvider _serviceProvider;

        public AuthenticationWindow(IServiceProvider serviceProvider, IAuthenticationRepository authenticationRepository, IRobaService robaService, IFileParser fileParser)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _authenticationRepository = authenticationRepository;
            _robaService = robaService;
            _fileParser = fileParser;
            dialogHandler = new DialogHandler();

            UserBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            var username = UserBox.Text;
            var pass = PassBox.Password;
            bool suceeded, isAdmin;

            (suceeded, isAdmin) = await _authenticationRepository.AuthenticateToSytemAsync(username, pass);

            if (suceeded)
            {
                App.Current.Properties["IsAdmin"] = isAdmin;
                App.Current.Properties["Username"] = username;

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.WindowState = WindowState.Maximized;
                mainWindow.Show();
                this.Hide();
            }
            else
            {
                dialogHandler.GetFailedUsernamePassDialog();
                UserBox.Text = string.Empty;
                PassBox.Password = string.Empty;
                UserBox.Focus();
            }
            LoginButton.IsEnabled = true; // Must make this because of mutex lock.
        }
    }
}