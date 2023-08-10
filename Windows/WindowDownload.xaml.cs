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
    public class ItemRelease
    {
        public string ReleaseTag { get; set; }
        public string ReleaseTime { get; set; }
        public string DownloadURL { get; set; }
    }
    /// <summary>
    /// WindowDownloadFPV.xaml 的交互逻辑
    /// </summary>
    public partial class WindowDownload : Window
    {
        string owner;
        string repo;
        Predicate<Asset> assetPredicate;
        Func<string, string> processUrl;
        public bool abort = false;
        public string name = "";
        public string tag = "";
        public byte[]? bytes = null;
        CancellationTokenSource cts = new CancellationTokenSource();
        public WindowDownload(string owner, string repo, string title, Predicate<Asset> assetPredicate, Func<string, string> processUrl)
        {
            InitializeComponent();
            this.owner = owner;
            this.repo = repo;
            this.assetPredicate = assetPredicate;
            this.processUrl = processUrl;
            this.Title = title;
        }

        private void BtnNote(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                Process.Start("explorer.exe", $"https://github.com/{owner}/{repo}/releases/tag/{btn.Tag}");
            }
        }
        private async void BtnDownload(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                var split = ((string)btn.Tag).Split('|', 2);
                tag = split[0];
                DownloadTitle.Text += tag;
                string url = processUrl.Invoke(split[1]);
                name = url.Substring(url.LastIndexOf('/') + 1);
                DownloadName.Text = name;
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
                    DownloadProcess.Text = FormatSize(received) + (total == null ? "" : (" / " + FormatSize(total))) + percent;
                });
                var token = cts.Token;
                token.Register(() => abort = true);
                bool success = await httpClient.GetByteArrayAsync(url,
                    bytes => this.bytes = bytes,
                    ex => MessageBox.Show(name + " 下载失败!\n" + url + "\n" + ex.ToString(), "下载错误"),
                    token
                );
                if (success) DialogResult = true;

                DownloadProcess.Text = "下载完成";
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cts.Cancel();
        }
        private async void BtnUpdateList(object sender, RoutedEventArgs e)
        {
            await ReloadVersionList();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ReloadVersionList();
        }
        private async Task ReloadVersionList()
        {
            BtnRefresh.IsEnabled = false;
            BtnRefresh.Content = "正在刷新...";
            var releases = await Github.Releases(owner, repo);
            var list = new List<ItemRelease>();
            foreach (Release release in releases)
            {
                var asset = release.Assets.Find(assetPredicate);
                if (asset == null) continue;
                list.Add(new()
                {
                    ReleaseTag = release.TagName,
                    ReleaseTime = release.PublishedAt.ToString("yyyy年MM月dd日 HH:mm:ss"),
                    DownloadURL = release.TagName + "|" + asset.DownloadURL
                });
            }
            BtnRefresh.IsEnabled = true;
            BtnRefresh.Content = "刷新版本列表";
            ListVersion.ItemsSource = list;
        }


        private static readonly string[] SIZE_UNITS = { "B", "KB", "MB", "GB", "TB" };
        public static string FormatSize(double? size, uint point = 2)
        {
            if (size == null || size <= 0) return "?";
            string unit = SIZE_UNITS[0];
            int i = 0;
            while (size > 1024)
            {
                i++;
                if (i >= SIZE_UNITS.Length) break;
                size /= 1024;
                unit = SIZE_UNITS[i];
            }
            return string.Format("{0:N" + point + "}", size) + unit;
        }
    }
}
