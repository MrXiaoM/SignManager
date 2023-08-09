using System;
using System.Collections.Generic;
using System.IO;
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
    /// WindowKFCFactory.xaml 的交互逻辑
    /// </summary>
    public partial class WindowKFCFactory : Window
    {
        public class ListConfigItem
        {
            public string Version { get; set; }
            public string Type { get; set; }
            public string BaseUrl { get; set; }
        }
        private string configFile = Environment.CurrentDirectory + "\\KFCFactory.json";
        private KFCFactoryConfig config;
        private List<ListConfigItem> items = new();
        public WindowKFCFactory()
        {
            InitializeComponent();
            config = KFCFactoryConfig.Read(configFile) ?? new();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            items.Clear();
            items = new();
            foreach (string version in config.Services.Keys)
            {
                var service = config.Services[version];
                var type = service.Type;
                if (service is KFCFactoryConfig.UnidbgFetchQSign) type = "unidbg-fetch-qsign";
                if (service is KFCFactoryConfig.MagicSignerGuide) type = "magic-signer";
                items.Add(new()
                {
                    Version = version,
                    Type = type,
                    BaseUrl = service.BaseURL.Length > 0 ? service.BaseURL : "(本地)"
                });
            }
            ListViewServices.ItemsSource = items;
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVisibility(Visibility.Collapsed,
                TxtBaseUrlInfo, TxtBaseUrl,
                TxtKeyInfo, TxtKey,
                TxtServerIdKeyInfo, TxtServerIdKey,
                TxtAuthKeyInfo, TxtAuthKey);
            if (ComboType.SelectedIndex == 0)
            {
                SetVisibility(Visibility.Visible,
                    TxtBaseUrlInfo, TxtBaseUrl,
                    TxtKeyInfo, TxtKey);
            }
            if (ComboType.SelectedIndex == 1)
            {
                SetVisibility(Visibility.Visible,
                    TxtBaseUrlInfo, TxtBaseUrl,
                    TxtServerIdKeyInfo, TxtServerIdKey,
                    TxtAuthKeyInfo, TxtAuthKey);
            }
        }

        private void SetVisibility(Visibility visibility, params FrameworkElement[] controls)
        {
            foreach (FrameworkElement c in controls)
            {
                c.Visibility = visibility;
            }
        }

        private void ListViewServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewServices.SelectedIndex < 0)
            {
                TxtNotSelect.Visibility = Visibility.Visible;
                GridSelectConfig.Visibility = Visibility.Hidden;
                return;
            }
            TxtNotSelect.Visibility = Visibility.Hidden;
            GridSelectConfig.Visibility = Visibility.Visible;
            var item = items[ListViewServices.SelectedIndex];
            TxtTitle.Text = item.Version;
            var service = config.Services[item.Version];
            if (service.Type == "TLV544Provider") ComboType.SelectedIndex = 2;
            else if (service is KFCFactoryConfig.UnidbgFetchQSign s)
            {
                TxtBaseUrl.Text = service.BaseURL;
                TxtKey.Text = s.Key;
                TxtServerIdKey.Text = "";
                TxtAuthKey.Text = "";
                ComboType.SelectedIndex = 0;
            }
            else if (service is KFCFactoryConfig.MagicSignerGuide s1)
            {
                TxtBaseUrl.Text = service.BaseURL;
                TxtKey.Text = "";
                TxtServerIdKey.Text = s1.ServerIdentityKey;
                TxtAuthKey.Text = s1.AuthorizationKey;
                ComboType.SelectedIndex = 1;
            }
            else
            {
                TxtBaseUrl.Text = "";
                TxtKey.Text = "";
                TxtServerIdKey.Text = "";
                TxtAuthKey.Text = "";
                ComboType.SelectedIndex = -1;
            }
        }

        private async void BtnNew(object sender, RoutedEventArgs e)
        {
            var input = new WindowInput("新建配置", "输入新配置版本号 (#.#.##)", "确定");
            if (sender is Button btn && input.ShowDialog().GetValueOrDefault(false))
            {
                if (App.ContainsUnallowedCharater(input.result))
                {
                    MessageBox.Show("新配置版本号中包含不允许的字符");
                    return;
                }
                if (config.Services.ContainsKey(input.result))
                {
                    MessageBox.Show($"已有名为 {input.result} 的配置");
                    return;
                }
                config.Services.Add(input.result, new KFCFactoryConfig.UnidbgFetchQSign()
                {
                    BaseURL = "http://127.0.0.1:8080",
                    Key = "114514"
                });
                config.Write(configFile);
                Refresh();
                ComboType.SelectedIndex = items.Count - 1;
                btn.IsEnabled = false;
                btn.Content = "成功";
                await Task.Delay(TimeSpan.FromSeconds(3));
                btn.IsEnabled = true;
                btn.Content = "新建";
            }
        }
        private void BtnDelete(object sender, RoutedEventArgs e)
        {
            if (ListViewServices.SelectedIndex < 0) return;
            var item = items[ListViewServices.SelectedIndex];
            var ver = item.Version;
            config.Services.Remove(ver);
            config.Write(configFile);
            ListViewServices.SelectedIndex = -1;
            Refresh();
        }
        private async void BtnCopy(object sender, RoutedEventArgs e)
        {
            if (ListViewServices.SelectedIndex < 0) return;
            var item = items[ListViewServices.SelectedIndex];
            var service = config.Services[item.Version];
            var input = new WindowInput("复制配置", "输入复制到的新配置版本号 (#.#.##)", "确定", item.Version);
            if (sender is Button btn && input.ShowDialog().GetValueOrDefault(false))
            {
                if (App.ContainsUnallowedCharater(input.result))
                {
                    MessageBox.Show("新配置版本号中包含不允许的字符");
                    return;
                }
                if (config.Services.ContainsKey(input.result))
                {
                    MessageBox.Show($"已有名为 {input.result} 的配置");
                    return;
                }
                config.Services.Add(input.result, service.Copy());
                config.Write(configFile);
                Refresh();
                ComboType.SelectedIndex = items.Count - 1;
                btn.IsEnabled = false;
                btn.Content = "复制成功";
                await Task.Delay(TimeSpan.FromSeconds(3));
                btn.IsEnabled = true;
                btn.Content = "复制";
            }
        }

        private async void BtnSave(object sender, RoutedEventArgs e)
        {
            if (ListViewServices.SelectedIndex < 0 || ComboType.SelectedIndex < 0) return;
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                btn.Content = "保存中";
                var item = items[ListViewServices.SelectedIndex];
                var ver = item.Version;
                KFCFactoryConfig.EncryptService service;
                if (ComboType.SelectedIndex == 0)
                {
                    service = new KFCFactoryConfig.UnidbgFetchQSign()
                    {
                        BaseURL = TxtBaseUrl.Text,
                        Key = TxtKey.Text
                    };
                }
                else if (ComboType.SelectedIndex == 1)
                {
                    service = new KFCFactoryConfig.MagicSignerGuide()
                    {
                        BaseURL = TxtBaseUrl.Text,
                        ServerIdentityKey = TxtServerIdKey.Text,
                        AuthorizationKey = TxtAuthKey.Text
                    };
                }
                else
                {
                    service = new KFCFactoryConfig.EncryptService()
                    {
                        Type = "TLV544Provider",
                        BaseURL = ""
                    };
                }
                config.Services[ver] = service;

                config.Write(configFile);
                btn.Content = "保存成功";
                await Task.Delay(TimeSpan.FromSeconds(3));
                btn.IsEnabled = true;
                btn.Content = "保存";
            }
        }
    }
}
