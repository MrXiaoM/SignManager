using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace SignManager
{
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public class GithubSource
        {
            public string Name { get; set; }
            public string UrlPrefix { get; set; } = "";
        }
        public class KFCFactoryInfo
        {
            public Visibility Error { get; set; } = Visibility.Collapsed;
            public Visibility Normal { get; set; } = Visibility.Collapsed;
            public string ErrorMessage { get; set; } = "";
            public string Version { get; set; } = "";
            public string Type { get; set; } = "";
            public string Address { get; set; } = "";
            public Brush AddressColor { get; set; } = brushWarn;
        }

        private FileInfo scriptCmd = new(Environment.CurrentDirectory + "\\start_unidbg-fetch-qsign.cmd");
        private FileInfo scriptShell = new(Environment.CurrentDirectory + "\\start_unidbg-fetch-qsign.sh");
        private DirectoryInfo pluginsDir = new(Environment.CurrentDirectory + "\\plugins");
        private DirectoryInfo qsignDir = new(Environment.CurrentDirectory + "\\unidbg-fetch-qsign");
        private DirectoryInfo txlibDir = new(Environment.CurrentDirectory + "\\unidbg-fetch-qsign\\txlib");
        private static Brush brushNormal = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#55B155"));
        private static Brush brushWarn = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE4A0"));
        private static Brush brushError = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#F55762"));
        private List<GithubSource> githubSources = new()
        {
            new() { Name = "源站 github.com" },
            new() { Name = "ghproxy.com", UrlPrefix = "https://ghproxy.com/" },
            new() { Name = "github.moeyy.cn", UrlPrefix = "https://github.moeyy.cn/" },
        };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!pluginsDir.Exists) pluginsDir.Create();
            if (!qsignDir.Exists) qsignDir.Create();
            ComboGithubSource.ItemsSource = githubSources;
            ComboGithubSource.SelectedIndex = 1;
            RunCheck();
            Activate();
        }

        private void RunCheck()
        {
            var plugins = pluginsDir.GetFiles("fix-protocol-version-*.mirai2.jar");
            TxtFPVVer.Foreground = brushError;
            if (plugins.Length == 0)
            {
                TxtFPVVer.Text = "未安装";
            }
            else if (plugins.Length == 1)
            {
                TxtFPVVer.Foreground = brushNormal;
                TxtFPVVer.Text = plugins[0].Name.Substring(21, plugins[0].Name.Length - 32);
            }
            else
            {
                TxtFPVVer.Text = "重复安装";
            }
            string scriptVersion = "";
            if (scriptShell.Exists)
            {
                string s = File.ReadAllLines(scriptShell.FullName, Encoding.UTF8)[0];
                scriptVersion = s.Substring(s.IndexOf('=') + 1);
            }
            else if (scriptCmd.Exists) {
                string s = File.ReadAllLines(scriptShell.FullName, Encoding.UTF8)[1];
                scriptVersion = s.Substring(s.IndexOf('=') + 1);
            }
            int tempPort = -1;
            if (scriptVersion.Length == 0)
            {
                TxtSignVer.Foreground = TxtSignAddress.Foreground = brushError;
                TxtSignVer.Text = TxtSignAddress.Text = "找不到脚本";
            }
            else
            {
                TxtSignVer.Foreground = brushNormal;
                TxtSignVer.Text = scriptVersion;
                var config = UnidbgFetchQSignConfig.Read(txlibDir.FullName + "\\" + scriptVersion + "\\config.json");
                if (config == null)
                {
                    TxtSignAddress.Foreground = brushError;
                    TxtSignAddress.Text = "未找到配置";
                }
                else
                {
                    TxtSignAddress.Foreground = brushNormal;
                    TxtSignAddress.Text = $"http://{config.Host}:{config.Port}";
                    tempPort = config.Port;
                }
            }
            List<KFCFactoryInfo> kfcFactory = new();
            var kfcFactoryFile = Environment.CurrentDirectory + "\\KFCFactory.json";
            if (File.Exists(kfcFactoryFile))
            {
                var i = TxtSignAddress.Text.LastIndexOf(':');
                var port = (i > 8) ? TxtSignAddress.Text[i++..] : "####";
                var config = KFCFactoryConfig.Read(kfcFactoryFile);
                foreach (string version in config.Services.Keys)
                {
                    var service = config.Services[version];
                    var type = service.Type;
                    if (service is KFCFactoryConfig.UnidbgFetchQSign) type = "unidbg-fetch-qsign";
                    if (service is KFCFactoryConfig.MagicSignerGuide) type = "magic-signer";
                    var info = new KFCFactoryInfo()
                    {
                        Normal = Visibility.Visible,
                        Version = version,
                        Type = type,
                        Address = service.BaseURL.Length > 0 ? service.BaseURL : "(无)"
                    };
                    if (type == "unidbg-fetch-qsign" && tempPort > 0 
                        && info.Version == TxtSignVer.Text
                        && info.Address.EndsWith($":{tempPort}"))
                    {
                        info.AddressColor = brushNormal;
                    }
                    kfcFactory.Add(info);
                }
            }
            else
            {
                kfcFactory.Add(new() { Error = Visibility.Visible, ErrorMessage = "未找到配置" });
            }
            ListKFCFactoryInfo.ItemsSource = kfcFactory;

            List<DirectoryInfo> txlibVersions = txlibDir.Exists ? new (txlibDir.GetDirectories("*.*.*")) : new();
            ComboQSignVer.ItemsSource = txlibVersions;
            ComboQSignVer.SelectedIndex = txlibVersions.Count > 0 ? 0 : -1;
        }

        private string ProcessAssetURL(string download)
        {
            if (download.StartsWith("https://github.com"))
            {
                return githubSources[ComboGithubSource.SelectedIndex].UrlPrefix + download;
            }
            return download;
        }

        private void OpenURL(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink)
            {
                Process.Start("explorer.exe", hyperlink.NavigateUri.ToString());
            }
        }

        private async void BtnRefresh(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                btn.Content = "正在检查";
                RunCheck();
                btn.Content = "检查完成!";
                await Task.Delay(TimeSpan.FromSeconds(3));
                btn.IsEnabled = true;
                btn.Content = "重新检查";
            }
        }
        private void BtnUpdateFPV(object sender, RoutedEventArgs e)
        {
            var download = new WindowDownload("cssxsh", "fix-protocol-version",
                "下载/更新 cssxsh/fix-protocol-version",
                // 资源文件名要求条件
                it => it.Name.EndsWith(".mirai2.jar"),
                ProcessAssetURL
            );
            if (download.ShowDialog().GetValueOrDefault(false) && download.bytes != null)
            {
                foreach (FileInfo file in pluginsDir.GetFiles("fix-protocol-version-*.mirai2.jar"))
                {
                    file.Delete();
                }
                File.WriteAllBytes(Environment.CurrentDirectory + "\\plugins\\" + download.name, download.bytes);
            }
        }
        private void BtnKFCFactoryConfig(object sender, RoutedEventArgs e)
        {
            new WindowKFCFactory().ShowDialog();
            RunCheck();
        }
        private void BtnUpdateProtocol(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO");
        }
        private async void BtnUpdateQSign(object sender, RoutedEventArgs e)
        {
            var download = new WindowDownload("fuqiuluo", "unidbg-fetch-qsign",
                "下载/更新 fuqiuluo/unidbg-fetch-qsign",
                // 资源文件名要求条件
                it => it.Name.EndsWith(".zip"),
                ProcessAssetURL
            );
            if (download.ShowDialog().GetValueOrDefault(false) && download.bytes != null)
            {
                string temp = Environment.CurrentDirectory + "\\.SignManager.unidbg-fetch-qsign.temp.zip";
                App.Delete(qsignDir, true);
                await File.WriteAllBytesAsync(temp, download.bytes);
                // 解决压缩包套压缩包的问题
                var zip = ZipFile.OpenRead(temp);
                if (zip.Entries.Count == 1 && zip.Entries[0].Name.EndsWith(".zip"))
                {
                    zip.Entries[0].ExtractToFile(temp + ".temp1");
                    zip.Dispose();
                    File.Delete(temp);
                    File.Move(temp + ".temp1", temp);
                }
                else zip.Dispose();
                // 解压
                using (zip = ZipFile.OpenRead(temp))
                {
                    string unzip = qsignDir.FullName;
                    if (!unzip.EndsWith("\\")) unzip += "\\";
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        string path = entry.FullName.Replace("/", "\\");
                        if (path.StartsWith("unidbg-fetch-qsign"))
                        {
                            path = path.Substring(path.IndexOf('\\') + 1);
                        }
                        if (path.StartsWith("\\")) path = path.Substring(1);
                        path = unzip + path;
                        // 以 \ 结尾的是目录
                        if (path.EndsWith('\\'))
                        {
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        }
                        else entry.ExtractToFile(path, true);
                    }
                }
                File.Delete(temp);
            }
        }

        private void BtnGenQSignScripts(object sender, RoutedEventArgs e)
        {
            if (ComboQSignVer.SelectedIndex < 0) return;
            string version = ((List<DirectoryInfo>)ComboQSignVer.ItemsSource)[ComboQSignVer.SelectedIndex].Name;
            File.WriteAllText(scriptCmd.FullName, @$"@echo off
set TXLIB_VERSION={version}
title unidbg-fetch-qsign with txlib/%TXLIB_VERSION%
java -cp unidbg-fetch-qsign/lib/* MainKt --basePath=unidbg-fetch-qsign/txlib/%TXLIB_VERSION%
set EL=%ERRORLEVEL%
if %EL% NEQ 0 (
    echo Process exited with %EL%
    pause
)
", Encoding.UTF8);
            File.WriteAllText(scriptShell.FullName, @$"TXLIB_VERSION={version}
java -cp unidbg-fetch-qsign/lib/* MainKt --basePath=unidbg-fetch-qsign/txlib/$TXLIB_VERSION
", Encoding.UTF8);
            MessageBox.Show("已保存到:\nstart_unidbg-fetch-qsign.cmd (适用于 Windows)\nstart_unidbg-fetch-qsign.sh (适用于 Linux/macOS)", "保存成功");
        }

        private void BtnQSignConfig(object sender, RoutedEventArgs e)
        {
            if (ComboQSignVer.SelectedIndex < 0) return;
            string version = ((List<DirectoryInfo>)ComboQSignVer.ItemsSource)[ComboQSignVer.SelectedIndex].Name;
            string configFile = txlibDir.FullName + "\\" + version + "\\config.json";
            var config = UnidbgFetchQSignConfig.Read(configFile);
            if (config == null)
            {
                MessageBox.Show("找不到配置文件，或配置文件已损坏");
            }
            else
            {
                new WindowUnidbgFetchQSign(configFile, config, version).ShowDialog();
                RunCheck();
            }
        }

        private void ComboQSignVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridQSignConfigBtn.IsEnabled = ComboQSignVer.SelectedIndex >= 0;
        }
    }
}
