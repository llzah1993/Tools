namespace LTNet
{
	//classes which implement this interface can be sent/received via network
	public interface IMessageData
	{
		byte Protocol{get;}

		void Serialize(DataStream writer);
		void Deserialize(DataStream reader);
	}
}