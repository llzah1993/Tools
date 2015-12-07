/*
 用于远程热更新资源，暂不支持脚本
 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ResourceUpdater{

    private static ResourceUpdater instance = null;

    public static ResourceUpdater Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new ResourceUpdater();
                return instance;
            }
            else
            {
                return instance;
            }
        }
        set
        {
 
        }
    }

    /// <summary>
    /// 本地url
    /// </summary>
    string localUrl = 
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_STANDALONE_WIN
		"file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
		"jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
		Application.dataPath + "/Raw/";
#endif

	/// <summary>
	/// 缓存url
	/// </summary>
    string cachUrl = Application.persistentDataPath + "/";

	/// <summary>
	/// 资源服 
	/// </summary>
    string serverUrl = string.Empty;

    /// <summary>
    /// 下载任务
    /// </summary>
    class DonwloadTask
    {
        public DonwloadTask(string url, string tag, Action<WWW, string> onloadcomplete)
        {
            this.url = url;
            this.tag = tag;
            this.onLoadComplete = onloadcomplete;
        }

        public string url;
        public string tag;
        public Action<WWW, string> onLoadComplete;
    }

    /// <summary>
    ///下载任务管理
    /// </summary>
    class DonwloadTaskManager
    {
        public DonwloadTaskManager(DonwloadTask task)
        {
            this.task = task;
            this.www = new WWW(this.task.url);
        }
        public WWW www;
        public DonwloadTask task;
    }

    public LocalResoure localRes
    {
        get;
        set;
    }

    public RemoteResource remoteRes
    {
        get;
        set;
    }

	/// <summary>
	/// 下载队列
	/// </summary>
    Queue<DonwloadTask> tasks = new Queue<DonwloadTask>();

	/// <summary>
	/// 下载列表
	/// </summary>
    List<DonwloadTaskManager> runners = new List<DonwloadTaskManager>();

    /// <summary>
    /// 开始检测版本
    /// </summary>
    /// <param name="url"></param>
    /// <param name="tag"></param>
    /// <param name="oncheckvertioncomplete"></param>
    /// <param name="ischeckremote">是否检测资源服</param>
    public void StartCheckUpdate(string url, string tag, Action oncheckvertioncomplete, bool ischeckremote)
    {
        Action<Exception> oncomlele = (err) =>
        {
            if (null != err)
            {
                Debug.Log("Some error exists");
                return;
            }
            else
            {
                //合并远程服务器版本文件与本地版本文件
            }
        };

        Action onlocalinitcomplete = () =>
        {
            //开始检测资源服
            if (ischeckremote)
            {
                //remoteRes.Initialize(url, onlocalinitcomplete);
            }
        };

        localRes.Initialize(url, onlocalinitcomplete);
    }

    void StartDownLoadTaskRequest(string url, string tag, Action<WWW, string> oncomleteaction)
    {
        DonwloadTask task = new DonwloadTask(url, tag, oncomleteaction);
        tasks.Enqueue(task);
    }


    public void UpdateTaskList()
    {
        if (tasks.Count > 0 && runners.Count == 0)
        {
            runners.Add(new DonwloadTaskManager(tasks.Dequeue()));
        }

        foreach (DonwloadTaskManager mtask in runners)
        {
            if (mtask.www.isDone)
            {
                mtask.task.onLoadComplete(mtask.www, string.Empty);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="localurl"></param>
    /// <param name="tag"></param>
    /// <param name="oninitcomlete"></param>
    public void DownLoadFromurl(string url, string tag, Action<WWW, string> oninitcomlete)
    {
        StartDownLoadTaskRequest(url, tag, oninitcomlete);
    }


}
