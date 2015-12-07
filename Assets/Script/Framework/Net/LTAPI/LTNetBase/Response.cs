namespace LTNet
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class Response : IMessageData
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
		public byte mResult;//(1 byte) only applicable in a response message

		public byte Protocol
		{
			get
			{
				return mProtocol;
			}
		}

	    public Response()
	    {
	    }

	    public virtual void Serialize(DataStream writer)
	    {
			//no need to implement as this is a response
	    }

	    public virtual void Deserialize(DataStream reader)
	    {
			mProtocol = reader.ReadByte();

			if (mProtocol != 0xFF)
			{
				mDestServerInstance = reader.ReadInt32();
				mSrcServerInstance = reader.ReadInt32();
				mSeq = reader.ReadInt64();
				mFlag = reader.ReadInt32();
				mMessageId = reader.ReadInt32();
				mReserved_1 = reader.ReadInt64();
				mReserved_2 = reader.ReadInt32();
				mReserved_3 = reader.ReadInt32();
				mResult = reader.ReadByte ();
			}
	    }
	}
}
