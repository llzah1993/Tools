using System;
using LTNet;
using LTUnityPlugin;

public abstract class RedAlertRequestBase : Request
{
	protected override void SetServerInstance()
	{
		mDestServerInstance = PluginManager.PluginInstance<AccountCenter>().ServerInstanceId;
	}
	
	protected override void SetProtocol()
	{
		mProtocol = NetProtocols.MESSAGE_PROTOCOL;
	}
}

