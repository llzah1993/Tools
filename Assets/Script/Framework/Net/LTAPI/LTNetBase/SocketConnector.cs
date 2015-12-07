//by Kevin, Jan 20th 2015
namespace LTNet
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
    using Framework;

	public sealed class ConnectCallback
	{
		public Action<SocketConnector, object> mCallback;
		public object mUserdata;

		public ConnectCallback(Action<SocketConnector, object> callback, object userdata)
		{
			mCallback = callback;
			mUserdata = userdata;
		}

		public void Invoke(SocketConnector connector)
		{
			mCallback(connector, mUserdata);
		}
	}
	 
	public class DataHolder
	{
		private byte[] mRecvDataCache;//use array as buffer for efficiency consideration
		private int mTail = -1;

		public void PushData(byte[] data, int length)
		{
			if (mRecvDataCache == null)
				mRecvDataCache = new byte[length];

			if (this.Count + length >  this.Capacity)//current capacity is not enough, enlarge the cache
			{
				byte[] newArr = new byte[this.Count + length];
				mRecvDataCache.CopyTo(newArr, 0);
				mRecvDataCache = newArr;
			}

			Array.Copy(data, 0, mRecvDataCache, mTail + 1, length);
			mTail += length;
		}

		public Message PopMessage()
		{
			if (this.Count == 0)
			{
				//skip if no data is currently in the cache
				return null;
			}

			if (this.Count >= 4)
			{
				DataStream reader = new DataStream(mRecvDataCache, true);
				int packLen = (int)reader.ReadInt32();
				if (packLen > 0)
				{
					if (this.Count - 4 >= packLen)
					{
						Message message = new Message();
						message.Deserialize(mRecvDataCache, 4, packLen);

						this.RemoveFromHead(packLen + 4);//remove deserialized data including the first 4 bytes indicating package length
						return message;
					}

                    ADebug.Log("[DataHolder Pop] package split, waiting for complete data");
					return null;
				}

                ADebug.Log("[DataHolder Pop] wrong package length");
                return null;
			}

            ADebug.Log("[DataHolder Pop] cannot read package length");
            return null;
		}

		public void Reset()
		{
			mTail = -1;
		}

		private void RemoveFromHead(int countToRemove)
		{
			if (countToRemove > 0 && this.Count - countToRemove > 0)
			{
                Array.Copy(mRecvDataCache, countToRemove, mRecvDataCache, 0, this.Count - countToRemove);
			}
			mTail -= countToRemove;
		}

		//cache capacity
		public int Capacity
		{
			get
			{
				return mRecvDataCache != null ? mRecvDataCache.Length : 0;
			}
		}

        //indicate how much data is currently in cache in bytes
		public int Count
		{
			get
			{
				return mTail + 1;
			}
		}
	}

	public class SocketConnector : MonoBehaviour, ISocketConnector
	{
		public enum LoopingMode
		{
			FIXED,
			TIME_INTERVAL
		}

		private NetSocket mSocket;
		private bool mUsingAsync = false;
		private bool mStopReceiving = false;
		private LoopingMode mLoopingMode = LoopingMode.FIXED;

		private List<ConnectCallback> mConnCallbackList;
		private DataHolder mDataHolder = new DataHolder();

		private float mLoopingInterval = 0.1f;
		//private readonly object mLock = new object();

		//methods
		public void BindSocket(NetSocket socket)
		{
			this.Disconnect();
			mSocket = socket;
		}

		public void Connect()
		{
            ADebug.Log("[SocketConnector connect] with " + (mUsingAsync ? "Async mode" : "Sync mode"));

			try
			{
				if (mUsingAsync)
					mSocket.ConnectAsync();
				else
					mSocket.Connect();
			}
			catch (Exception e)
			{
                ADebug.LogError("[SocketConnector connect] exception: {0}" , e.Message);
				return;
			}

			StartCoroutine("PollingConnResult");
			StartCoroutine("StartReceiving");
		}

		public void Disconnect()
		{
			//DebugUtil.Log("[SocketConnector Disconnect]");
			StopAllCoroutines();

			if (mSocket != null) mSocket.Disconnect();
		}

		public bool IsConnected()
		{
			return mSocket.Connected;
		}

		public bool SendNetMessage(Message message)
		{
			byte[] data = message.Serialize();
			byte[] buffer = new byte[data.Length + 4];

			DataStream writer = new DataStream(buffer, true);
			writer.WriteInt32((uint)data.Length);//write whole package length
			writer.WriteRaw(data);

			return mSocket.Send(writer.ToByteArray());
		}

		public void RegisterConnectCallback(ConnectCallback callback)
		{
			if (mConnCallbackList == null)
				mConnCallbackList = new List<ConnectCallback>();
			mConnCallbackList.Add(callback);
		}

		public void RegisterConnectCallback(Action<SocketConnector, object> callback, object userdata)
		{
			this.RegisterConnectCallback(new ConnectCallback(callback, userdata));
		}

		public void SetLoopingMode(LoopingMode mode)
		{
			mLoopingMode = mode;
		}

		public void SetLoopingInterval(float dt)
		{
			mLoopingInterval = dt;
		}

		public void EnableReceiving(bool isReceiving)
		{
			mStopReceiving = !isReceiving;
		}

		private IEnumerator PollingConnResult()
		{
			while (true)
			{
				//DebugUtil.Log ("[SocketConnector PollingConnResult]");

				if (mSocket.ConnectDone)
				{
					if (mConnCallbackList != null)
					{
						foreach(ConnectCallback cb in mConnCallbackList)
						{
							cb.Invoke(this);
						}
					}
					yield break;
				}
				else
				{
					if (mLoopingMode == LoopingMode.FIXED)
						yield return new WaitForFixedUpdate();
					else if (mLoopingMode == LoopingMode.TIME_INTERVAL)
						yield return new WaitForSeconds(mLoopingInterval);
				}
			}
		}

		private IEnumerator StartReceiving()
		{
			mDataHolder.Reset();

			while(!mStopReceiving)
			{
				byte[] buffer = null;
				int length = 0;
				if (mSocket != null && mSocket.Connected && mSocket.DataAvailable)
				{
					buffer = new byte[2048];
					length = mSocket.Receive(buffer);
				}

				if (length > 0 && buffer != null)
				{
					mDataHolder.PushData(buffer, length);
				}

				//dispatch a response to all subscribers
				Message message = mDataHolder.PopMessage(); //try to pop a message, could be null
				if (message != null)
				{
					MessageDispatcher.Instance.Dispatch(message);
				}

				if (mLoopingMode == LoopingMode.FIXED)
					yield return new WaitForFixedUpdate();
				else if (mLoopingMode == LoopingMode.TIME_INTERVAL)
					yield return new WaitForSeconds(mLoopingInterval);
			}
		}

		//properties
		public NetSocket Socket
		{
			get
			{
				return mSocket;
			}
		}

		public bool UsingAsync
		{
			get
			{
				return mUsingAsync;
			}
			set
			{
				mUsingAsync = value;
			}
		}

	}//SocketConnector
}
