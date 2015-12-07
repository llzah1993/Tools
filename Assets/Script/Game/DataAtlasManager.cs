using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class DataAtlasManager : MonoBehaviour
{

    public const string jsonFilePath = "Game/Data/DataAtlas";
    public const string jsonDir = "Game/Data/";
    public static Dictionary<string, Dictionary<string, DataBase>> dataDic = new Dictionary<string, Dictionary<string, DataBase>>();


    protected static Dictionary<string, Dictionary<string, List<DataBase>>> prefixDataDic = new Dictionary<string, Dictionary<string, List<DataBase>>>();


    private static DataAtlasManager instance = null;
    public static DataAtlasManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        Initialize();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    protected void Initialize()
    {
        dataDic.Clear();
        //TextAsset jsonListAsset = Resources.Load<TextAsset>(jsonFilePath);
        //string[] files = jsonListAsset.text.Split("\r\n".ToCharArray());
        //for (int i = 0; i < files.Length; i++)
        //{
        //    string jsonfile = files[i];
        //    jsonfile = jsonfile.Trim();
        //    if (jsonfile != string.Empty)
        //    {
        //        TextAsset jsonText = Resources.Load<TextAsset>(jsonDir + jsonfile);
        //        DataAtlas dataAtlas = LitJson.JsonMapper.ToObject<DataAtlas>(jsonText.text);
        //        dataAtlas.initialize(dataDic);
        //    }
        //}

        TextAsset jsonListAsset = Resources.Load<TextAsset>(jsonFilePath);
        DataAtlas dataAtlas = LitJson.JsonMapper.ToObject<DataAtlas>(jsonListAsset.text);
        dataAtlas.initialize(dataDic);
    }

    public Dictionary<string, T> getDataWithType<T>() where T : DataBase
    {
        string typename = typeof(T).Name;
        Dictionary<string, DataBase> dataBaseDic;
        if (dataDic.TryGetValue(typename, out dataBaseDic))
        {
            return dataBaseDic as Dictionary<string, T>;
        }
        return null;
    }

    public T getDataWithTypeById<T>(string id) where T : DataBase
    {
        string typename = typeof(T).Name;
        Dictionary<string, DataBase> dataBaseDic;
        DataBase database;
        if (dataDic.TryGetValue(typename, out dataBaseDic))
        {
            if (dataBaseDic.TryGetValue(id, out database))
            {
                return database as T;
            }
        }
        return null;
    }

    public List<T> getDataWithTypeWithPrefix<T>(string prefix) where T : DataBase
    {
        List<DataBase> datalist = null;
        string typename = typeof(T).Name;
        if (dataDic.ContainsKey(typename))
        {
            if (prefixDataDic.ContainsKey(typename))
            {
                Dictionary<string, List<DataBase>> prefixDataListDic = prefixDataDic[typename];
                if (prefixDataListDic.ContainsKey(prefix))
                {
                    datalist = prefixDataListDic[prefix];
                }
                else
                {
                    datalist = new List<DataBase>();
                    Dictionary<string, DataBase> dataBaseDic;
                    if (dataDic.TryGetValue(typename, out dataBaseDic))
                    {
                        foreach (string keystr in dataBaseDic.Keys)
                        {
                            if (keystr.StartsWith(prefix))
                            {
                                DataBase db = dataBaseDic[keystr];
                                datalist.Add(db);
                            }
                        }
                    }
                    prefixDataListDic.Add(prefix, datalist);
                }
            }
            else
            {
                Dictionary<string, List<DataBase>> prefixDataListDic = new Dictionary<string, List<DataBase>>();
                datalist = new List<DataBase>();
                Dictionary<string, DataBase> dataBaseDic;
                if (dataDic.TryGetValue(typename, out dataBaseDic))
                {
                    foreach (string keystr in dataBaseDic.Keys)
                    {
                        if (keystr.StartsWith(prefix))
                        {
                            DataBase db = dataBaseDic[keystr];
                            datalist.Add(db);
                        }
                    }
                }
                prefixDataListDic.Add(prefix, datalist);
                prefixDataDic.Add(typename, prefixDataListDic);
            }

            return datalist as List<T>;
        }
        return null;
    }


    public void ClearPrefixCache()
    {
        prefixDataDic.Clear();
    }
}

