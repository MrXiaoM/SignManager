using System.Windows;

namespace SignManager
{
    /// <summary>
    /// WindowInput.xaml 的交互逻辑
    /// </summary>
    public partial class WindowInput : Window
    {
        public string result = "";
        public WindowInput(string title, string info, string btnText, string value = "")
        {
            InitializeComponent();
            Title = title;
            TxtInfo.Text = info;
            TxtValue.Text = value;
            BtnConfirm.Content = btnText;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            result = TxtValue.Text;
            DialogResult = true;
            Close();
        }
    }
}
