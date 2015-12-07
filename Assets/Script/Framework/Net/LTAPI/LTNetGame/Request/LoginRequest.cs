using LTNet;

public class LoginRequest : RedAlertRequestBase
{
	public ulong mUserId;
	public string mToken;
	
	public LoginRequest(ulong id , string token)
	{
		mUserId = id;
		mToken = token;
	}

    // µÇÂ¼ ÊµÀýºÅÊ¼ÖÕÎª0xffff
	protected override void SetServerInstance ()
	{
		mDestServerInstance = 0xFFFF;
	}

	protected override void SetMessageId()
	{
		mMessageId = NetProtocols.LOGIN_GAME_REQ;
	}
	
	public override void Serialize(DataStream writer)
	{
		base.Serialize(writer);
		writer.WriteInt64(mUserId);
		writer.WriteString16(mToken);
	}
	
}