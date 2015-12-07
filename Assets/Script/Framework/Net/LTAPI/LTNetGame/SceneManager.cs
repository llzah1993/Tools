using UnityEngine;
using System.Collections;
using System;

public class SceneManager : MonoBehaviour {

    public static SceneManager Instance;
    AsyncOperation asyncOperation;
    Action LoadComplete;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void LoadScene(string name ,Action cb)
    {
        LoadComplete = cb;
        StartCoroutine("loadScene", name);
    }

    void OnGUI()
    {
        //判断异步对象并且异步对象没有加载完毕，显示进度  
        if (asyncOperation != null && !asyncOperation.isDone)
        {
            GUILayout.Label("progress:" + (float)asyncOperation.progress * 100 + "%");
        }
    }

    IEnumerator loadScene(string sceneName)
    {
        yield return asyncOperation = Application.LoadLevelAsync(sceneName);
        LoadComplete();
    }  
}
