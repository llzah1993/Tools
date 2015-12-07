using UnityEngine;
using System.Collections.Generic;
using System;

namespace LTUnityPlugin {
[DisallowMultipleComponent]
public class PluginManager : MonoBehaviour {
    private static PluginManager container = null;
    private Dictionary<string, object> singletonMap = null;
    public string[] InitPlugin;
    // 更新后的主场景
    public string MainScene;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        container = this;
        singletonMap = new Dictionary<string, object>();
        for(int i = 0; i < InitPlugin.Length; i++) {
            Type t = Type.GetType(InitPlugin[i]);
            if(t != null) {
                AddInitPluginInstancePrivate(t);
            }
        }
    }

    void Update() {
    }

    void OnApplicationQuit() {
        if(container != null) {
            Destroy(container);
            container = null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private static PluginManager CheckInstance() {
        if(container == null) {
            container = new GameObject("PluginManager").AddComponent<PluginManager>();
            container.singletonMap = new Dictionary<string, object>();
        }
        return container;
    }

    /// <summary>
    /// 内部调用 应该只有初始化的时候会用到
    /// </summary>
    /// <param name="type"></param>
    private MonoBehaviour AddInitPluginInstancePrivate(Type type) {
        if(!singletonMap.ContainsKey(type.Name)) {
            singletonMap.Add(type.Name, gameObject.AddComponent(type));
        }
        return singletonMap[type.Name] as MonoBehaviour;
    }

    private T AddPluginInstancePrivate<T>() where T : MonoBehaviour {
        if(!singletonMap.ContainsKey(typeof(T).Name)) {
            singletonMap.Add(typeof(T).Name, gameObject.AddComponent(typeof(T)));
        }
        return (T)singletonMap[typeof(T).Name];
    }

    private void RemovePluginInstancePrivate<T>() where T : MonoBehaviour {
        if(container != null && singletonMap.ContainsKey(typeof(T).Name)) {
            Destroy((UnityEngine.Object)(singletonMap[typeof(T).Name]));
            singletonMap.Remove(typeof(T).Name);
        }
    }

    private bool HadPluginInstancePrivate<T>() where T : MonoBehaviour {
        if(container == null) {
            return false;
        }
        if(singletonMap != null && singletonMap.ContainsKey(typeof(T).Name)) {
            return true;
        }
        return false;
    }

    /************************************************************************/
    /* PLBLIC static Mth                                                    */
    /************************************************************************/
    public static string GetMainScene() {
        return container.MainScene;
    }

    public static T PluginInstance<T>() where T : MonoBehaviour {
        return container.AddPluginInstancePrivate<T>();
    }

    public static void RemovePluginInstance<T>() where T : MonoBehaviour {
        container.RemovePluginInstancePrivate<T>();
    }

    public static bool HadPluginInstance<T>() where T : MonoBehaviour {
        return container.HadPluginInstancePrivate<T>();
    }
}
}