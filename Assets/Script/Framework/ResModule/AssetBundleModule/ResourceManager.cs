using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {

    private static ResourceManager instance = null;

    public static ResourceManager Instance
    {
        get
        {
            if (null == instance)
            {
                GameObject go = new GameObject("ResourceManager");
                instance = go.AddComponent<ResourceManager>();
                return instance;
            }
            else
            {
                return instance;
            }
        }
        
    }

    /// <summary>
    /// 是否检测资源服版本
    /// </summary>
    private bool ischeckremote = true;
    public bool isCheckremote
    {
        set
        {
            ischeckremote = value;
        }
        get
        {
            return ischeckremote;
        }
    }

	// Use this for initialization
	void Start () 
    {
        DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
    {
        ResourceUpdater.Instance.UpdateTaskList();
	}

    private void Initialize()
    {
        ResourceUpdater.Instance.StartCheckUpdate("", "", OnVertionCheckComplete, ischeckremote);
    }

    private void OnVertionCheckComplete()
    {
        //生成资源更新清单，并下载资源

    }

    private void OnUpdateComplete()
    {
        //载入资源
    }
}
