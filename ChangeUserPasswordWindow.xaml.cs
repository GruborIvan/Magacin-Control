using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using System;
using System.Windows;

namespace CSS_MagacinControl_App
{
    /// <summary>
    /// Interaction logic for ChangeUserPassword.xaml
    /// </summary>
    public partial class ChangeUserPasswordWindow : Window
    {
        private Guid userIdentifier;
        private readonly DialogHandler dialogHandler;

        private readonly IAuthenticationRepository _authenticationRepository;

        public ChangeUserPasswordWindow(Guid userIdentifier, IAuthenticationRepository authenticationRepository)
        {
            InitializeComponent();
            _authenticationRepository = authenticationRepository;
            this.userIdentifier = userIdentifier;
            dialogHandler = new DialogHandler();
            PasswordBox1.Focus();
        }

        private async void PromenaLozinkeButton_Click(object sender, RoutedEventArgs e)
        {
            string passwordUnos1 = PasswordBox1.Password;
            string passwordUnos2 = PasswordBox2.Password;

            if (passwordUnos1 != passwordUnos2)
            {
                dialogHandler.GetRazliciteSifre_PromenaSifreDialog();
            }

            await _authenticationRepository.ChangeUserPasswordAsync(userIdentifier, passwordUnos1);
            dialogHandler.GetUspesnoPromenjenaLozinkaKorisnikaDialog();

            PasswordBox1.Password = string.Empty;
            PasswordBox2.Password = string.Empty;

            this.Close();
        }
    }
}