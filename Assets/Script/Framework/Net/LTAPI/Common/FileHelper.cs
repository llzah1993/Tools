using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LTUnityPlugin {
public class FileHelper {

    public static string Separator {
        get {
            return "/";
        }
    }

    private static string[] UsefullLocal = new string[] {
        "zh_CN"
    };
    /// <summary>
    /// 部分多语言资源的获取
    /// </summary>
    /// <returns></returns>
    public static string GetUsefullLocalization() {
        if(UsefullLocal.Contains(DeviceInfo.Instance.GetLocalization())) {
            return DeviceInfo.Instance.GetLocalization();
        } else {
            return DeviceInfo.Instance.GetDefaultLocalization();
        }
    }

    public static string Combine(params string[] paths) {
        string temp = string.Empty;
        for(int i = 0; i < paths.Length; i++) {
            string path = paths[i];
            path = path.Replace(@"\", Separator);
            if(path.EndsWith(Separator) || (i == paths.Length - 1)) {
                temp += path;
            } else {
                temp += path;
                temp += Separator;
            }
        }
        return temp;
    }

    public static bool StreamUpdateExists(string path) {
        string expath = AppInfo.Instance.StreamPath + path;
        return File.Exists(expath);
    }


    public static byte[] LoadStreamLazy(string path) {
        if(StreamUpdateExists(path)) {
            return File.ReadAllBytes(AppInfo.Instance.StreamPath + path);
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return File.ReadAllBytes(Application.streamingAssetsPath + FileHelper.Separator + path);
#elif UNITY_ANDROID
        throw new NotImplementedException();
#endif
    }

    public static WWW LoadStreamWWW(string path) {
        string loadUir = string.Empty;
        if(StreamUpdateExists(path)) {
            loadUir = "file://" + AppInfo.Instance.StreamPath + path;
        } else {
            loadUir = Application.streamingAssetsPath + FileHelper.Separator + path;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            loadUir = "file://" + loadUir;
#endif
        }
        return new WWW(loadUir);
    }

}
}
