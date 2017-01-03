using System.Text.RegularExpressions;
using System.Windows;

namespace MediaKeyTimer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Regex _isNumeric = new Regex("[0-9]");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !_isNumeric.IsMatch(e.Text);
        }
    }
}
