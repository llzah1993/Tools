namespace LTNet
{
	using UnityEngine;
	using System.Collections;

	public interface ISocketConnector
	{
		void BindSocket(NetSocket socket);
		void Connect();
		void Disconnect();
		bool IsConnected();
	
		NetSocket Socket { get; }
		bool UsingAsync { get; set;}
	}
}