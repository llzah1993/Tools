using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using LTUnityPlugin.WebClient;
using Framework;

namespace LTUnityPlugin {

public class HttpHelper {

    public static string BuildCheckUrl(string type) {
        return AppInfo.Instance.GetStr(AppInfo.Key.CheckUrl) + "?" + AppInfo.Instance.ToString() + "&" + DeviceInfo.Instance.ToString() + "&reqType=" + type;
    }

    public static string LoadString(string url, Encoding ecd) {
        HttpWebClient cl = new HttpWebClient();
        cl.Encoding = ecd;
        string result = string.Empty;
        try {
            result = cl.DownloadString(url);
        } catch(Exception ex) {
            ADebug.LogError(ex.ToString());
        }
        return result;
    }

    public static HttpWebClient LoadStringAsync(string url, Encoding ecd, HttpDownloadStringCompletedEventHandler handler) {
        //ADebug.LogError("LoadStringAsync:{0}" , url);
        HttpWebClient cl = new HttpWebClient();
        cl.Encoding = ecd;
        cl.DownloadStringCompleted += handler;
        cl.DownloadStringAsync(new Uri(url));
        return cl;
    }

    /// <summary>
    /// 异步下载文件 注意回调参数
    /// </summary>
    /// <param name="url"></param>
    /// <param name="file"></param>
    /// <param name="pointbreak"></param>
    /// <param name="progressHandler"></param>
    /// <param name="compHandler"></param>
    public static HttpWebClient LoadFileAsync(string url, string file, bool pointbreak, HttpDownloadProgressChangedEventHandler progressHandler, HttpDownloadFileCompletedEventHandler compHandler) {
        HttpWebClient cl = new HttpWebClient();
        cl.IsBreakpoint = pointbreak;
        cl.DownloadProgressChanged += progressHandler;
        cl.DownloadFileCompleted += compHandler;
        cl.DownloadFileAsync(new Uri(url), file);
        return cl;
    }

    /// <summary>
    /// 向服务器发送埋点统计信息.
    /// </summary>
    /// <param name="keyPoint"></param>
    public static void LogToServer(string keyPoint) {
        HttpWebClient cl = new HttpWebClient();
        cl.DownloadStringAsync(new Uri(BuildCheckUrl(keyPoint)));
    }
}
}
