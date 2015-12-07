using LTNet;
using LTUnityPlugin;

class LiteRequest : Request {
    uint m_moduleMsgID;
    string format = string.Empty;
    System.Object[] values;

    public LiteRequest(uint msgID) {
        m_moduleMsgID = msgID;
    }

    public LiteRequest(uint msgID, string _format, System.Object[] _values) {
        m_moduleMsgID = msgID;
        format = _format;
        values = _values;
    }

    protected override void SetServerInstance() {
        mDestServerInstance = PluginManager.PluginInstance<AccountCenter>().ServerInstanceId;
    }

    protected override void SetProtocol() {
        mProtocol = NetProtocols.MESSAGE_PROTOCOL;
    }

    protected override void SetMessageId() {
        mMessageId = NetProtocols.GAME_NETLOGIC_REQ;
    }

    public uint ModuleMsgID {
        get { return m_moduleMsgID; }
    }

    public override void Serialize(DataStream writer) {
        base.Serialize(writer);
        writer.WriteInt32(m_moduleMsgID);
        int len = format.Length;
        for(int i = 0; i < len; i++) {
            char token = format[i];

            switch(token) {
                case 'b':
                    writer.WriteByte((byte)System.Convert.ToByte(values[i]));
                    break;
                case 'h':
                    writer.WriteInt16((ushort)System.Convert.ToInt16(values[i]));
                    break;
                case 'i':
                    writer.WriteInt32((uint)System.Convert.ToInt32(values[i]));
                    break;
                case 'l':
                    writer.WriteInt64((ulong)System.Convert.ToInt64(values[i]));
                    break;
                case 'f':
                    writer.WriteFloat((float)System.Convert.ToSingle(values[i]));
                    break;
                case 's':
                    writer.WriteString8((string)values[i]);
                    break;
                case 'w':
                    writer.WriteString16((string)values[i]);
                    break;
            }
        }
    }
}
