using UnityEngine;
using System.Collections;
using LTNet;
using System.Collections.Generic;

public class UpdateRoomListResponse : Response 
{
     public uint mRoomNumber ; //（房间数量）
     public List<int> mRoomIdList = new List<int>();

    public UpdateRoomListResponse()
    {

    }

    public override void Deserialize(DataStream reader)
    {
		base.Deserialize(reader);

        mRoomNumber = reader.ReadInt32();
		for (int i = 0; i < mRoomNumber; i++)
        {
            int temp = (int)reader.ReadInt32();
            mRoomIdList.Add(temp);
        }
    }
}

public class EnterRoomResponse : Response
{
	public uint mUserNumber ; //（房间数量）
	public List<long> mUserIdList = new List<long>();
    public List<string> mUsernameList = new List<string>();

    public EnterRoomResponse()
    {
    }

    public override void Deserialize(DataStream reader)
    {
		base.Deserialize(reader);

		mUserNumber = reader.ReadInt32();
		for(int i = 0 ;i < mUserNumber ; ++i)
        {
            long id =(long) reader.ReadInt64();
            string name = reader.ReadString16();
          
			mUserIdList.Add(id);
            mUsernameList.Add(name);
         
        }
    }
}

public class LeaveRoomResponse : Response
{
    public LeaveRoomResponse()
    {
    }
}
