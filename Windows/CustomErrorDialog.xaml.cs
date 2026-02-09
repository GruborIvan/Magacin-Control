using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace CSS_PA_Otprema.Windows
{
    /// <summary>
    /// Interaction logic for CustomErrorDialog.xaml
    /// </summary>
    public partial class CustomErrorDialog : Window
    {
        private bool _allowClose;

        public string Caption { get; }
        public string Message { get; }
        public string ButtonText { get; }

        public CustomErrorDialog(
            Window owner,
            string caption,
            string message,
            string buttonText = "OK")
        {
            InitializeComponent();

            Owner = owner;
            Caption = caption;
            Message = message;
            ButtonText = buttonText;

            SystemSounds.Exclamation.Play();
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _allowClose = true;
            DialogResult = true;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter or Key.Escape or Key.Space)
                e.Handled = true;
        }

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            if (!_allowClose)
                e.Cancel = true;
        }
    }
}
