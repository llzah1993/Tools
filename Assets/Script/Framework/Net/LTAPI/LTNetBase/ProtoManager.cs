namespace LTNet
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class ProtoManager
	{
		private static ProtoManager mInstance;
		private Dictionary<int, Func<DataStream, IMessageData>> mProtocolMapping;

		private ProtoManager()
		{
			mProtocolMapping = new Dictionary<int, Func<DataStream, IMessageData>>();
		}

		public void AddProtocol<T>(byte protocol, uint messageId) where T: IMessageData, new()
		{
			int key = GenKey (protocol, messageId);
			if (mProtocolMapping.ContainsKey(key))
			{
				mProtocolMapping.Remove(key);
			}

			mProtocolMapping.Add(key, 
			                     (stream) => {
				T data = new T();
				data.Deserialize(stream);
				return data;
			});
		}

		public IMessageData TryDeserialize(byte protocol, uint messageId, byte[] buffer, int index, int count)
		{
			DataStream stream = new DataStream(buffer, index, count, true);
			int key = GenKey (protocol, messageId);

			IMessageData ret = null;
			if (mProtocolMapping.ContainsKey(key))
			{
				ret = mProtocolMapping[key](stream);
			}

			return ret;
		}

		private int GenKey(int protocol, uint messageId)
		{
			return (protocol << 24 | (int)messageId);
		}

		public static ProtoManager Instance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = new ProtoManager();
				}
				return mInstance;
			}
		}
	}
}
