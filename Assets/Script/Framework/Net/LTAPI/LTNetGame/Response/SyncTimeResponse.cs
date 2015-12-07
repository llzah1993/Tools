using LTNet;

public class SyncTimeResponse : Response 
{
	public ulong mServerTimeStamp;

    public SyncTimeResponse()
    {
    }

    public override void Deserialize(DataStream reader)
    {
		base.Deserialize(reader);

        mServerTimeStamp = reader.ReadInt64();
    }
}

