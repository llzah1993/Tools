namespace LTNet
{
	using UnityEngine;
	using System;
	using System.Collections;

	public abstract class Request : SendOutBehaviour, IMessageData
	{
		public byte mProtocol = 0;// (1 byte)
		public uint mDestServerInstance = 0;	 // (4 byte)
		public uint mSrcServerInstance = 0; // (4 byte)
		public UInt64 mSeq = 0; // (8 byte)
		public uint mFlag = 0; // (4 byte)
		public uint mMessageId = 0; // (4 byte)
		public UInt64 mReserved_1 = 0; // (64 byte)
		public uint mReserved_2 = 0; // (4 byte)
		public uint mReserved_3 = 0; // (4 byte)

		public byte Protocol
		{
			get
			{
				return mProtocol;
			}
		}

	    protected override Message CreateMessage()
	    {
			SetProtocol();
			SetServerInstance();
			SetMessageId();

	        return new Message(this);
	    }

	    public virtual void Serialize(DataStream writer)
	    {
			writer.WriteByte(mProtocol);
			writer.WriteInt32(mDestServerInstance);
			writer.WriteInt32(mSrcServerInstance);
			writer.WriteInt64 (mSeq);
			writer.WriteInt32(mFlag);
			writer.WriteInt32(mMessageId);
			writer.WriteInt64(mReserved_1);
			writer.WriteInt32(mReserved_2);
			writer.WriteInt32(mReserved_3);
	    }

	    public virtual void Deserialize(DataStream reader)
	    {
			//no need to implement as this is a request
	    }

	    public virtual void Send(SocketConnector connector)
	    {
	        connector.SendNetMessage(this.CreateMessage());
	    }
	}
}