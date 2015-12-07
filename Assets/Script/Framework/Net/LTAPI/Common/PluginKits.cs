using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LTUnityPlugin {
public class PluginKits {
    private static readonly string JNIClass = "com/ltgame/netgame/unity/tools/Kits";

    public static void ShowToast(string text) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public static void showTestToast(string text) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public static void RestartApp() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public static void FinishApp() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#elif UNITY_ANDROID
#endif
    }

    public static string GetApkSrc() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return string.Empty;
#elif UNITY_ANDROID
		return string.Empty;
#endif
    }

    public static string GetDeviceInfo() {
        return "{\"uuid\":\"5b7dfa7e-34d2-4ac7-acec-d39c87e2b7b8\",\"mobileOpr\":\"SIMULATOR\",\"country\":\"CN\",\"language\":\"zh\",\"deviceId\":\"SIMULATOR\",\"deviceMode\":\"SIMULATOR\",\"netState\":\"UNKNOW\",\"memory\":8888,\"screenSize\":\"1280x720\",\"osVersion\":\"WinSimulator\"}";
    }

    public enum NetWorkStatus {
        NETWORKTYPE_INVALID, NETWORKTYPE_MOBILE, NETWORKTYPE_WIFI
    }

    public static NetWorkStatus GetNetWorkType() {
        return NetWorkStatus.NETWORKTYPE_WIFI;
    }

    public class SDState {
        public static readonly string MEDIA_UNKNOWN = "unknown";
        public static readonly string MEDIA_REMOVED = "removed";
        public static readonly string MEDIA_UNMOUNTED = "unmounted";
        public static readonly string MEDIA_CHECKING = "checking";
        public static readonly string MEDIA_NOFS = "nofs";
        public static readonly string MEDIA_MOUNTED = "mounted";
        public static readonly string MEDIA_MOUNTED_READ_ONLY = "mounted_ro";
        public static readonly string MEDIA_SHARED = "shared";
        public static readonly string MEDIA_BAD_REMOVAL = "bad_removal";
        public static readonly string MEDIA_UNMOUNTABLE = "unmountable";
    }

    public static string GetSDState() {
        return SDState.MEDIA_MOUNTED;
    }


    public static string GetSDPath() {
        return Application.dataPath;
    }


    public static void OpenSetting() {
    }

    public static void InstallAPK(string apk) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }
}
}
