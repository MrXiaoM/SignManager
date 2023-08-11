using System.Windows;
using System.Windows.Controls;

namespace SignManager
{
    /// <summary>
    /// WindowUnidbgFetchQSign.xaml 的交互逻辑
    /// </summary>
    public partial class WindowUnidbgFetchQSign : Window
    {
        private string ver;
        private string configFile;
        private UnidbgFetchQSignConfig config;
        public WindowUnidbgFetchQSign(string configFile, UnidbgFetchQSignConfig config, string ver)
        {
            InitializeComponent();
            this.configFile = configFile;
            this.config = config;
            this.ver = ver;
            Title += ver;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtHost.Text = config.Host;
            TxtPort.Text = config.Port.ToString();
            TxtKey.Text = config.Key;
            CheckAutoRegister.IsChecked = config.AutoRegister;
            TxtQUA.Text = config.QUA;
            TxtVersion.Text = config.Version;
            TxtCode.Text = config.Code;
            CheckDynarmic.IsChecked = config.Dynarmic;
            CheckUnicorn.IsChecked = config.Unicorn;
            CheckDebug.IsChecked = config.Debug;
        }

        private void BtnSave(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TxtPort.Text, out int port) && port >= 0 && port <= 65535) {
                config.Host = TxtHost.Text;
                config.Port = port;
                config.Key = TxtKey.Text;
                config.AutoRegister = CheckAutoRegister.IsChecked.GetValueOrDefault(false);
                config.QUA = TxtQUA.Text;
                config.Version= TxtVersion.Text;
                config.Code = TxtCode.Text;
                config.Dynarmic = CheckDynarmic.IsChecked.GetValueOrDefault(false);
                config.Unicorn = CheckUnicorn.IsChecked.GetValueOrDefault(false);
                config.Debug = CheckDebug.IsChecked.GetValueOrDefault(false);
                config.Write(configFile);
                Close();
            }
            else
            {
                MessageBox.Show("端口必须为 [0, 65535] 间的整数");
            }
        }

        private void ModifyProtocol_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (MessageBox.Show(@"非专业人士请勿修改该选项!
乱改可能会导致你的账号被冻结!
是否继续修改?", "警告", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    cb.IsChecked = false;
                }
                else
                {
                    TxtQUA.IsEnabled = TxtVersion.IsEnabled = TxtCode.IsEnabled = true;
                }
            }
        }
        private void ModifyProtocol_UnChecked(object sender, RoutedEventArgs e)
        {
            TxtQUA.IsEnabled = TxtVersion.IsEnabled = TxtCode.IsEnabled = false;
        }
    }
}
