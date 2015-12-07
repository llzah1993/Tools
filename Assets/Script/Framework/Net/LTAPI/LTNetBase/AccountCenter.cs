using Framework;
using LTUnityPlugin.WebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LTUnityPlugin {
public class AccountCenter : MonoBehaviour {

    public delegate void AccountCallback(string data);

    private SimpleMyJson.JsonNodeArray accounts;
    private string accountfile;

    private string joyId;
    public string JoyId {
        get {return joyId;}
        set {joyId = value;}
    }

    private string token;
    public string Token {
        get {return token;}
        set {token = value;}
    }

    private uint serverInstanceId;
    public uint ServerInstanceId {
        get { return serverInstanceId; }
        set { serverInstanceId = value; }
    }

    public int Count {
        get {
            return accounts.GetListCount();
        }
    }

    public Account this[int index] {
        get {
            return new Account(accounts.GetArrayItem(accounts.Count - (1 + index)) as SimpleMyJson.JsonNodeObject);
        }
    }

    private AccountCallback callback;

    public void Init(AccountCallback call) {
        callback = call;
        accountfile = AppInfo.Instance.ConfigPath + "Account.txt";
        if(!File.Exists(accountfile)) {
            accounts = new SimpleMyJson.JsonNodeArray();
            Save();
        }
        accounts = Read().Get("accounts") as SimpleMyJson.JsonNodeArray;
    }

    private void Test() {
        Add("3iihl8wu", "kmad8e", "111");
        Add("t8ad8qpb", "84ln7r", "111");
        Add("c9ahaubv", "xiyctr", "111");
        Add("oikh9d05", "bwlbw1", "111");
        Add("ufpjyckz", "jaxelo", "111");
        Add("cd4ll3av", "587vst", "111");
        Add("kvt36imx", "u8ssmc", "111");
        Add("p9rrctuj", "ymjuj8", "111");
        Add("kwg7tte4", "yn7pc9", "111");
        Add("eltzszlf", "k0ia3v", "111");
        Add("n1m5u1qj", "vztiw8", "111");
        Add("s3qk9gae", "r3edm8", "111");
        Add("cuo76f2v", "pimu5j", "111");
        Add("ebutsp6i", "1pvf4r", "111");
        ADebug.Log(this[0].ToString());
    }

    private SimpleMyJson.JsonNodeObject Read() {
        SimpleMyJson.JsonNodeObject json = null;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        json = SimpleMyJson.Parse(File.ReadAllText(accountfile)) as SimpleMyJson.JsonNodeObject;
#elif UNITY_ANDROID
        using(FileStream fs = File.Open(accountfile, FileMode.Open)) {
            json = SimpleMyJsonBinary.Read(fs) as SimpleMyJson.JsonNodeObject;
        }
#endif
        return json;
    }

    public void Save() {
        SimpleMyJson.JsonNodeObject json = new SimpleMyJson.JsonNodeObject();
        json.SetDictValue("accounts", accounts);
        try {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            FileStream fs = new FileStream(accountfile, FileMode.OpenOrCreate);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
#elif UNITY_ANDROID
            FileStream fs = new FileStream(accountfile, FileMode.OpenOrCreate);
            SimpleMyJsonBinary.Write(fs, json);
            fs.Flush();
            fs.Close();
#endif
        } catch(System.Exception ex) {
            ADebug.LogError(ex.ToString());
        }
    }

    public void Add(string user, string pass, string ser) {
        if(string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) {
            return;
        }
        foreach(SimpleMyJson.JsonNodeObject node in accounts) {
            if(node.Get("user").AsString().Equals(user)) {
                accounts.Remove(node);
                break;
            }
        }
        Account acc = new Account(user, pass, ser);
        accounts.Add(acc.ToJson());
        Save();
    }

    public void Del(string user) {
        bool had = false;
        foreach(SimpleMyJson.JsonNodeObject node in accounts) {
            if(node.Get("user").AsString().Equals(user)) {
                accounts.Remove(node);
                had = true;
                break;
            }
        }
        if(had)
            Save();
    }

    public void Login(string use, string pwd) {
        string result = CheckLoginAccount(use, pwd);
        if(!string.IsNullOrEmpty(result)) {
            // 回调
            SimpleMyJson.JsonNodeObject json = new SimpleMyJson.JsonNodeObject();
            json.SetDictValue("code", NetSDK.Key.BI_ACTION_LOGIN_FAIL);
            json.SetDictValue("info", result);
            json.SetDictValue("data", string.Empty);
            callback(json.ToString());
            return;
        }
        Add(use, pwd, serverInstanceId.ToString());
        StartCoroutine(LoginI(use, pwd));
    }

    private IEnumerator LoginI(string use, string pwd) {
        SimpleMyJson.JsonNodeObject result = new SimpleMyJson.JsonNodeObject();
        HttpWebClient hw = HttpHelper.LoadStringAsync(AppInfo.Instance.GetStr(AppInfo.Key.UserServer) + "login?uname=" + use + "&password=" + pwd, System.Text.Encoding.UTF8, delegate(object sender, HttpDownloadStringCompletedEventArgs e) {
            if(e.Error != null) {
                result.SetDictValue("code", NetSDK.Key.BI_ACTION_LOGIN_FAIL);
                result.SetDictValue("info", e.Error.Message);
                result.SetDictValue("data", string.Empty);
            } else {
                string checkRep = IsNetworkSuccess(e.Result);
                if(string.IsNullOrEmpty(checkRep)) {
                    result.SetDictValue("code", NetSDK.Key.BI_ACTION_LOGIN_SUCCESS);
                    result.SetDictValue("info", "ok");
                    result.SetDictValue("data", e.Result);
                } else {
                    result.SetDictValue("code", NetSDK.Key.BI_ACTION_LOGIN_FAIL);
                    result.SetDictValue("info", checkRep);
                    result.SetDictValue("data", e.Result);
                }
            }
        });
        yield return StartCoroutine(hw.Coroutine);
        callback(result.ToString());
    }

    public void QuickReg() {
        StartCoroutine(QuickRegI());
    }

    private IEnumerator QuickRegI() {
        SimpleMyJson.JsonNodeObject result = new SimpleMyJson.JsonNodeObject();
        HttpWebClient hw = HttpHelper.LoadStringAsync(AppInfo.Instance.GetStr(AppInfo.Key.UserServer) + "quickReg", System.Text.Encoding.UTF8, delegate(object sender, HttpDownloadStringCompletedEventArgs e) {
            if(e.Error != null) {
                result.SetDictValue("code", NetSDK.Key.BI_ACTION_QUICKREG_FAIL);
                result.SetDictValue("info", e.Error.Message);
                result.SetDictValue("data", string.Empty);
            } else {
                string checkRep = IsNetworkSuccess(e.Result);
                if(string.IsNullOrEmpty(checkRep)) {
                    result.SetDictValue("code", NetSDK.Key.BI_ACTION_QUICKREG_SUCCESS);
                    result.SetDictValue("info", "ok");
                    result.SetDictValue("data", e.Result);
                } else {
                    result.SetDictValue("code", NetSDK.Key.BI_ACTION_QUICKREG_FAIL);
                    result.SetDictValue("info", checkRep);
                    result.SetDictValue("data", e.Result);
                }
            }
        });
        yield return StartCoroutine(hw.Coroutine);
        callback(result.ToString());
    }

    public void Regist(string usename, string password, string surePass) {
        string result = CheckRegistSuccess(usename, password, surePass);
        if(!string.IsNullOrEmpty(result)) {
            // 回调
            SimpleMyJson.JsonNodeObject json = new SimpleMyJson.JsonNodeObject();
            json.SetDictValue("code", NetSDK.Key.BI_ACTION_REG_FAIL);
            json.SetDictValue("info", result);
            json.SetDictValue("data", string.Empty);
            callback(json.ToString());
            return;
        }
        StartCoroutine(RegistI(usename, password, surePass));
    }

    private IEnumerator RegistI(string usename, string password, string surePass) {
        SimpleMyJson.JsonNodeObject result = new SimpleMyJson.JsonNodeObject();
        HttpWebClient hw = HttpHelper.LoadStringAsync(AppInfo.Instance.GetStr(AppInfo.Key.UserServer) + "reg?uname=" + usename + "&nname=tzbd" + "&password=" + password, System.Text.Encoding.UTF8, delegate(object sender, HttpDownloadStringCompletedEventArgs e) {
            if(e.Error != null) {
                result.SetDictValue("code", NetSDK.Key.BI_ACTION_REG_FAIL);
                result.SetDictValue("info", e.Error.Message);
                result.SetDictValue("data", "{\"status\":-999,\"msg\":\"\"}");
            } else {
                string checkRep = IsNetworkSuccess(e.Result);
                if(string.IsNullOrEmpty(checkRep)) {
                    result.SetDictValue("code", NetSDK.Key.BI_ACTION_REG_SUCCESS);
                    result.SetDictValue("info", "ok");
                    result.SetDictValue("data", e.Result);
                } else {
                    result.SetDictValue("code", NetSDK.Key.BI_ACTION_REG_FAIL);
                    result.SetDictValue("info", checkRep);
                    result.SetDictValue("data", e.Result);
                }
            }
        });
        yield return StartCoroutine(hw.Coroutine);
        callback(result.ToString());
    }

    private string IsNetworkSuccess(string data) {
        SimpleMyJson.JsonNodeObject obj = SimpleMyJson.Parse(data) as SimpleMyJson.JsonNodeObject;
        int status = obj.asDict()["status"].AsInt();
        string umsg = obj.asDict()["msg"].AsString();
        if(status <= 0) {
            return umsg;
        }
        return string.Empty;
    }

    private string CheckRegistSuccess(string usename, string password, string surePass) {
        if(usename.Length == 0 || password.Length == 0) {
            return PluginManager.PluginInstance<NetSDK>().Tips["use_or_pwd_null"];
        }
        int acountResult = IsAccountName(usename);
        if(acountResult == 2) {
            return PluginManager.PluginInstance<NetSDK>().Tips["use_unvalid_0"];
        } else if(acountResult == 0 || acountResult == -1) {
            return PluginManager.PluginInstance<NetSDK>().Tips["use_unvalid"];
        }
        if(!NumAndchar(password)) {
            return PluginManager.PluginInstance<NetSDK>().Tips["pwd_unvalid"];
        }
        int comparePass = password.CompareTo(surePass);
        if(comparePass != 0) {
            return PluginManager.PluginInstance<NetSDK>().Tips["pwd_not_same"];
        }
        return string.Empty;
    }

    private string CheckLoginAccount(string usename, string password) {
        if(usename.Length == 0 || password.Length == 0) {
            return PluginManager.PluginInstance<NetSDK>().Tips["use_or_pwd_null"];
        }

        int acountResult = IsAccountName(usename);
        if(acountResult == 0 || acountResult == -1 || acountResult == 2) {
            return PluginManager.PluginInstance<NetSDK>().Tips["use_unvalid"];
        }

        if(!NumAndchar(password)) {
            return PluginManager.PluginInstance<NetSDK>().Tips["pwd_unvalid"];
        }
        return string.Empty;
    }

    private int IsAccountName(string account) {
        int num = 0;
        for(int i = 0; i < account.Length; i++) {
            char c = account[i];
            if(i == 0) {
                if(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9') {
                    num++;
                } else if(c == '_') {
                    return 2;
                } else {
                    return 0;
                }
            } else {
                if(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_') {
                    num++;
                } else {
                    return 0;
                }
            }
        }
        if(num == account.Length) {
            return 1;
        }
        return -1;
    }

    private static bool NumAndchar(string password) {
        for(int i = 0; i < password.Length;) {
            char c = password[i];
            if((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                i++;
            } else {
                return false;
            }
        }
        return true;
    }
}

public class Account {
    private string username;
    public string Username {
        get { return username;}
    }
    private string password;
    public string Password {
        get { return password; }
    }
    private string server;
    public string Server {
        get { return server; }
    }

    public Account(string user, string pass, string ser) {
        username = user;
        password = pass;
        server = ser;
    }

    public Account(SimpleMyJson.JsonNodeObject json) {
        username = json.Get("user").AsString();
        password = json.Get("pwd").AsString();
        server = json.Get("server").AsString();
    }

    public SimpleMyJson.JsonNodeObject ToJson() {
        SimpleMyJson.JsonNodeObject node = new SimpleMyJson.JsonNodeObject();
        node.SetDictValue("user", username);
        node.SetDictValue("pwd", password);
        node.SetDictValue("server", server);
        return node;
    }

    public override string ToString() {
        return ToJson().ToString();
    }
}
}
