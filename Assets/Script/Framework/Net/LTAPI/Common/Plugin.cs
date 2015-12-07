using UnityEngine;
using System.Collections;
using System;

namespace LTUnityPlugin {
/// <summary>
/// By Simon.H For LT NetGame Unity plugin.
/// —— 2015.08
/// </summary>
public abstract class Plugin : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public abstract string JNIClass {get;}
    /// <summary>
    ///
    /// </summary>
    /// <param name="callback">sendUnityMessage(callback) from java</param>
    /// <param name="extend">maybe json or other string data as you i</param>
    public void InitBind(string callback, string extend) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
        //IntPtr classId = AndroidJNI.FindClass(JNIClass);
        //IntPtr instance = Instance();
        //IntPtr bindUnity = AndroidJNI.GetMethodID(classId, "bindUnity", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
        //IntPtr arg0 = AndroidJNI.NewStringUTF(name);
        //IntPtr arg1 = AndroidJNI.NewStringUTF(callback);
        //IntPtr arg2 = AndroidJNI.NewStringUTF(extend);
        //AndroidJNI.CallVoidMethod(instance, bindUnity, new jvalue[] {
        //    new jvalue{l = arg0},
        //    new jvalue{l = arg1},
        //    new jvalue{l = arg2}
        //});
        //AndroidJNI.DeleteLocalRef(classId);
        //AndroidJNI.DeleteLocalRef(instance);
        //AndroidJNI.DeleteLocalRef(arg0);
        //AndroidJNI.DeleteLocalRef(arg1);
        //AndroidJNI.DeleteLocalRef(arg2);
#endif
    }

    /// <summary>
    /// eng... must implement getInstance in java !!!!
    /// </summary>
    /// <returns></returns>
    public IntPtr Instance() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return IntPtr.Zero;
#elif UNITY_ANDROID
        //IntPtr classId = AndroidJNI.FindClass(JNIClass);
        //IntPtr getInstance = AndroidJNI.GetStaticMethodID(classId, "getInstance", "()L" + JNIClass + ";");
        //IntPtr instance = AndroidJNI.CallStaticObjectMethod(classId, getInstance, new jvalue[] { });
        //AndroidJNI.DeleteLocalRef(classId);
        //return instance;
		return IntPtr.Zero;;
#endif
    }
}
}