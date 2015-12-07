using LTNet;

public class ReadyGameRequest : RedAlertRequestBase
{
	protected override void SetMessageId()
	{
		mMessageId = (uint)NetProtocols.READY_REQ;
	}
}
