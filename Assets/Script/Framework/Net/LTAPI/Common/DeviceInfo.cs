using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTUnityPlugin.WebClient;

namespace LTUnityPlugin {
public class DeviceInfo {

    public class Key {
        public static readonly string Uuid = "uuid";
        public static readonly string MobileOpr = "mobileOpr";
        public static readonly string Country = "country";
        public static readonly string Language = "language";
        public static readonly string DeviceId = "deviceId";
        public static readonly string DeviceMode = "deviceMode";
        public static readonly string NetState = "netState";
        public static readonly string Memory = "memory";
        public static readonly string ScreenSize = "screenSize";
        public static readonly string OsVersion = "osVersion";

        public static string[] KeyArray = new string[] {
            Uuid,
            MobileOpr,
            Country,
            Language,
            DeviceId,
            DeviceMode,
            NetState,
            Memory,
            ScreenSize,
            OsVersion
        };
    }


    private static DeviceInfo instance;
    public static DeviceInfo Instance {
        get {
            if(instance == null) {
                instance = new DeviceInfo();
            }
            return instance;
        }
    }

    private SimpleMyJson.JsonNodeObject json;

    /// <summary>
    /// this function call jni must use in mono main thread!!
    /// </summary>
    public void Init() {
        string deviceInfo = PluginKits.GetDeviceInfo();
        json = SimpleMyJson.Parse(deviceInfo) as SimpleMyJson.JsonNodeObject;
    }

    public string GetStr(string key) {
        return json.Get(key).AsString();
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
        // remove last &
        result.Remove(result.Length - 1, 1);
        return result.ToString();
    }
    /// <summary>
    /// 根据取得区域语言标示  例如中国大陆 zh_CN
    /// </summary>
    /// <returns></returns>
    public string GetLocalization() {
        return json.Get(Key.Language).AsString().ToLower() + "_" + json.Get(Key.Country).AsString().ToUpper();
    }

    public string GetDefaultLocalization() {
        return "zh_CN";
    }
}
}
