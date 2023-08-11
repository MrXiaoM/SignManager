using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using CsharpJson;

namespace SignManager
{
    public class Content
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string DownloadURL { get; set; }
        public static Content FromJson(JsonObject obj)
        {
            return new()
            {
                Name = obj["name"].ToString(),
                Path = obj["path"].ToString(),
                Type = obj["type"].ToString(),
                DownloadURL = obj["download_url"].IsNull() ? "" : obj["download_url"].ToString(),
            };
        }
    }
    public class Release
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public string Name { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<Asset> Assets { get; set; }
        public static DateTime ToDateTime(string s)
        {
            return DateTime.ParseExact(s, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }
        public static Release FromJson(JsonObject obj)
        {
            var id = obj["id"].ToInt();
            var tag = obj["tag_name"].ToString();
            var name = obj["name"].ToString();
            var author = User.FromJson(obj["author"].ToObject());
            var createdAt = ToDateTime(obj["created_at"].ToString());
            var publishedAt = ToDateTime(obj["published_at"].ToString());
            var assets = new List<Asset>();
            foreach (JsonValue value in obj["assets"].ToArray())
            {
                assets.Add(Asset.FromJson(value.ToObject()));
            }
            return new()
            {
                Id = id,
                TagName = tag,
                Name = name,
                Author = author,
                CreatedAt = createdAt,
                PublishedAt = publishedAt,
                Assets = assets
            };
        }
    }
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Uploader { get; set; }
        public string ContentType { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string DownloadURL { get; set; }
        public static Asset FromJson(JsonObject obj)
        {
            return new()
            {
                Id = obj["id"].ToInt(),
                Name = obj["name"].ToString(),
                Uploader = User.FromJson(obj["uploader"].ToObject()),
                ContentType = obj["content_type"].ToString(),
                DownloadCount = obj["download_count"].ToInt(),
                CreatedAt = Release.ToDateTime(obj["created_at"].ToString()),
                UpdatedAt = Release.ToDateTime(obj["updated_at"].ToString()),
                DownloadURL = obj["browser_download_url"].ToString()
            };
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string AvatarURL { get; set; }
        public static User FromJson(JsonObject obj)
        {
            return new() {
                Id = obj["id"].ToInt(),
                Login = obj["login"].ToString(),
                AvatarURL = obj["avatar_url"].ToString()
            };
        }
    }
    public class Github
    {
        private static HttpClient http = init();
        private static HttpClient init()
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri("https://api.github.com/")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("User-Agent", "MrXiaoM/SignManager");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            return client;
        }

        public static async Task<List<Release>> Releases(string owner, string repo)
        {
            var list = new List<Release>();
            try
            {
                string path = $"repos/{owner}/{repo}/releases";
                var body = await (await http.GetAsync(path)).Content.ReadAsStringAsync();
                var array = JsonDocument.FromString(body).Array;
                foreach (JsonValue value in array)
                {
                    list.Add(Release.FromJson(value.ToObject()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return list;
        }
        public static async Task<List<Content>> Contents(string owner, string repo, string path = "")
        {
            var list = new List<Content>();
            try
            {
                string path1 = $"repos/{owner}/{repo}/contents/{path}";
                var body = await (await http.GetAsync(path1)).Content.ReadAsStringAsync();
                var array = JsonDocument.FromString(body).Array;
                foreach (JsonValue value in array)
                {
                    list.Add(Content.FromJson(value.ToObject()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return list;
        }
    }
}
