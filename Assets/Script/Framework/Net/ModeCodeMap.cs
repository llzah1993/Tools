using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

public class ModeCodeMap : MonoBehaviour {

    public static Dictionary<ushort, DataModeHandler> modeCodeMapDic;
    
	void Start () {
        modeCodeMapDic = new Dictionary<ushort, DataModeHandler>();
        Initialize();
	}

    public void Initialize()
    {
        modeCodeMapDic.Clear();
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].BaseType == typeof(DataModeHandler))
            {
                DataModeHandler mt = Activator.CreateInstance(types[i]) as DataModeHandler;
                modeCodeMapDic.Add(mt.DataModeCode, mt);
            }
        }

        //string path = Application.dataPath + "/Script/DataModeHandlerClass";
        //string[] classEntries = this.GetDataModeHandlerClassEntries(path);
        //for (int i = 0; i < classEntries.Length; i++)
        //{
        //    string className = classEntries[i].Substring(classEntries[i].LastIndexOf("\\") + 1);
        //    className = className.Substring(0, className.IndexOf("."));
        //    Type type = Type.GetType("." + className, true, true);  //new ModeCodeTest1().GetType().Namespace + "." + className
        //    DataModeHandler mt = Activator.CreateInstance(type) as DataModeHandler;
        //    ushort dataModeCode = mt.DataModeCode;
        //    modeCodeMapDic.Add(dataModeCode, mt);
        //}

    }

    //Type[] modecodes = new Type[] {
    //    typeof(ModeCodeTest1),
    //    typeof(ModeCodeTest2),
    //};

    //private string[] GetDataModeHandlerClassEntries(string path)
    //{
    //    string[] classEntries = Directory.GetFiles(path, "*.cs", SearchOption.TopDirectoryOnly);
    //    return classEntries;
    //}
}

