namespace LTNet
{
	using System;

	public class NetSocketSetting
	{
		public bool mIsNoDelay;
		
		public int mRecvBufSize;

		public int mRecvTimeOut;
		
		public int mSendBufSize;

		public int mSendTimeOut;
		
		public NetSocketSetting(int sendTimeOut, int recvTimeOut, bool noDelay, int recvBufSize, int sendBufSize)
		{
			this.mSendTimeOut = sendTimeOut;
			this.mRecvTimeOut = recvTimeOut;
			this.mIsNoDelay = noDelay;
			this.mRecvBufSize = recvBufSize;
			this.mSendBufSize = sendBufSize;
		}
	}
}

