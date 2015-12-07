using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTNet;
using UnityEngine;

public delegate void ModuleHandle(DataStream reader);

class LiteResponse : Response
{

    public static Dictionary<uint, ModuleHandle> ProtocalID_DataBufferDic = new Dictionary<uint, ModuleHandle>();

    public override void Deserialize(DataStream reader)
    {

        base.Deserialize(reader);

        uint protocalID = reader.ReadInt32();

        ProtocalID_DataBufferDic[protocalID](reader);
    }

    private void HandlerModule(DataStream reader)
    {
        ushort dataModCode = reader.ReadInt16();
        ProtocalID_DataBufferDic[dataModCode](reader);
    }

}
