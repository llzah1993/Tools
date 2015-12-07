using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LTAssetsManager :BaseLoader 
{
    protected Dictionary<string, ResAnsyRequest> AssetsObjDic = new Dictionary<string, ResAnsyRequest>();

    protected static LTAssetsManager instance;

    public static LTAssetsManager Instance
    {
        get 
        { 
            return instance;
        }
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(Initialize());

        instance = this;

//         string path = "head";
// 
//         yield return StartCoroutine(LoadAssetsBundleAynsc("uires.unity3d"));
// 
//         yield return StartCoroutine(LoadResourceAsync(path));
// 
//         GameObject obj  = GetResource<GameObject>(path);
//         GameObject head = GameObject.Instantiate(obj);
//         head.transform.SetParent(GameObject.Find("UICamera").transform);
//         head.transform.localPosition = Vector3.zero;
//         head.transform.localScale = Vector3.one;
//         head.transform.SetAsFirstSibling();

    }

//     public void Test()
//     {
//         GameObject h = GameObject.Find("head(Clone)");
//         GameObject.Destroy(h);
// 
//         UnLoadBundleAssets("uires.unity3d");
// 
//         ResourcesCollect();
// 
//     }


    public AssetBundleLoadAssetOperation LoadAssetsBundleAynsc(string bundleName)
    {
        return AssetBundleManager.LoadAssetAsync(bundleName);
    }

    public  ResAnsyRequest LoadResourceAsync(string resPathName)
    {
        string assetName = Path.GetFileName(resPathName);
        ResAnsyRequest ar = null;
        if(AssetsObjDic.ContainsKey(resPathName))
        {
            ar = AssetsObjDic[resPathName];
            return ar;
        }

        AsyncOperation rr = AssetBundleManager.LoadObjectAynsc(assetName);
        if(rr == null)
        {
            rr = UnityEngine.Resources.LoadAsync(resPathName);
        }

        if(rr != null)
        {
           ar = new ResAnsyRequest(rr);
           AssetsObjDic.Add(resPathName, ar);
        }

        return ar;
    }

    public ResAnsyRequest LoadResourceAsync(string resPathName, string findBundleName)
    {
        string assetName = Path.GetFileName(resPathName);

        ResAnsyRequest ar = null;
        if (AssetsObjDic.ContainsKey(resPathName))
        {
            ar = AssetsObjDic[resPathName];
            return ar;
        }

        AsyncOperation rr = AssetBundleManager.LoadObjectAynsc(findBundleName, assetName);
        if (rr == null)
        {
            rr = UnityEngine.Resources.LoadAsync(resPathName);
        }

        if (rr != null)
        {
            ar = new ResAnsyRequest(rr);
            AssetsObjDic.Add(resPathName, ar);
        }

        return ar;
    }

    public T GetResource<T>(string resPathName) where T: Object
    {

        ResAnsyRequest rr = AssetsObjDic[resPathName];
        T resObj = rr.GetAsset<T>();

        return resObj;
    }

    public void UnLoadResourceAssets(string path,bool unloadAsset = false)
    {
        if(AssetsObjDic.ContainsKey(path))
        {
            ResAnsyRequest rr = AssetsObjDic[path];
            if(rr.rType == ResAnsyRequest.ResAnsyType.ResAnsyType_Resource)
            {
                Object obj = rr.GetAsset<Object>();
                if(unloadAsset)
                {
                    Resources.UnloadAsset(obj);
                }
            }
            AssetsObjDic.Remove(path);       
        }
 
    }
    public void UnLoadBundleAssets(string assetsPath)
    {
        AssetBundleManager.UnloadAssetBundle(assetsPath);
    }

    public void ResourcesCollect()
    {
        Resources.UnloadUnusedAssets();
    }

}

public class ResAnsyRequest : IEnumerator
{
    public enum ResAnsyType
    {
        ResAnsyType_Resource,
        ResAnsyType_Bundles
    }

    public AsyncOperation asynOperation;

    public ResAnsyType rType;
    public ResAnsyRequest(AsyncOperation ao)
    {
        asynOperation = ao;
        if(asynOperation.GetType() == typeof(ResourceRequest))
        {
            rType = ResAnsyType.ResAnsyType_Resource;
        }
        else
        {
            rType = ResAnsyType.ResAnsyType_Bundles;
        }
    }

    public T GetAsset<T>() where T : UnityEngine.Object
    {
        if (asynOperation.isDone)
        {
            if (asynOperation.GetType() == typeof(AssetBundleRequest))
            {
                AssetBundleRequest ar = (AssetBundleRequest)asynOperation;
                return (ar.asset as T);
            }

            if (asynOperation.GetType() == typeof(ResourceRequest))
            {
                ResourceRequest rr = (ResourceRequest)asynOperation;
                return (rr.asset as T);
            }

        }

        return null;
    }

    public bool MoveNext()
    {
        return !isDone();
    }

    public void Reset()
    {

    }

    public bool isDone()
    {
        return asynOperation.isDone;
    }

    public object Current
    {
        get
        {
            return null;
        }
    }
}