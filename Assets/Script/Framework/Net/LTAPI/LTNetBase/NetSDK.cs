using UnityEngine;
using System.Collections;
using System;
using LTUnityPlugin.WebClient;
using LTNet;
using Framework;

namespace LTUnityPlugin {

public class NetSDK : Plugin {
    public delegate void NetSDKEmptyDelegate();

    public NetSDKEmptyDelegate exitCallback;
    public NetSDKEmptyDelegate backkeyCallback;

    private NetSDKTips tips;
    public NetSDKTips Tips {
        get { return tips;}
    }

    void Awake() {
        InitBind("NETSDKCallback", string.Empty);
        tips = new NetSDKTips();
        backkeyCallback += Quit;
        PluginManager.PluginInstance<AccountCenter>().Init(NETSDKCallback);
    }

    void Start() {
    }

    public void Init() {

    }

    void Update() {
        // 返回键 Windows esc
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(backkeyCallback != null) {
                backkeyCallback();
            }
        }
    }

    public override string JNIClass {
        get {
            return "com/ltgame/netgame/unity/plugin/ltsdk/NetSDK";
        }
    }

    public bool IsUseLTSDK() {
        return true;
    }

    public bool IsAutoLogin() {
        return false;
    }

    public void Login() {
        {
            if(PluginManager.PluginInstance<AccountCenter>().Count == 0) {
                SimpleMyJson.JsonNodeObject result = new SimpleMyJson.JsonNodeObject();
                result.SetDictValue("code", NetSDK.Key.BI_ACTION_LOGIN_FAIL);
                result.SetDictValue("info", tips["reg_please"]);
                result.SetDictValue("data", string.Empty);
                NETSDKCallback(result.ToString());
            } else {
                Account acount = PluginManager.PluginInstance<AccountCenter>()[0];
                PluginManager.PluginInstance<AccountCenter>().Login(acount.Username, acount.Password);
            }
        }
    }

    public void SwitchAccount() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void Logout() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void OpenUserCenter() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void ShowFloat() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void HideFloat() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
        IntPtr classId = AndroidJNI.FindClass(JNIClass);
        IntPtr instance = Instance();
        IntPtr hideFloat = AndroidJNI.GetMethodID(classId, "hideFloat", "()V");
        AndroidJNI.CallVoidMethod(instance, hideFloat, new jvalue[] { });
        AndroidJNI.DeleteLocalRef(classId);
        AndroidJNI.DeleteLocalRef(instance);
#endif
    }

    public void Quit() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        SimpleMyJson.JsonNodeObject json = new SimpleMyJson.JsonNodeObject();
        json.SetDictValue("code", NetSDK.Key.ACTION_QUIT_CUSTOM);
        json.SetDictValue("info", string.Empty);
        json.SetDictValue("data", string.Empty);
        NETSDKCallback(json.ToString());
#elif UNITY_ANDROID
#endif
    }

    public void SetLoginInfo(string roleInfoJson) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void SetCidengInfo(string cideng) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void Pay(string payInfoJson) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
#endif
    }

    public void NETSDKCallback(string jsonstr) {
        SimpleMyJson.JsonNodeObject json = SimpleMyJson.Parse(jsonstr) as SimpleMyJson.JsonNodeObject;
        int callkey = json.Get<int>("code", 0);
        string info = json.Get<string>("info", string.Empty);
        string data = json.Get<string>("data", string.Empty);
        switch(callkey) {
            case NetSDK.Key.BI_ACTION_QUICKREG_SUCCESS:
            case NetSDK.Key.BI_ACTION_REG_SUCCESS: {
                    SimpleMyJson.JsonNodeObject result = SimpleMyJson.Parse(data) as SimpleMyJson.JsonNodeObject;
                    string name = result["content"].asDict()["uname"].AsString();
                    string pass = result["content"].asDict()["password"].AsString();
                    PluginManager.PluginInstance<AccountCenter>().Login(name, pass);
                }
                break;
            case NetSDK.Key.BI_ACTION_LOGIN_SUCCESS: {
                    SimpleMyJson.JsonNodeObject result = SimpleMyJson.Parse(data) as SimpleMyJson.JsonNodeObject;
                    string joyid = result["content"].asDict()["uid"].AsString();
                    string token = result["content"].asDict()["token"].AsString();
                    PluginManager.PluginInstance<AccountCenter>().JoyId = joyid;
                    PluginManager.PluginInstance<AccountCenter>().Token = token;
                    string[] serurl = AppInfo.Instance.GetStr(AppInfo.Key.ServerUrl).Split(':');
                    NetworkManager.Connector.BindSocket(new NetSocket(serurl[0], int.Parse(serurl[1])));
                    NetworkManager.Connector.UsingAsync = true;
                    NetworkManager.Connector.RegisterConnectCallback(delegate(SocketConnector connector, object userdata) {
                        if(connector.IsConnected()) {
                            Debug.Log("connected successfully");
                        } else {
                            Debug.Log("Connection failed");
                        }
                    }, null);
                    NetworkManager.Connector.Connect();
                }
                break;
            case NetSDK.Key.BI_ACTION_QUICKREG_FAIL:
            case NetSDK.Key.BI_ACTION_REG_FAIL:
            case NetSDK.Key.BI_ACTION_LOGIN_FAIL:
                ADebug.LogError(info);
                break;
            case NetSDK.Key.ACTION_QUIT_CUSTOM:
                if(exitCallback != null) {
                    exitCallback();
                }
                break;
        }
    }

    public class Key {
        public static readonly string LtJoyId = "LtJoyId";
        public static readonly string LtAppId = "LtAppId";
        public static readonly string LtInstantId = "LtInstantId";
        public static readonly string LtInstantAlias = "LtInstantAlias";
        public static readonly string LtReserve = "LtReserve";
        public static readonly string AppName = "AppName";
        public static readonly string RoleId = "RoleId";
        public static readonly string RoleName = "RoleName";
        public static readonly string RoleLevel = "RoleLevel";
        public static readonly string RoleFighting = "RoleFighting";
        public static readonly string RoleVipLevel = "RoleVipLevel";
        public static readonly string RoleBalance = "RoleBalance";
        public static readonly string RolePartyName = "RolePartyName";
        public static readonly string ProductId = "ProductId";
        public static readonly string ProductName = "ProductName";
        public static readonly string ProductIcon = "ProductIcon";
        public static readonly string ProductDescript = "ProductDescript";
        public static readonly string MoneyAmount = "MoneyAmount";
        public static readonly string AppExt1 = "AppExt1";
        public static readonly string AppExt2 = "AppExt2";

        ///

        public const int ACTION_INIT_SUCCESS = 100;
        public const int ACTION_INIT_FAIL = 101;
        public const int ACTION_LOGIN_SUCCESS = 110;
        public const int ACTION_LOGIN_FAIL = 111;
        public const int ACTION_LOGIN_CANCEL = 112;
        public const int ACTION_PLATFORMACCOUNTSWITCH_SUCCESS = 120;
        public const int ACTION_PLATFORMACCOUNTSWITCH_FAIL = 121;
        public const int ACTION_PLATFORMACCOUNTSWITCH_CANCEL = 122;
        public const int ACTION_QUIT_SUCCESS = 130;
        public const int ACTION_QUIT_FAIL = 131;
        public const int ACTION_QUIT_CANCEL = 132;
        public const int ACTION_QUIT_CUSTOM = 133;
        public const int ACTION_PAY_SUCCESS = 140;
        public const int ACTION_PAY_FAIL = 141;
        public const int ACTION_PAY_CANCEL = 142;
        public const int ACTION_PAY_NOW = 143;
        public const int ACTION_LOGOUT_SUCCESS = 150;
        public const int ACTION_LOGOUT_FAIL = 151;
        public const int USERTYPE_MATURE = 1;
        public const int USERTYPE_IMMATURE = 2;
        public const int USERTYPE_UNKNOWN = 3;

        // built in call back.
        public const int BI_ACTION_REG_SUCCESS = 8880;
        public const int BI_ACTION_REG_FAIL = 8881;
        public const int BI_ACTION_QUICKREG_SUCCESS = 8882;
        public const int BI_ACTION_QUICKREG_FAIL = 8883;
        public const int BI_ACTION_LOGIN_SUCCESS = 8884;
        public const int BI_ACTION_LOGIN_FAIL = 8885;
    }
}
}