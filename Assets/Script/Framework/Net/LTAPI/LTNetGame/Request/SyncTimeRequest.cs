using LTNet;

public class SyncTimeRequest : RedAlertRequestBase
{
	public SyncTimeRequest()
	{
	}

	protected override void SetMessageId()
	{
		mMessageId = NetProtocols.TIME_REQ;
	}
}