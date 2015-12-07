using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LTNet;

public class StartLoadingResponse : Response
{
	public uint mUserNumber;
	public List<long> mUserList = new List<long>();
	public List<string> mUsernameList = new List<string>();
	public List<Vector3> mPostionList = new List<Vector3>();
	
	public StartLoadingResponse()
	{
	}
	
	public override void Deserialize(DataStream reader)
	{
		base.Deserialize(reader);

		mUserNumber = reader.ReadInt32();
		for(int i = 0 ; i < mUserNumber; ++i)
		{
			long id = (long)reader.ReadInt64();
			string name = reader.ReadString16();
			int posX = (int)reader.ReadInt32();
			int posY = (int)reader.ReadInt32();
			mUserList.Add(id);
			mUsernameList.Add(name);
			mPostionList.Add(new Vector3(posX * 0.001f, 0, posY * 0.001f));
		}
	}
	
}

public class EndLoadingResponse : Response
{
    public EndLoadingResponse()
    {
    }
}


