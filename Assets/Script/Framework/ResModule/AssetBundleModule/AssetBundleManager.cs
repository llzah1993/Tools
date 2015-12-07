using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/*
 	In this demo, we demonstrate:
	1.	Automatic asset bundle dependency resolving & loading.
		It shows how to use the manifest assetbundle like how to get the dependencies etc.
	2.	Automatic unloading of asset bundles (When an asset bundle or a dependency thereof is no longer needed, the asset bundle is unloaded)
	3.	Editor simulation. A bool defines if we load asset bundles from the project or are actually using asset bundles(doesn't work with assetbundle variants for now.)
		With this, you can player in editor mode without actually building the assetBundles.
	4.	Optional setup where to download all asset bundles
	5.	Build pipeline build postprocessor, integration so that building a player builds the asset bundles and puts them into the player data (Default implmenetation for loading assetbundles from disk on any platform)
	6.	Use WWW.LoadFromCacheOrDownload and feed 128 bit hash to it when downloading via web
		You can get the hash from the manifest assetbundle.
	7.	AssetBundle variants. A prioritized list of variants that should be used if the asset bundle with that variant exists, first variant in the list is the most preferred etc.
*/

// 带有引用计数的LoadedAssetBundle
public class LoadedAssetBundle
{
	public AssetBundle m_AssetBundle;
	public int m_ReferencedCount;
	
	public LoadedAssetBundle(AssetBundle assetBundle)
	{
		m_AssetBundle = assetBundle;
		m_ReferencedCount = 1;
	}
}

public class AssetBundleManager : MonoBehaviour
{
	static string m_BaseDownloadingURL = "";

	static string[] m_Variants =  {  };

	static AssetBundleManifest m_AssetBundleManifest = null;

#if UNITY_EDITOR

	static int m_SimulateAssetBundleInEditor = -1;

	const string kSimulateAssetBundles = "SimulateAssetBundles";

#endif

    /// <summary>
    /// 已经载入内存的AssetBundle
    /// </summary>
	static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle> ();

    /// <summary>
    /// 正在载入内存过程的WWW资源
    /// </summary>
	static Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW> ();

    /// <summary>
    /// 正在载入过程中产生的错误信息
    /// </summary>
	static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();

    /// <summary>
    /// 正在进行中的加载
    /// </summary>
	static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation> ();

    /// <summary>
    /// assetbundle依赖的资源
    /// </summary>
	static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();

	// 
	public static string BaseDownloadingURL
	{
		get { return m_BaseDownloadingURL; }
		set { m_BaseDownloadingURL = value; }
	}

	// 
	public static string[] Variants
	{
		get { return m_Variants; }
		set { m_Variants = value; }
	}

	// 
	public static AssetBundleManifest AssetBundleManifestObject
	{
		set {m_AssetBundleManifest = value; }
	}

#if UNITY_EDITOR
	// 
	public static bool SimulateAssetBundleInEditor 
	{
		get
		{
			if (m_SimulateAssetBundleInEditor == -1)
				m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;
			
			return m_SimulateAssetBundleInEditor != 0;
		}
		set
		{
			int newValue = value ? 1 : 0;
			if (newValue != m_SimulateAssetBundleInEditor)
			{
				m_SimulateAssetBundleInEditor = newValue;
				EditorPrefs.SetBool(kSimulateAssetBundles, value);
			}
		}
	}
#endif

    /// <summary>
    /// 获取载入内存的名为assetBundleName的assetbundle
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="error"></param>
    /// <returns></returns>
	static public LoadedAssetBundle GetLoadedAssetBundle (string assetBundleName, out string error)
	{
		if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
			return null;
	
		LoadedAssetBundle bundle = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
		if (bundle == null)
			return null;
		
		string[] dependencies = null;
		if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
			return bundle;

		foreach(var dependency in dependencies)
		{
			if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
				return bundle;

			LoadedAssetBundle dependentBundle;
			m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
			if (dependentBundle == null)
				return null;
		}

		return bundle;
	}

    /// <summary>
    /// 加载AssetBundleManifest文件
    /// </summary>
    /// <param name="manifestAssetBundleName"></param>
    /// <returns></returns>
	static public AssetBundleLoadManifestOperation Initialize (string manifestAssetBundleName)
	{
		var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
		DontDestroyOnLoad(go);
	
#if UNITY_EDITOR	
		if (SimulateAssetBundleInEditor)
			return null;
#endif

		LoadAssetBundle(manifestAssetBundleName, true);
		var operation = new AssetBundleLoadManifestOperation (manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
		m_InProgressOperations.Add (operation);
		return operation;
	}
	
    /// <summary>
    /// 加载名为assetBundleName的assetbundle和它依赖的文件
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="isLoadingAssetBundleManifest"></param>
	static protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
	{
#if UNITY_EDITOR
		if (SimulateAssetBundleInEditor)
			return;
#endif

		if (!isLoadingAssetBundleManifest)
			assetBundleName = RemapVariantName (assetBundleName);

		bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

		if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
			LoadDependencies(assetBundleName);
	}
	
    /// <summary>
    /// 选择最优的assetbundle名字
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
	static protected string RemapVariantName(string assetBundleName)
	{
		string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

		// If the asset bundle doesn't have variant, simply return.
		if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0 )
			return assetBundleName;

		string[] split = assetBundleName.Split('.');

		int bestFit = int.MaxValue;
		int bestFitIndex = -1;
		// Loop all the assetBundles with variant to find the best fit variant assetBundle.
		for (int i = 0; i < bundlesWithVariant.Length; i++)
		{
			string[] curSplit = bundlesWithVariant[i].Split('.');
			if (curSplit[0] != split[0])
				continue;
			
			int found = System.Array.IndexOf(m_Variants, curSplit[1]);
			if (found != -1 && found < bestFit)
			{
				bestFit = found;
				bestFitIndex = i;
			}
		}

		if (bestFitIndex != -1)
			return bundlesWithVariant[bestFitIndex];
		else
			return assetBundleName;
	}

    /// <summary>
    /// 使用www加载assertBundle
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="isLoadingAssetBundleManifest"></param>
    /// <returns></returns>
	static protected bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest)
	{
		LoadedAssetBundle bundle = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
		if (bundle != null)
		{
			bundle.m_ReferencedCount++;
			return true;
		}

		if (m_DownloadingWWWs.ContainsKey(assetBundleName) )
			return true;

		WWW download = null;
		string url = m_BaseDownloadingURL + assetBundleName;

		if (isLoadingAssetBundleManifest)
			download = new WWW(url);
		else
			download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0); 

		m_DownloadingWWWs.Add(assetBundleName, download);

		return false;
	}

    /// <summary>
    /// 加载assetBundle依赖的资源
    /// </summary>
    /// <param name="assetBundleName"></param>
	static protected void LoadDependencies(string assetBundleName)
	{
		if (m_AssetBundleManifest == null)
		{
			Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
			return;
		}

        // 从AssetBundleManifest读取依赖文件
		string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
		if (dependencies.Length == 0)
			return;
			
		for (int i=0;i<dependencies.Length;i++)
			dependencies[i] = RemapVariantName (dependencies[i]);
			
		// 缓存并加载所有依赖文件
		m_Dependencies.Add(assetBundleName, dependencies);
		for (int i=0;i<dependencies.Length;i++)
			LoadAssetBundleInternal(dependencies[i], false);
	}

    /// <summary>
    /// 卸载assetbundle和其依赖的资源
    /// </summary>
    /// <param name="assetBundleName"></param>
	static public void UnloadAssetBundle(string assetBundleName)
	{
#if UNITY_EDITOR
		if (SimulateAssetBundleInEditor)
			return;
#endif

		//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);

		UnloadAssetBundleInternal(assetBundleName);
		UnloadDependencies(assetBundleName);

		//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
	}

    /// <summary>
    /// 卸载assetbundle依赖的资源
    /// </summary>
    /// <param name="assetBundleName"></param>
	static protected void UnloadDependencies(string assetBundleName)
	{
		string[] dependencies = null;
		if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
			return;

		// Loop dependencies.
		foreach(var dependency in dependencies)
		{
			UnloadAssetBundleInternal(dependency);
		}

		m_Dependencies.Remove(assetBundleName);
	}

	/// <summary>
	/// 释放assetbundle
	/// </summary>
	/// <param name="assetBundleName"></param>
    static protected void UnloadAssetBundleInternal(string assetBundleName)
	{
		string error;
		LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
		if (bundle == null)
			return;

		if (--bundle.m_ReferencedCount == 0)
		{
			bundle.m_AssetBundle.Unload(false);
			m_LoadedAssetBundles.Remove(assetBundleName);
			//Debug.Log("AssetBundle " + assetBundleName + " has been unloaded successfully");
		}
	}

    /// <summary>
    /// 检测加载队列
    /// </summary>
	void Update()
	{
		var keysToRemove = new List<string>();
		foreach (var keyValue in m_DownloadingWWWs)
		{
			WWW download = keyValue.Value;

			if (download.error != null)
			{
				m_DownloadingErrors.Add(keyValue.Key, download.error);
				keysToRemove.Add(keyValue.Key);
				continue;
			}

			if(download.isDone)
			{
				//Debug.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);
				m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle) );
				keysToRemove.Add(keyValue.Key);
			}
		}

		// 移除已经完成的www加载
		foreach( var key in keysToRemove)
		{
			WWW download = m_DownloadingWWWs[key];
			m_DownloadingWWWs.Remove(key);
			download.Dispose();
		}

		// 更新当前正在加载的assetbundle
		for (int i=0;i<m_InProgressOperations.Count;)
		{
			if (!m_InProgressOperations[i].Update())
			{
				m_InProgressOperations.RemoveAt(i);
			}
			else
				i++;
		}
	}


    /// <summary>
    /// 从assetBundle中加载asset
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    static public AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName)
    {
        AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
        if (SimulateAssetBundleInEditor)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("There is no asset in " + assetBundleName);
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
            operation = new AssetBundleLoadAssetOperationSimulation(target);
        }
        else
#endif
        {
            LoadAssetBundle(assetBundleName);
            operation = new AssetBundleLoadAssetOperationBundle(assetBundleName);

            m_InProgressOperations.Add(operation);
        }

        return operation;
    }

    /// <summary>
    /// 从assetBundle中加载名为assetName的asset
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
	static public AssetBundleLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
	{
		AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
		if (SimulateAssetBundleInEditor)
		{
			string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
			if (assetPaths.Length == 0)
			{
				Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
				return null;
			}

			// @TODO: Now we only get the main object from the first asset. Should consider type also.
			Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
			operation = new AssetBundleLoadAssetOperationSimulation (target);
		}
		else
#endif
		{
			LoadAssetBundle (assetBundleName);
			operation = new AssetBundleLoadAssetOperationFull (assetBundleName, assetName, type);

			m_InProgressOperations.Add (operation);
		}

		return operation;
	}

    /// <summary>
    /// 从assetBundle中加载名为levelName的关卡
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="levelName"></param>
    /// <param name="isAdditive"></param>
    /// <returns></returns>
	static public AssetBundleLoadOperation LoadLevelAsync (string assetBundleName, string levelName, bool isAdditive)
	{
		AssetBundleLoadOperation operation = null;
#if UNITY_EDITOR
		if (SimulateAssetBundleInEditor)
		{
			string[] levelPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
			if (levelPaths.Length == 0)
			{
				///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
				//        from that there right scene does not exist in the asset bundle...
			
				Debug.LogError("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
				return null;
			}

			if (isAdditive)
				EditorApplication.LoadLevelAdditiveInPlayMode(levelPaths[0]);
			else
				EditorApplication.LoadLevelInPlayMode(levelPaths[0]);

			operation = new AssetBundleLoadLevelSimulationOperation();
		}
		else
#endif
		{
			LoadAssetBundle (assetBundleName);
			operation = new AssetBundleLoadLevelOperation (assetBundleName, levelName, isAdditive);

			m_InProgressOperations.Add (operation);
		}

		return operation;
	}

    /// <summary>
    /// 从已加载列表中查找名为assetName的bundle
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    static public AssetBundleRequest LoadObjectAynsc(string assetName)
    {
        Dictionary<string, LoadedAssetBundle>.Enumerator assetDic = m_LoadedAssetBundles.GetEnumerator();
        while(assetDic.MoveNext())
        {
            LoadedAssetBundle ab = assetDic.Current.Value;
            if (ab.m_AssetBundle.Contains(assetName))
            {
                AssetBundleRequest ar = ab.m_AssetBundle.LoadAssetAsync(assetName);
                return ar;
            }
        }
        return null;
    }

    /// <summary>
    /// 查找包含assetName的bundle
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    static public AssetBundleRequest LoadObjectAynsc(string bundleName, string assetName)
    {
        if (m_LoadedAssetBundles.ContainsKey(bundleName))
        {
            LoadedAssetBundle ab = m_LoadedAssetBundles[bundleName];
            if (ab.m_AssetBundle.Contains(assetName))
            {
                AssetBundleRequest ar = ab.m_AssetBundle.LoadAssetAsync(assetName);
                return ar;
            }
        }
        return null;
    }


} // End of AssetBundleManager.