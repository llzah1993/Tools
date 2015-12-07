using UnityEngine;
using System.Collections;

public class LTAssetsManagerDemo : MonoBehaviour {

    public string resPathName = "UIPrefab/head";

    public string bundleName = "uires.unity3d";

    protected GameObject go;

    LTAssetsManager lt;
	// Use this for initialization
    IEnumerator Start()
    {
        while(LTAssetsManager.Instance == null)
        {
            yield return new WaitForFixedUpdate();
        }

       lt = LTAssetsManager.Instance;

        //Load Bundles
        yield return StartCoroutine(lt.LoadAssetsBundleAynsc(bundleName));

        //Load Resources
        yield return StartCoroutine(lt.LoadResourceAsync(resPathName));

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void LoadRes()
    {
        GameObject obj = lt.GetResource<GameObject>(resPathName);
        go = GameObject.Instantiate(obj);
        go.transform.SetParent(GameObject.Find("UICamera").transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.SetAsFirstSibling();
    }


    public void UnloadRes()
    {
        GameObject.Destroy(go);
        //lt.UnLoadBundleAssets(bundleName); //UnLoad Bundles
        lt.ResourcesCollect();
    }

}
