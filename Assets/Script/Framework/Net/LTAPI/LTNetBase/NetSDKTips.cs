using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace LTUnityPlugin {
public class NetSDKTips {

    private SimpleMyJson.JsonNodeObject tipsJson;

    public NetSDKTips() {
        string tipLoc = FileHelper.Combine("ltplugin", "NetSDKTips_" + FileHelper.GetUsefullLocalization() + ".json");
        byte[] data = FileHelper.LoadStreamLazy(tipLoc);
        tipsJson = SimpleMyJson.Parse(System.Text.Encoding.UTF8.GetString(data)) as SimpleMyJson.JsonNodeObject;
    }

    public string this[string key] {
        get { return this.tipsJson.Get(key).AsString(); }
    }

}
}
