using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {

    public static TimeManager Self;

    ulong mServerTimeSynced;//millisecond
	float mClientTimeStartSync;//second
	float mEstimatedDelayToServer;//second
    float mClientTimeReceiveResponse;//second
    void Start()
    {
        Self = this;

		NetworkManager.Self.RegisterHandler<SyncTimeResponse>((msg) =>{
			SyncTimeResponse data = (SyncTimeResponse)msg.GetMessageData();
			mEstimatedDelayToServer = (Time.time - mClientTimeStartSync) * 0.5f;
			mServerTimeSynced = data.mServerTimeStamp + (ulong)(mEstimatedDelayToServer * 1000);
            mClientTimeReceiveResponse = Time.time;
		});
    }

    public float GetDelayToServer()
    {
        return mEstimatedDelayToServer;
    }

    public float ConvertToClientTime(ulong serverTime)
    {
		if (serverTime == 0)
			return 0;

        ulong dt = serverTime - mServerTimeSynced;
        return mClientTimeStartSync + mEstimatedDelayToServer + (dt * 0.001f);
    }

    public ulong GetCurrentServerTime()
    {
        float dt = Time.time - mClientTimeReceiveResponse;
		return mServerTimeSynced + (ulong)(dt * 1000);
    }

    public void RequestServerTime()
    {
        SyncTimeRequest req = new SyncTimeRequest();
        req.Send(NetworkManager.Connector);
        mClientTimeStartSync =  Time.time;
    }
}
