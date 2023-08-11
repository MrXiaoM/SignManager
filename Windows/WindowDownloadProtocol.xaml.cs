using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading;

namespace SignManager
{
    public class ItemProtocol
    {
        public string Version { get; set; } = "";
        public string DownloadPhone { get; set; } = "";
        public string DownloadPad { get; set; } = "";
        public Visibility Phone
        {
            get
            {
                return DownloadPhone.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility Pad
        {
            get
            {
                return DownloadPad.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
    /// <summary>
    /// WindowDownloadFPV.xaml 的交互逻辑
    /// </summary>
    public partial class WindowDownloadProtocol : Window
    {
        Func<string, string> processUrl;
        public WindowDownloadProtocol(Func<string, string> processUrl)
        {
            InitializeComponent();
            this.processUrl = processUrl;
        }

        private void BtnNote(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {

            }
        }
        private async void BtnDownload(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                var split = ((string) btn.Tag).Split(":", 2);
                string ver = split[0];
                string url = processUrl.Invoke(split[1]);
                var name = ((string)btn.Content).ToLower().Replace("__", "_") + ".json";
                DownloadTitle.Text += ver;
                DownloadName.Text = url.Substring(url.LastIndexOf('/') + 1);
                ListVersion.IsEnabled = BtnRefresh.IsEnabled = false;
                ListVersion.Visibility = BtnRefresh.Visibility = Visibility.Hidden;
                ProcessGrid.Visibility = Visibility.Visible;
                HttpClientHandler handler = new();
                ProgressMessageHandler processHandler = new(handler);
                HttpClient httpClient = new(processHandler);

                // 回调进度
                processHandler.HttpReceiveProgress += (sender, e) => Dispatcher.Invoke(() =>
                {
                    double received = e.BytesTransferred;
                    double? total = e.TotalBytes;
                    string percent = total == null ? "" : (" " + string.Format("{0:N2}%", received / total * 100d));
                    DownloadProcessBar.Value = received / (total ?? received) * 10000;
                    DownloadProcess.Text = WindowDownload.FormatSize(received) + (total == null ? "" : (" / " + WindowDownload.FormatSize(total))) + percent;
                });

                bool success = await httpClient.GetByteArrayAsync(url,
                    bytes => File.WriteAllBytes(Environment.CurrentDirectory + "\\" + name, bytes),
                    ex => MessageBox.Show(name + " 下载失败!\n" + url + "\n" + ex.ToString(), "下载错误")
                );

                ListVersion.IsEnabled = BtnRefresh.IsEnabled = true;
                ListVersion.Visibility = BtnRefresh.Visibility = Visibility.Visible;
                ProcessGrid.Visibility = Visibility.Hidden;
                btn.IsEnabled = true;
                DownloadProcess.Text = "正在获取下载信息";
                MessageBox.Show($"{name} : {ver} 下载成功!");
            }
        }

        private async void BtnUpdateList(object sender, RoutedEventArgs e)
        {
            await ReloadVersionList();
        }
        private async Task ReloadVersionList()
        {
            BtnRefresh.IsEnabled = ComboSource.IsEnabled = ListVersion.IsHitTestVisible = false;
            BtnRefresh.Content = "正在刷新...";
            var list = new List<ItemProtocol>();
            if (ComboSource.SelectedIndex == 0)
            {
                var dict = new Dictionary<string, ItemProtocol>();
                await FetchFromRomiChan(dict, "android_phone", (protocol, ver, url) => protocol.DownloadPhone = $"{ver}:{url}");
                await FetchFromRomiChan(dict, "android_pad", (protocol, ver, url) => protocol.DownloadPad = $"{ver}:{url}");
                list.AddRange(dict.Values);
            }
            else if (ComboSource.SelectedIndex == 1)
            {
                await FetchFromMrXiaoM(list);
            }
            BtnRefresh.IsEnabled = ComboSource.IsEnabled = ListVersion.IsHitTestVisible = true;
            BtnRefresh.Content = "刷新版本列表";
            ListVersion.ItemsSource = list;
        }

        private async Task FetchFromRomiChan(Dictionary<string, ItemProtocol> dict, string path, Action<ItemProtocol, string, string> action)
        {
            var files = await Github.Contents("RomiChan", "protocol-versions", path);
            foreach (Content file in files)
            {
                var ver = file.Name.Replace(".json", "");

                if (!dict.ContainsKey(ver)) dict.Add(ver, new() { Version = ver });
                var protocol = dict[ver];

                var url = file.DownloadURL;
                if (url.Length == 0) continue;

                action.Invoke(protocol, ver, url);
            }
        }

        private async Task FetchFromMrXiaoM(List<ItemProtocol> list)
        {
            foreach (Content verDir in await Github.Contents("MrXiaoM", "qsign", "txlib"))
            {
                if (verDir.Type != "dir") continue;
                string ver = verDir.Name;
                List<Content> files = await Github.Contents("MrXiaoM", "qsign", $"txlib/{ver}");
                files.RemoveAll(it => it.Type != "file"
                || (!it.Name.Equals("android_phone.json", StringComparison.CurrentCultureIgnoreCase)
                && !it.Name.Equals("android_pad.json", StringComparison.CurrentCultureIgnoreCase)));

                ItemProtocol protocol = new() { Version = ver };
                foreach (Content file in files)
                {
                    if (file.Name.Equals("android_phone.json", StringComparison.CurrentCultureIgnoreCase)) protocol.DownloadPhone = $"{ver}:{file.DownloadURL}";
                    if (file.Name.Equals("android_pad.json", StringComparison.CurrentCultureIgnoreCase)) protocol.DownloadPad = $"{ver}:{file.DownloadURL}";
                }
                list.Add(protocol);
            }
        }

        private async void ComboSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListVersion == null) return;
            await ReloadVersionList();
        }
    }
}
