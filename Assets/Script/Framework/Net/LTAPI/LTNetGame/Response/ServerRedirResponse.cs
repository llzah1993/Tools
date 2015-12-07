namespace LTNet
{
	public class ServerRedirResponse : Response
	{
		private string mRedirHost;
		private int mRedirPort;

		public string GetRedirHost()
		{
			return mRedirHost;
		}

		public int GetRedirPort()
		{
			return mRedirPort;
		}

		public override void Deserialize(DataStream reader)
		{
			base.Deserialize(reader);

			mRedirPort = (int)reader.ReadInt32();
			mRedirHost = reader.ReadString8();
		}
	}
}