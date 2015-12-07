namespace LTNet
{
	using System;
	using System.Collections;
	using System.Net;

	public interface INetSocket
	{
		void Connect();
		void Connect(string host, int port);

		void ConnectAsync();
		void ConnectAsync(Action<object> callback, object userdata);
		void ConnectAsync(string host, int port, Action<object> callback, object userdata);

		void Disconnect();

		bool Send(byte[] data);
		int Receive(byte[] buffer);

		string Host { get; }
	 	int Port { get; }
		bool Connected { get; }
		bool DataAvailable { get; }
	}
}