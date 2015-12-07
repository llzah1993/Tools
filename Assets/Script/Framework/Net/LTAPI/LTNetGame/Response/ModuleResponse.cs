using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTNet;
using UnityEngine;

//public delegate void ModuleHandle(DataStream reader);

class ModuleResponse : Response
{
    public static readonly ushort HANDLE_MODU_TESTS = 998;


    public static Dictionary<ushort, ModuleHandle> ModCode_ModBufferDic = new Dictionary<ushort, ModuleHandle>();

    public override void Deserialize(DataStream reader)
    {

        base.Deserialize(reader);

        uint mesgCode = reader.ReadInt32();

        byte dataModNum = reader.ReadByte();

        while (dataModNum > 0)
        {
            //Tryparse
            HandlerModule(reader);

            dataModNum--;
        }
    }

    private void HandlerModule(DataStream reader)
    {
        ushort dataModCode = reader.ReadInt16();
        ModCode_ModBufferDic[dataModCode](reader);
    }

}
