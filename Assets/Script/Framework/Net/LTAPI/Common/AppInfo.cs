using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using LTUnityPlugin.WebClient;
namespace LTUnityPlugin {

public class AppInfo {

    public class Key {
        public static readonly string GameId = "gameId";
        public static readonly string ChannelId = "channelId";
        public static readonly string AppType = "appType";
        public static readonly string AppVersion = "appVersion";
        public static readonly string ResBuiltIn = "resBuiltIn";
        public static readonly string ResMode = "resMode";
        public static readonly string ResDir = "resDir";
        public static readonly string ConfigDir = "configDir";
        public static readonly string StreamAssets = "streamAssets";
        public static readonly string DownDir = "downDir";
        public static readonly string RecordDir = "recordDir";
        public static readonly string LibPacker = "libPacker";
        public static readonly string ResVersion = "resVersion";
        public static readonly string DllVersion = "dllVersion";
        public static readonly string ServerUrl = "serverUrl";
        public static readonly string ServerListUrl = "serverListUrl";
        public static readonly string UserServer = "userServer";
        public static readonly string CheckUrl = "checkUrl";
        public static readonly string NewUser = "newUser";
        public static readonly string DownUrl = "downUrl";
        public static readonly string Cideng = "cideng";
        public static readonly string WXAPPKey = "wx";
        public static readonly string XFFlag = "xf";

        public static string[] KeyArray  = new string[] {
            GameId,
            ChannelId,
            AppType,
            AppVersion,
            ResBuiltIn,
            ResVersion,
            DllVersion,
        };
    }

    private static AppInfo instance;
    public static AppInfo Instance {
        get {
            if(instance == null) {
                instance = new AppInfo();
            }
            return instance;
        }
    }

    private string baseDir {
        get {
            return Application.persistentDataPath;
        }
    }

    private SimpleMyJson.JsonNodeObject json;

    private string resourcePath = string.Empty;
    public string ResourcePath {
        get {
            if(string.IsNullOrEmpty(resourcePath)) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                resourcePath = Directory.GetParent(Application.dataPath).FullName.Replace(@"\", FileHelper.Separator) + FileHelper.Separator + json.Get(Key.ResDir).AsString() + FileHelper.Separator;
#elif UNITY_ANDROID
                resourcePath = Application.persistentDataPath + FileHelper.Separator + json.Get(Key.ResDir).AsString() + FileHelper.Separator;
#endif
                if(!Directory.Exists(resourcePath)) {
                    Directory.CreateDirectory(resourcePath);
                }
            }
            return resourcePath;
        }
    }

    private string configPath = string.Empty;
    public string ConfigPath {
        get {
            if(string.IsNullOrEmpty(configPath)) {
                configPath = ResourcePath + FileHelper.Separator + json.Get(Key.ConfigDir).AsString() + FileHelper.Separator;
                if(!Directory.Exists(configPath)) {
                    Directory.CreateDirectory(configPath);
                }
            }
            return configPath;
        }
    }

    private string recordPath = string.Empty;
    public string RecordPath {
        get {
            if(string.IsNullOrEmpty(recordPath)) {
                recordPath = ResourcePath + FileHelper.Separator + json.Get(Key.RecordDir).AsString() + FileHelper.Separator;
                if(!Directory.Exists(recordPath)) {
                    Directory.CreateDirectory(recordPath);
                }
            }
            return recordPath;
        }
    }

    private string streamPath = string.Empty;
    public string StreamPath {
        get {
            if(string.IsNullOrEmpty(streamPath)) {
                streamPath = ResourcePath + FileHelper.Separator + json.Get(Key.StreamAssets).AsString() + FileHelper.Separator;
                if(!Directory.Exists(streamPath)) {
                    Directory.CreateDirectory(streamPath);
                }
            }
            return streamPath;
        }
    }

    private string dllPackerFile = string.Empty;
    public string DllPackerFile {
        get {
            if(string.IsNullOrEmpty(dllPackerFile)) {
                dllPackerFile = ResourcePath + FileHelper.Separator + json.Get(Key.LibPacker).AsString();
                if(!Directory.Exists(Directory.GetParent(dllPackerFile).FullName)) {
                    Directory.CreateDirectory(Directory.GetParent(dllPackerFile).FullName);
                }
            }
            return dllPackerFile;
        }
    }

    private string downLoadPath = string.Empty;
    public string DownloadPath {
        get {
            if(string.IsNullOrEmpty(downLoadPath)) {
                downLoadPath = ResourcePath + FileHelper.Separator + json.Get(Key.DownDir).AsString() + FileHelper.Separator;
                if(!Directory.Exists(downLoadPath)) {
                    Directory.CreateDirectory(downLoadPath);
                }
            }
            return downLoadPath;
        }
    }

    private string downLoadAPPPath = string.Empty;
    public string DownloadAPPPath {
        get {
            if(string.IsNullOrEmpty(downLoadAPPPath)) {
                if(Application.platform == RuntimePlatform.Android) {
                    downLoadAPPPath = PluginKits.GetSDPath() + "Download" + FileHelper.Separator + json.Get(Key.GameId).AsString() + FileHelper.Separator + json.Get(Key.ChannelId).AsString() + FileHelper.Separator;
                } else {
                    downLoadAPPPath = DownloadPath;
                }
                if(!Directory.Exists(downLoadAPPPath)) {
                    Directory.CreateDirectory(downLoadAPPPath);
                }
            }
            return downLoadAPPPath;
        }
    }

    public void Init() {
        string testData = "{\"gameId\":\"999\",\"channelId\":\"0000000\",\"appType\":\"1\",\"appVersion\":\"1\",\"resBuiltIn\":\"1\",\"resMode\":\"external\",\"resDir\":\"Resources\",\"configDir\":\"Config\",\"streamAssets\":\"Stream\",\"downDir\":\"Download\",\"recordDir\":\"Record\",\"libPacker\":\"Gamelibs/libGame.so\",\"userServer\":\"http://netuser.joymeng.com/user/\",\"checkUrl\":\"http://10.80.1.35/ltupdate/cs/app.php\"}";
        json = SimpleMyJson.Parse(testData) as SimpleMyJson.JsonNodeObject;

        // dll version rewrite mark!
        json.SetDictValue(Key.DllVersion, 99999);

        // 获取当前实际资源版本号 rewrite mark !
        json.SetDictValue(Key.ResVersion, 1);

        // test data
        json.SetDictValue(Key.ServerUrl, "netesb1.joymeng.com:60000");
    }

    public void SetKeyValue(string key, string value) {
        json.SetDictValue(key, value);
    }

    public void SetKeyValue(string key, int value) {
        json.SetDictValue(key, value);
    }

    public string GetStr(string key) {
        SimpleMyJson.IJsonNode value;
        if(json.TryGetValue(key, out value)) {
            return value.AsString();
        } else {
            return string.Empty;
        }

    }

    public int GetInt(string key) {
        return json.Get(key).AsInt();
    }

    public override string ToString() {
        StringBuilder result = new StringBuilder();
        foreach(string key in Key.KeyArray) {
            result.Append(key);
            result.Append("=");
            result.Append(HttpWebClient.UrlEncode(json.Get(key).AsString()));
            result.Append("&");
        }
        result.Append("channel_id=");
        result.Append(HttpWebClient.UrlEncode(json.Get(Key.ChannelId).AsString()));
        result.Append("&");
        result.Append("app_id=");
        result.Append(HttpWebClient.UrlEncode(json.Get(Key.GameId).AsString()));
        return result.ToString();
    }
}

}