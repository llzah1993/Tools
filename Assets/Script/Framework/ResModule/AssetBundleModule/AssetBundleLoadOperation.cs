/*
 * 封装了AssetBundle的各种加载方式
 */


using UnityEngine;
using System.Collections;

/// <summary>
/// 加载AssetBundle的抽象基类
/// </summary>
public abstract class AssetBundleLoadOperation : IEnumerator
{
	public object Current
	{
		get
		{
			return null;
		}
	}
	public bool MoveNext()
	{
		return !IsDone();
	}
	//
	public void Reset()
	{
	}
	
	abstract public bool Update ();
	
	abstract public bool IsDone ();
}


/// <summary>
/// 编辑器模式，
/// </summary>
public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
{	
	public AssetBundleLoadLevelSimulationOperation ()
	{
	}
	
	public override bool Update ()
	{
		return false;
	}
	
	public override bool IsDone ()
	{		
		return true;
	}
}

/// <summary>
/// 非编辑器模式加载关卡
/// </summary>
public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
	protected string 				m_AssetBundleName;
	protected string 				m_LevelName;
	protected bool 						m_IsAdditive;
	protected string 				m_DownloadingError;
	protected AsyncOperation		m_Request;

	public AssetBundleLoadLevelOperation (string assetbundleName, string levelName, bool isAdditive)
	{
		m_AssetBundleName = assetbundleName;
		m_LevelName = levelName;
		m_IsAdditive = isAdditive;
	}

	public override bool Update ()
	{
		if (m_Request != null)
			return false;
		
		LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
		if (bundle != null)
		{
			if (m_IsAdditive)
				m_Request = Application.LoadLevelAdditiveAsync (m_LevelName);
			else
				m_Request = Application.LoadLevelAsync (m_LevelName);
			return false;
		}
		else
			return true;
	}
	
	public override bool IsDone ()
	{
		// Return if meeting downloading error.
		// m_DownloadingError might come from the dependency downloading.
		if (m_Request == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}
		
		return m_Request != null && m_Request.isDone;
	}
}



/// <summary>
/// 加载AssetBundle资源的抽象基类
/// </summary>
public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
	public abstract T GetAsset<T>() where T : UnityEngine.Object;
}

/// <summary>
/// 编辑器模式加载AssetBundle
/// </summary>
public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
{
	Object							m_SimulatedObject;
	
	public AssetBundleLoadAssetOperationSimulation (Object simulatedObject)
	{
		m_SimulatedObject = simulatedObject;
	}
	
	public override T GetAsset<T>()
	{
		return m_SimulatedObject as T;
	}
	
	public override bool Update ()
	{
		return false;
	}
	
	public override bool IsDone ()
	{
		return true;
	}
}

/// <summary>
/// 非编辑器模式加载AssetBundle
/// </summary>
public class AssetBundleLoadAssetOperationBundle : AssetBundleLoadAssetOperation
{
    protected string m_AssetBundleName;
    protected string m_DownloadingError;
    protected LoadedAssetBundle bundle;
    public AssetBundleLoadAssetOperationBundle(string bundleName)
    {
        m_AssetBundleName = bundleName;
    }

    public override T GetAsset<T>()
    {
        if (bundle != null && typeof(T) == typeof(LoadedAssetBundle))
            return bundle as T;
        else
            return null;
    }

    // Returns true if more Update calls are required.
    public override bool Update()
    {
        LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
        if (bundle != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override bool IsDone()
    {
        if (m_DownloadingError != null)
        {
            Debug.LogError(m_DownloadingError);
        }

        return true;
    }
}

/// <summary>
/// 非编辑器模式加载AssetBundle
/// </summary>
public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
{
	protected string 				m_AssetBundleName;
	protected string 				m_AssetName;
	protected string 				m_DownloadingError;
	protected System.Type 			m_Type;
	protected AssetBundleRequest	m_Request = null;

	public AssetBundleLoadAssetOperationFull (string bundleName, string assetName, System.Type type)
	{
		m_AssetBundleName = bundleName;
		m_AssetName = assetName;
		m_Type = type;
	}
	
	public override T GetAsset<T>()
	{
		if (m_Request != null && m_Request.isDone)
			return m_Request.asset as T;
		else
			return null;
	}
	
	// Returns true if more Update calls are required.
	public override bool Update ()
	{
		if (m_Request != null)
			return false;

		LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
		if (bundle != null)
		{
			m_Request = bundle.m_AssetBundle.LoadAssetAsync (m_AssetName, m_Type);
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public override bool IsDone ()
	{
		// Return if meeting downloading error.
		// m_DownloadingError might come from the dependency downloading.
		if (m_Request == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}

		return m_Request != null && m_Request.isDone;
	}
}

/// <summary>
/// 非编辑器模式加载AssetBundle的manifest
/// </summary>
public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
{
	public AssetBundleLoadManifestOperation (string bundleName, string assetName, System.Type type)
		: base(bundleName, assetName, type)
	{
	}

	public override bool Update ()
	{
		base.Update();
		
		if (m_Request != null && m_Request.isDone)
		{
			AssetBundleManager.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
			return false;
		}
		else
			return true;
	}
}

