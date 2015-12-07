using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

namespace LTUnityPlugin {

    class MainDemo : MonoBehaviour {

        public uint ServerInstance = 0x3001;

        void Start() {
            DeviceInfo.Instance.Init();

            AppInfo.Instance.Init();

            PluginManager.PluginInstance<AccountCenter>().ServerInstanceId = ServerInstance;

            PluginManager.PluginInstance<NetSDK>().Init();

        }

        void OnGUI() {

            float scale = 1.0f;

            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                scale = Screen.width / 320;
            }

            float btnWidth = 200 * scale;
            float btnHeight = 45 * scale;
            float btnTop = 10 * scale;
            GUI.skin.button.fontSize = Convert.ToInt32(16 * scale);

            btnTop += btnHeight + 10 * scale;
            if (GUI.Button(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), "初始化&加载主场景")) {
                //Account acount = PluginManager.PluginInstance<AccountCenter>()[0];
                //PluginManager.PluginInstance<AccountCenter>().Login(acount.Username, acount.Password);
                PluginManager.PluginInstance<AccountCenter>().QuickReg();

                //Process myProcess = new Process();

                //try {
                //    myProcess.StartInfo.UseShellExecute = false;
                //    myProcess.StartInfo.FileName = @"C:\Users\zhenyang\AppData\Local\Youdao\Dict\Application\YodaoDict.exe";
                //    myProcess.Start();
                //}
                //catch (Exception e) {
                //    Console.WriteLine(e.Message);
                //}

            }

            btnTop += btnHeight + 10 * scale;
            if (GUI.Button(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), "发送协议"))
            {
                LiteRequest enterReq = new LiteRequest(NetProtocols.HANDLE_PROTO_MAP_VIEW, "lhhb",
                new object[] { DateTime.Now.Ticks, (int)(UnityEngine.Random.value * 1024), (int)(UnityEngine.Random.value * 1024), 11 }); //发送当前时间戳
                enterReq.Send(NetworkManager.Connector);
            }

            //btnTop += btnHeight + 10 * scale;
            //string str = "PostfixExpression";
            //GUI.TextField(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), str );

            btnTop += btnHeight + 10 * scale;
            if (GUI.Button(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), "计算后缀表达式"))
            {
                ProtoExpression postfixExpression = new ProtoExpression();
                postfixExpression.rpnStream = new string[] { "-10", "2", "/" ,"1" ,"abs","()"};
                float result = Convert.ToSingle(postfixExpression.RPNEvaluate());

            }

            //btnTop += btnHeight + 10 * scale;
            //if (GUI.Button(new Rect((Screen.width - btnWidth) / 2, btnTop, btnWidth, btnHeight), "计算后缀表达式"))
            //{
            //    ProtoExpression postfixExpression = new ProtoExpression();
            //    postfixExpression.rpnStream = new string[] { "-10", "2", "/", "1", "abs", "()" };
            //    float result = Convert.ToSingle(postfixExpression.RPNEvaluate());

            //}

        }


    }

}
