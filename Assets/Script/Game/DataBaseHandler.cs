using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;

public abstract class DataBaseHandler : MonoBehaviour
{

    protected static string savepath = Application.dataPath + "/CLTUI/Example/Resources/JsonData/";

    public virtual object getSerializeObj()
    {
        return this;
    }

    public virtual void SerializeToJson()
    {
        object sobj = getSerializeObj();
        string jsontext = JsonMapper.ToJson(sobj);
        string path = savepath + sobj.GetType().ToString() + ".json";
        File.WriteAllText(path, jsontext);
    }


    public abstract void DeSerilizeInJson(string json);

}

public abstract class DataBase
{
    public virtual string SerializeToJson()
    {
        string jsontext = JsonMapper.ToJson(this);
        return jsontext;
    }

    //public abstract void DeSerilizeInJson(string json);
}
