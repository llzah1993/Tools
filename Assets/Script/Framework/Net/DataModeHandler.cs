using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LTNet;


public abstract class DataModeHandler : MonoBehaviour
{

    //public ushort dataModCode;

    public abstract void handler(DataStream reader, Dictionary<ushort, DataModeHandler> ModeCode_DataDic , ushort dataModeCode);

    public abstract ushort DataModeCode
    {
        get;
    }

}

