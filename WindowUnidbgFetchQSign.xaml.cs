using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

    }
}
