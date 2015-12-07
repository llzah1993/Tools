using UnityEngine;
using System.Collections;
using LTNet;

public class EnterGameResponse :Response 
{
    public ulong mUserId;
    public string mUsername;

    public EnterGameResponse()
    {
    }

    public override  void Deserialize(DataStream reader)
    {
		base.Deserialize(reader);
       	mUserId =  reader.ReadInt64();
       	mUsername = reader.ReadString16();
    }
}
