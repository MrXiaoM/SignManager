using CsharpJson;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SignManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static bool ContainsUnallowedCharater(string s)
        {
            return s.Contains("\\") || s.Contains("/") || s.Contains(":")
                || s.Contains("*") || s.Contains("?") || s.Contains("\"")
                || s.Contains("<") || s.Contains(">") || s.Contains("|");
        }
        private static void delete(DirectoryInfo info)
        {
            foreach (FileInfo file in info.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                delete(dir);
            }
            info.Delete();
        }
        public static void Delete(DirectoryInfo info, bool create = false)
        {
            delete(info);
            if (create) info.Create();
        }
    }

    public class KFCFactoryConfig
    {
        public Dictionary<string, EncryptService> Services;
        public class EncryptService
        {
            public string Type { get; set; }
            public string BaseURL { get; set; }
            public virtual EncryptService Copy()
            {
                return new EncryptService()
                {
                    Type = Type,
                    BaseURL = BaseURL
                };
            }
        }
        public class UnidbgFetchQSign : EncryptService
        {
            public string Key { get; set; }
            public UnidbgFetchQSign()
            {
                Type = "fuqiuluo/unidbg-fetch-qsign";
            }
            public override EncryptService Copy()
            {
                return new UnidbgFetchQSign()
                {
                    BaseURL = BaseURL,
                    Key = Key
                };
            }
        }
        public class MagicSignerGuide : EncryptService
        {
            public string ServerIdentityKey { get; set; }
            public string AuthorizationKey { get; set; }
            public MagicSignerGuide()
            {
                Type = "kiliokuara/magic-signer-guide";
            }
            public override EncryptService Copy()
            {
                return new MagicSignerGuide()
                {
                    BaseURL = BaseURL,
                    ServerIdentityKey = ServerIdentityKey,
                    AuthorizationKey = AuthorizationKey
                };
            }
        }
        public static KFCFactoryConfig Read(string file)
        {
            var json = JsonDocument.FromString(File.ReadAllText(file)).Object;
            var dict = new Dictionary<string, EncryptService>();
            foreach (string version in json.Keys)
            {
                var obj = json[version].ToObject();
                string type = obj["type"].ToString();
                if (type == "fuqiuluo/unidbg-fetch-qsign" || type == "fuqiuluo" || type == "unidbg-fetch-qsign")
                {
                    dict.Add(version, new UnidbgFetchQSign()
                    {
                        BaseURL = obj["base_url"].ToString(),
                        Key = obj["key"].ToString()
                    });
                }
                else if (type == "kiliokuara/magic-signer-guide" || type == "kiliokuara" || type == "magic-signer-guide" || type == "vivo50")
                {
                    dict.Add(version, new MagicSignerGuide()
                    {
                        BaseURL = obj["base_url"].ToString(),
                        ServerIdentityKey = obj["serverIdentityKey"].ToString(),
                        AuthorizationKey = obj["authorizationKey"].ToString()
                    });
                }
                else
                {
                    dict.Add(version, new()
                    {
                        Type = type,
                        BaseURL = ""
                    });
                }
            }
            return new()
            {
                Services = dict
            };
        }
        public void Write(string file)
        {
            var json = new JsonObject();
            foreach (string version in Services.Keys)
            {
                var service = Services[version];
                var obj = new JsonObject();
                obj["type"] = service.Type;
                obj["base_url"] = service.BaseURL;
                if (service is UnidbgFetchQSign s)
                {
                    obj["key"] = s.Key;
                }
                else if (service is MagicSignerGuide s1)
                {
                    obj["serverIdentityKey"] = s1.ServerIdentityKey;
                    obj["authorizationKey"] = s1.AuthorizationKey;
                }
                json[version] = obj;
            }

            var doc = new JsonDocument();
            doc.Object = json;
            File.WriteAllText(file, doc.ToJson(), Encoding.UTF8);
        }
    }

    public class UnidbgFetchQSignConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Key { get; set; }
        public bool AutoRegister { get; set; }
        public string QUA { get; set; }
        public string Version { get; set; }
        public string Code { get; set; }
        public bool Dynarmic { get; set; }
        public bool Unicorn { get; set; }
        public bool Debug { get; set; }
        public List<long> BlackList { get; set; }

        public static UnidbgFetchQSignConfig? Read(string file)
        {
            try
            {
                var obj = JsonDocument.FromString(File.ReadAllText(file)).Object;
                var server = obj["server"].ToObject();
                var protocol = obj["protocol"].ToObject();
                var unidbg = obj["unidbg"].ToObject();
                var blackList = new List<long>();
                if (obj.ContainsKey("black_list"))
                {
                    foreach (JsonValue value in obj["black_list"].ToArray())
                    {
                        blackList.Add(value.ToLong());
                    }
                }
                return new()
                {
                    Host = server["host"].ToString(),
                    Port = server["port"].ToInt(),
                    Key = obj["key"].ToString(),
                    AutoRegister = obj["auto_register"].ToBool(),
                    QUA = protocol["qua"].ToString(),
                    Version = protocol["version"].ToString(),
                    Code = protocol["code"].ToString(),
                    Dynarmic = unidbg["dynarmic"].ToBool(),
                    Unicorn = unidbg["unicorn"].ToBool(),
                    Debug = unidbg["debug"].ToBool(),
                    BlackList = blackList
                };
            }
            catch { }
            return null;
        }
        public void Write(string file)
        {
            var doc = JsonDocument.FromString(File.ReadAllText(file));
            var obj = doc.Object;

            var server = obj["server"].ToObject();
            var protocol = obj["protocol"].ToObject();
            var unidbg = obj["unidbg"].ToObject();
            var blackList = new JsonArray();

            server["host"] = Host;
            server["port"] = Port;
            obj["server"] = server;
            obj["key"] = Key;
            obj["auto_register"] = AutoRegister;
            protocol["qua"] = QUA;
            protocol["version"] = Version;
            protocol["code"] = Code;
            obj["protocol"] = protocol;
            unidbg["dynarmic"] = Dynarmic;
            unidbg["unicorn"] = Unicorn;
            unidbg["debug"] = Debug;
            obj["unidbg"] = unidbg;

            foreach (long qq in BlackList)
            {
                blackList.Add(new(qq));
            }
            obj["black_list"] = blackList;

            doc.Object = obj;
            File.WriteAllText(file, doc.ToJson(), Encoding.UTF8);
        }
    }

    public static class HttpClientExt
    {
        public static async Task<bool> GetByteArrayAsync(this HttpClient httpClient, string url, Action<byte[]> success, Action<HttpRequestException> fail)
        {
            try
            {
                byte[] bytes = await httpClient.GetByteArrayAsync(url);
                success(bytes);
                return true;
            }
            catch (HttpRequestException e)
            {
                fail(e);
                return false;
            }
        }
        public static async Task<bool> GetStringAsync(this HttpClient httpClient, string url, Action<string> success, Action<HttpRequestException> fail)
        {
            try
            {
                string s = await httpClient.GetStringAsync(url);
                success(s);
                return true;
            }
            catch (HttpRequestException e)
            {
                fail(e);
                return false;
            }
        }
    }
}
