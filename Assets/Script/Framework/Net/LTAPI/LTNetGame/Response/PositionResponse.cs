using UnityEngine;
using System.Collections;
using LTNet;
using System.Collections.Generic;

public class PositionResponse : Response 
{
    public ulong mId;
	public int mPosX;
	public int mPosY;
	public ulong mTime;
    public int mVelocityX;
    public int mVelocityY;
    public ulong mMoveStartTime;
	public int mSeqNum;

    public PositionResponse()
    {
    }

    public override void Deserialize(DataStream reader)
    {
		base.Deserialize(reader);

        mId = reader.ReadInt64();
        mPosX = (int)reader.ReadInt32();
        mPosY = (int)reader.ReadInt32();
        mTime = reader.ReadInt64();
        mVelocityX = (int)reader.ReadInt32();
        mVelocityY = (int)reader.ReadInt32();
        mMoveStartTime = reader.ReadInt64();
        mSeqNum = (int)reader.ReadInt32();
    }
}