using UnityEngine;
using System;

namespace LTUnityPlugin {
public class NetSDKDemo : MonoBehaviour {
    public uint ServerInstance = 0x3001;
    void Start() {
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            PluginManager.PluginInstance<NetSDK>().Quit();
        }
    }

    void OnGUI() {
        float scale = 1.0f;

        if(Application.platform == RuntimePlatform.IPhonePlayer) {
            scale = Screen.width / 320;
        }

        float btnWidth = 200 * scale;
        float btnHeight = 45 * scale;
        float btnTop = 10 * scale;
        GUI.skin.button.fontSize = Convert.ToInt32(16 * scale);

        btnTop += btnHeight + 10 * scale;
        if(GUI.Button(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), "快速注册&登录")) {
            PluginManager.PluginInstance<AccountCenter>().ServerInstanceId = ServerInstance;
            PluginManager.PluginInstance<NetSDK>().Init();
            if(PluginManager.PluginInstance<AccountCenter>().Count == 0) {
                PluginManager.PluginInstance<AccountCenter>().QuickReg();
            } else {
                Account acount = PluginManager.PluginInstance<AccountCenter>()[0];
                PluginManager.PluginInstance<AccountCenter>().Login(acount.Username, acount.Password);
            }
        }

        btnTop += btnHeight + 10 * scale;
        if(GUI.Button(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), "测试心跳")) {
            LiteRequest enterReq = new LiteRequest(NetProtocols.C_HEART_BEAT);
            enterReq.Send(NetworkManager.Connector);
        }
    }
}
}