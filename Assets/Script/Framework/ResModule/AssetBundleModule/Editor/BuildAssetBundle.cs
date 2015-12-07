using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using LTUnityPlugin;
using LitJson;

public class BuildAssetBundle : EditorWindow
{
	//
	private string help = "Help: 打包AssetBundle工具,用于更新资源 " + 
		"1. 可选择Android/iOS/PC平台 " +
			"2. 选择对应平台下的打包格式";
	//
	private static BuildAssetBundle instance;
	
	private string sourceDir = string.Empty;
	
	private string bundlePath = string.Empty;
	
	private string vertionFile = "resourcesinfo.txt";
	
	public string vertionText = string.Empty;
	
	
	public Dictionary<string, bool>  platformsForbundle =
		new Dictionary<string, bool>();
	
	public Dictionary<string, BuildAssetBundleOptions>  platformsForCompress =
		new Dictionary<string, BuildAssetBundleOptions>();
	
	public bool[] platforms = new bool[3]{false, false, false};
	
	public BuildAssetBundleOptions[] platformsCompress = new BuildAssetBundleOptions[3]{
		BuildAssetBundleOptions.None, 
		BuildAssetBundleOptions.None, 
		BuildAssetBundleOptions.None};
	
	bool posGroupEnabled = true;
	
	public BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
	
	[MenuItem("Tools/BuildAssetBundle")]
	static void BuildAssetBundleUtil()
	{
		instance = (BuildAssetBundle)EditorWindow.GetWindow(typeof(BuildAssetBundle));
	}
	
	void OnGUI()
	{
		GUILayout.Space (10);
		EditorGUILayout.LabelField (new GUIContent(help));
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Open a Folder"))
		{
			sourceDir = EditorUtility.OpenFolderPanel("Open a source folder", "", "");
		}
		
		if (GUILayout.Button("Set Assetbundle name"))
		{
			if (sourceDir != string.Empty)
			{
				FindAllResources(sourceDir);
			}
		}
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space (10);
		posGroupEnabled = EditorGUILayout.BeginToggleGroup ("选择打包平台", posGroupEnabled);
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (25);
		platforms [0] = EditorGUILayout.BeginToggleGroup ("Android", platforms[0]);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (new GUIContent("选择打包格式"),GUILayout.Width(75f));
		platformsCompress [0] = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup (platformsCompress [0]);
		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (25);
		platforms [1] = EditorGUILayout.BeginToggleGroup ("iOS", platforms[1]);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (new GUIContent("选择打包格式"),GUILayout.Width(75f));
		platformsCompress [1] = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup (platformsCompress [1]);
		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (25);
		platforms [2] = EditorGUILayout.BeginToggleGroup ("StandaloneWindows", platforms[2]);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (new GUIContent("选择打包格式"),GUILayout.Width(75f));
		platformsCompress [2] = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup (platformsCompress [2]);
		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.EndToggleGroup ();
		
		GUILayout.Space (10);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("资源版本号:");
		vertionText = EditorGUILayout.TextField(vertionText);
		
		if (GUILayout.Button("Build"))
		{
			if(posGroupEnabled)
			{
				platformsForbundle.Clear ();
				platformsForCompress.Clear ();
				platformsForbundle.Add ("Android", platforms [0]);
				platformsForbundle.Add ("iOS", platforms [1]);
				platformsForbundle.Add ("StandaloneWindows",platforms [2]);
				platformsForCompress.Add("Android", platformsCompress[0]);
				platformsForCompress.Add("iOS", platformsCompress[1]);
				platformsForCompress.Add("StandaloneWindows", platformsCompress[2]);
				
				foreach(string value in platformsForbundle.Keys)
				{
					if(platformsForbundle[value])
					{
						BuildTarget platform = BuildTarget.StandaloneWindows;
						string str = value;
						switch(str)
						{
						case "Android":
							platform = BuildTarget.Android;
							break;
						case "iOS":
							platform = BuildTarget.iOS;
							break;
						case "Standalone":
							platform = BuildTarget.StandaloneWindows;
							break;
						default:
							break;
						}
						bundlePath = Application.dataPath + "/../" + platform.ToString();
						BuildBundle(platform, platformsForCompress[platform.ToString()]);
						GenerateVertionInfo();
						ReadAssetBundleInfo();
					}
				}
			}
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// 设置名称
	/// </summary>
	/// <param name="path"></param>
	/// <param name="isrecursive"></param>
	void FindAllResources(string path, bool isrecursive = true)
	{
		DirectoryInfo[] childDirectory;
		FileInfo[] newFileInfo;
		DirectoryInfo FatherDirectory = new DirectoryInfo(path);
		childDirectory = FatherDirectory.GetDirectories("*.*");
		newFileInfo = FatherDirectory.GetFiles();
		
		//修改目录
		foreach (DirectoryInfo dirfile in childDirectory)
		{
			if (dirfile.Extension != ".meta")
			{
				string str = dirfile.FullName.Substring(dirfile.FullName.LastIndexOf("Assets"));
				//string fileGuid = AssetDatabase.AssetPathToGUID(str);
				AssetImporter importer = AssetImporter.GetAtPath(str);
				if (importer != null)
				{
					importer.assetBundleName = str;
				}
			}
		}
		
		//修改文件
		foreach (FileInfo file in newFileInfo)
		{
			if (file.Extension != ".meta" && file.Extension != ".cs")
			{
				string strs = file.FullName.Substring(file.FullName.LastIndexOf("Assets"));
				string[] dependencesfiles = AssetDatabase.GetDependencies((new string[] { strs }));
				foreach (string str in dependencesfiles)
				{
					//string fileGuid = AssetDatabase.AssetPathToGUID(str);
					AssetImporter importer = AssetImporter.GetAtPath(str);
					if (importer != null)
					{
						importer.assetBundleName = str;
					}
				}
			}
		}
		
		foreach (DirectoryInfo dirInfo in childDirectory)
		{
			FindAllResources(dirInfo.FullName);
		}
	}
	
	/// <summary>
	/// 打包平台
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	string GetPlatform(BuildTarget target)
	{
		switch (target)
		{
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "iOS";
		case BuildTarget.StandaloneWindows:
			return "StandaloneWindows";
		case BuildTarget.StandaloneWindows64:
			return "StandaloneWindows64";
			//添加其它平台
		default:
			return null;
		}
	}
	
	void BuildBundle(BuildTarget platform, BuildAssetBundleOptions bundleformat)
	{
		//BuildTarget platform = EditorUserBuildSettings.activeBuildTarget;
		if (null != platform)
		{
			if(!Directory.Exists(bundlePath))
				Directory.CreateDirectory(bundlePath);
			
			BuildPipeline.BuildAssetBundles(bundlePath, bundleformat, platform);
		}
	}
	
	/// <summary>
	/// 生成版本信息
	/// </summary>
	void GenerateVertionInfo()
	{
		if (vertionFile != null)
		{
			Vertion vertionInfo = new Vertion();
			vertionInfo.vertionNum = vertionText;
			
			PartAssetBundleInfo(bundlePath, vertionInfo);
			
			string json = LitJson.JsonMapper.ToJson(vertionInfo);
			File.WriteAllText(bundlePath + "/" + vertionFile,json); 
		}
	}
	
	private void PartAssetBundleInfo(string path, Vertion vertion)
	{
		DirectoryInfo[] childDirectory;
		FileInfo[] newFileInfo;
		DirectoryInfo FatherDirectory = new DirectoryInfo(path);
		childDirectory = FatherDirectory.GetDirectories("*.*");
		newFileInfo = FatherDirectory.GetFiles();
		
		foreach (FileInfo file in newFileInfo)
		{
			if (file.Extension == ".manifest")
			{
				string hash = GetHashCode(file);
				vertion.abHashcodes.Add(file.Name, hash);
			}
		}
		
		foreach (DirectoryInfo dirInfo in childDirectory)
		{
			PartAssetBundleInfo(dirInfo.FullName, vertion);
		} 
	}
	
	private void ReadAssetBundleInfo()
	{
		string str = File.ReadAllText (bundlePath + "/" + vertionFile);
		Vertion v = LitJson.JsonMapper.ToObject<Vertion>(str);
	}
	
	/// <summary>
	/// 取出hashcode
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	private string GetHashCode(FileInfo file)
	{
		string[] text = File.ReadAllLines(file.FullName);
		foreach (string str in text)
		{
			if (str.Contains("Hash: "))
			{
				return str.Substring(str.LastIndexOf("Hash: "));
			}
		}
		return null;
	}
	
	class Vertion
	{
		public string vertionNum;
		public Dictionary<string, string> abHashcodes = new Dictionary<string, string>();
	}
}
