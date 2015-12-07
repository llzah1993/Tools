using LTNet;

public class PositionRequest : RedAlertRequestBase
{
    ulong mUserId;
    int mPosX;
    int mPosY;
    ulong mTime;
    int mVelocityX;
    int mVelocityY;
    ulong mMoveStartTime;
	int mSeqNum;
	
	public PositionRequest(ulong id, int posX, int posY, ulong time, int vx, int vy, ulong moveStartTime, int seq)
	{
        mUserId = id;
        mPosX = posX;
        mPosY = posY;
        mTime = time;
        mVelocityX = vx;
        mVelocityY = vy;
        mMoveStartTime = moveStartTime;
		mSeqNum = seq;
	}

	protected override void SetMessageId()
	{
		mMessageId = NetProtocols.MOVE_REQ;
	}

    public override void Serialize(DataStream writer)
    {
		base.Serialize(writer);
        writer.WriteInt64(mUserId);
        writer.WriteInt32((uint)mPosX);
        writer.WriteInt32((uint)mPosY);
        writer.WriteInt64(mTime);
        writer.WriteInt32((uint)mVelocityX);
        writer.WriteInt32((uint)mVelocityY);
        writer.WriteInt64(mMoveStartTime);
		writer.WriteInt32 ((uint)mSeqNum);
    }

}