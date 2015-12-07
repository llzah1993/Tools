using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Text;
using System.IO;

[ExecuteInEditMode]
public class AssetBundleLoader : MonoBehaviour {

	public string loadfile = string.Empty;

	void OnGUI()
	{

#if UNITY_EDITOR
		if(GUILayout.Button("选择加载文件"))
		{
			GameObject go = new GameObject("new");
			if(null != Selection.activeObject)
			{
				if(null != PrefabUtility.GetPrefabType(Selection.activeObject))
				{
					loadfile = AssetDatabase.GetAssetPath(Selection.activeObject);
				}
			}
		}
#endif
		if(GUILayout.Button("LoadAssetbundle"))
		{

#if UNITY_EDITOR
			BuildTarget platform = EditorUserBuildSettings.activeBuildTarget;


			StringBuilder platformpath = new StringBuilder(string.Empty);
			platformpath.Append(Application.dataPath + "/../" + platform.ToString());

			string str = platformpath.ToString() + "/" + platform.ToString();
			if(File.Exists(str))
			{
				//manifestbundle
				AssetBundle manifestBundle = AssetBundle.CreateFromFile(str);
				
				if(manifestBundle != null)
				{
					//manifest文件
					AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
					
					//获取依赖文件
					string[] cubedepends = manifest.GetAllDependencies(loadfile);
					
					AssetBundle[] dependsAssetbundle = new AssetBundle[cubedepends.Length];
					
					for(int index = 0; index < cubedepends.Length; index++)
					{
						dependsAssetbundle[index] = AssetBundle.CreateFromFile(platformpath + "/" + cubedepends[index]);
					}
					
					AssetBundle cubeBundle = AssetBundle.CreateFromFile(platformpath + "/" + loadfile);
					GameObject cube = cubeBundle.LoadAsset("obj1") as GameObject;
					
					if(cube != null)
					{
						Instantiate(cube);
						
						manifestBundle.Unload(false);
						cubeBundle.Unload(false);
					}
				}
			}
#endif
		}

	}

}