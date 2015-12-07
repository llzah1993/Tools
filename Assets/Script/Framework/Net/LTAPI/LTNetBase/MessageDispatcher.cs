namespace LTNet
{
	using System.Collections;

	public delegate void MessageHandler(Message msg);

	public class MessageDispatcher
	{
		private static MessageDispatcher mInstance = null;
		private MessageHandler mHandler = null;

		public static MessageDispatcher Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = new MessageDispatcher();

				return mInstance;
			}
		}

		public void RegisterHandler(MessageHandler handler)
		{
			if (mHandler == null)
				mHandler = new MessageHandler(handler);
			else
				mHandler += handler;
		}

		public void UnregisterHandler(MessageHandler handler)
		{
			if (mHandler != null)
			{
				mHandler -= handler;
			}
		}

		public void Dispatch(Message msg)
		{
			if (mHandler != null)
				mHandler(msg);
		}
	}

}
