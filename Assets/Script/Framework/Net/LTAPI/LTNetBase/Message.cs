namespace LTNet
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.IO;

	public interface IMessage
	{
		byte GetProtocol();
		IMessageData GetMessageData();

		byte[] Serialize();
		Message Deserialize(byte[] buffer, int index, int count);
	}

	public class Message : IMessage
	{
		private IMessageData mData = null;

		public Message()
		{
		}

		public Message(IMessageData data)
		{
			mData = data;
		}

		public byte GetProtocol()
		{
			return mData != null ? mData.Protocol : (byte)0;
		}

		public IMessageData GetMessageData()
		{
			return mData;
		}

		public byte[] Serialize()
		{
			DataStream bufferWriter = new DataStream(true);

			if (mData != null)
				mData.Serialize(bufferWriter);

			return bufferWriter.ToByteArray();
		}

		public Message Deserialize(byte[] buffer, int index, int count)
		{
			DataStream bufferReader = new DataStream(buffer, index, count, true);

			byte protocol = bufferReader.ReadByte();
			uint messageId = 0;
			if (protocol != 0xFF)
			{
				if (protocol == 0x1)
				{
					bufferReader.Seek(20, SeekOrigin.Current);
					messageId = bufferReader.ReadInt32();
				}

				if (protocol == 0x2)//p2p message
				{
					bufferReader.Seek(37, SeekOrigin.Current);
					messageId = bufferReader.ReadInt32();
				}
			}

			mData = ProtoManager.Instance.TryDeserialize(protocol, messageId, buffer, index, count);

			return this;
		}
	}
}