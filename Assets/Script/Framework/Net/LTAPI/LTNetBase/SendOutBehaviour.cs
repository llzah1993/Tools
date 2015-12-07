namespace LTNet
{
	using System;

	public abstract class SendOutBehaviour
	{
		protected abstract void SetProtocol();
		protected abstract void SetServerInstance();
		protected abstract void SetMessageId();
		protected abstract Message CreateMessage();
	}
}
