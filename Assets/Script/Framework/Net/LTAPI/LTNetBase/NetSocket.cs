//by Kevin, Jan 17th 2015
//Simple encapsulation of TcpClient
namespace LTNet
{
    using Framework;
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;

	class SocketConnectionException : ApplicationException  
	{   
		public SocketConnectionException(string message) : base(message) { }  
		public static readonly string ALREADY_CONNECTED = "Socket is already connected to remote";
		public static readonly string CONNECTING = "Socket is currently trying to connect to remote";

		public override string Message  
		{  
			get  
			{  
				return base.Message;  
			}  
		}  
	}

	public sealed class SocketAsyncState
	{
		public TcpClient mTcpClient;
		public object mUserdata;

		public SocketAsyncState(TcpClient client, object data)
		{
			mTcpClient = client;
			mUserdata = data;
		}
	}

	public class NetSocket : INetSocket
	{
		protected TcpClient mTcpClient;
		protected string mHost = "";//could be a url or an ip address
		protected int mPort = -1;//initialized to -1 meaning no port has been assigned yet
		protected NetSocketSetting mSetting;
		protected Action<object> mExternalAsyncCb;//callback when async connection is done
		protected bool mConnecting = false;
		protected bool mConnectDone = false;//indicate whether a connect/async connect call is done

		//protected readonly object mLock = new object();

		//constructors
		public NetSocket(string host, int port, NetSocketSetting setting = null)
		{
			Initialize(host, port, setting);
		}

		public NetSocket(IPEndPoint ipEndPoint, NetSocketSetting setting = null)
		{
			Initialize(ipEndPoint.Address.ToString(), ipEndPoint.Port, setting);
		}

		public NetSocket()
		{
			Initialize(null, -1, null);
		}

		//methods
		protected void Initialize(string host, int port, NetSocketSetting setting)
		{
			mTcpClient = new TcpClient();
			//send data immediately by default
			mTcpClient.NoDelay = true;
			this.Bind(host, port);
			mSetting = setting;
			if (mSetting != null) this.ApplySetting(mSetting);
		}

		public void Bind(string host, int port)
		{
			mHost = host;
			mPort = port;
		}

		public void ApplySetting(NetSocketSetting setting)
		{
			mTcpClient.NoDelay = setting.mIsNoDelay;
			mTcpClient.ReceiveTimeout = setting.mRecvTimeOut;
			mTcpClient.ReceiveBufferSize = setting.mRecvBufSize;
			mTcpClient.SendTimeout = setting.mSendTimeOut;
			mTcpClient.SendBufferSize = setting.mSendBufSize;
		}

		public virtual void Connect()
		{
			if (mTcpClient.Connected)
			{
				throw new SocketConnectionException(SocketConnectionException.ALREADY_CONNECTED);
			}

			if (mConnecting)
			{
				throw new SocketConnectionException(SocketConnectionException.CONNECTING);
			}

			try
			{
				mTcpClient.Connect(mHost, mPort);
				mConnectDone = true;
			}
			catch (Exception e)
			{
                ADebug.LogError("[NetSocket Connect] exception: {0}", e.Message);
			}
		}

		public virtual void Connect(string host, int port)
		{
			this.Bind(host, port);
			this.Connect();
		}

		//invoked when async connection is done
		protected void InternalAsyncDone(IAsyncResult asyncResult)
		{
			SocketAsyncState state = (SocketAsyncState)asyncResult.AsyncState;
			try
			{
				//make sure EndConnect is invoked
				if (state.mTcpClient != null && state.mTcpClient.Client != null)
				{
					state.mTcpClient.EndConnect(asyncResult);
                    ADebug.Log("[NetSocket InternalAsyncDone]");
                }
				else
				{
                    ADebug.Log("[NetSocket InternalAsyncDone] connection is probably closed");
                    return;
				}
			}
			catch (Exception e)
			{
                ADebug.Log("[NetSocket InternalAsyncDone] exception: {0}" , e.Message);
            }

			mConnecting = false;
			mConnectDone = true;
			if (mExternalAsyncCb != null)
			{
				mExternalAsyncCb(state.mUserdata);
			}
		}

		public virtual void ConnectAsync()
		{
			ConnectAsync(null, null);
		}

		public virtual void ConnectAsync(Action<object>callback, object userdata)
		{
			if (mTcpClient.Connected)
			{
				throw new SocketConnectionException(SocketConnectionException.ALREADY_CONNECTED);
			}
			
			if (mConnecting)
			{
				throw new SocketConnectionException(SocketConnectionException.CONNECTING);
			}

			mExternalAsyncCb = callback;
			try
			{
				IAsyncResult ar = mTcpClient.BeginConnect(mHost, mPort, InternalAsyncDone, new SocketAsyncState(mTcpClient, userdata));
				if (!ar.IsCompleted)
					mConnecting = true;
			}
			catch (Exception e)
			{
                ADebug.Log("[NetSocket ConnectAsync] exception: {0}", e.Message);
            }
		}

		public virtual void ConnectAsync(string host, int port, Action<object> callback, object userdata)
		{
			this.Bind(host, port);
			this.ConnectAsync(callback, userdata);
		}

		public virtual void Disconnect()
		{
			if (mTcpClient.Connected || mConnecting)
			{
                ADebug.Log("[NetSocket Disconnect]");

				mTcpClient.Close();
				mConnecting = false;

				//TcpClient instance will be disposed after being closed
				//make sure there is always an available TcpClient to use
				mTcpClient = new TcpClient();
			}

			mConnectDone = false;
		}

		public virtual bool Send(byte[] data)
		{
			try
			{
				if (!this.NetStream.CanWrite)
				{
                    ADebug.Log("[NetSocket Send] NetStream cannot write");
                    return false;
				}

				if(data == null || data.Length <= 0)
				{
                    ADebug.Log("[NetSocket Send] data not valid");
                    return false;
				}

				this.NetStream.Write(data, 0, data.Length);
				this.NetStream.Flush();

				return true;
			}
			catch (Exception e)
			{
                ADebug.Log("[NetSocket Send] exception: {0}" , e.Message);
                return false;
			}
		}

		public virtual int Receive(byte[] buffer)
		{
			try
			{
				int length = -1;
				if (this.NetStream.DataAvailable)
				{
					length = this.NetStream.Read(buffer, 0, buffer.Length);
					this.NetStream.Flush();
				}

				return length;
			}
			catch (Exception e)
			{
                ADebug.Log("[NetSocket Receive] exception: {0}", e.Message);
                return -1;
			}
		}

		//properties
		public string Host
		{
			get
			{
				return mHost;
			}
		}
		
		public int Port
		{
			get
			{
				return mPort;
			}
		}

		protected NetworkStream NetStream
		{
			get
			{
				return mTcpClient.GetStream();
			}
		}

		public bool Connected
		{
			get
			{
				return mTcpClient.Connected;
			}
		}

		//indicate whether this socket is still trying to establish a connection in async mode
		public bool Connecting
		{
			get
			{
				return mConnecting;
			}
		}

		public bool ConnectDone
		{
			get
			{
				return mConnectDone;
			}
		}

		public bool DataAvailable
		{
			get 
			{
				return mTcpClient.GetStream().DataAvailable;
			}
		}

	}//NetSocket
}
