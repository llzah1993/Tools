using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using LTNet;
using LTUnityPlugin;
using Framework;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Self;

    public static SocketConnector Connector { get; set; }

    public static List<KeyValuePair<Type, MessageHandler>> mHandlerList = new List<KeyValuePair<Type, MessageHandler>>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Self = this;
    }

    void Start()
    {
        this.RegisterAll();
        MessageDispatcher.Instance.RegisterHandler(OnMessageReceived);

        Connector = gameObject.AddComponent<SocketConnector>();
        //Connector.BindSocket(new NetSocket("wuxiadns.joymeng.com", 60000));
        //Connector.UsingAsync = true;
        //Connector.RegisterConnectCallback(OnConnectDone, null);
        //Connector.Connect();
    }

    public void OnConnectDone(SocketConnector connector, object userdata)
    {
        if (connector.IsConnected())
            ADebug.Log("connected successfully");
        else
            ADebug.Log("conntection failed");
    }

    public void OnMessageReceived(Message message)
    {
        //DebugUtil.Log("got a message, protocol: " + message.GetProtocol());

        if (message.GetProtocol() == 0xFF)
        {
            //server redirection
            ServerRedirResponse redirMsg = (ServerRedirResponse)message.GetMessageData();

            Connector.BindSocket(new NetSocket(redirMsg.GetRedirHost(), redirMsg.GetRedirPort(), null));
            Connector.UsingAsync = false;
            Connector.Connect();
            RegisterHandler<LoginResponse>(delegate(Message msg)
            {
                LoginResponse temp = msg.GetMessageData() as LoginResponse;
                if (temp.mResult == 0)
                { // success
                    Debug.Log("可以跟进游戏服务器了!");
                    //gameObject.AddComponent<RequestUpdateMap>();
                }
                else
                {
                    Debug.LogError("连接esb服务器失败!");
                }
            });
            LoginRequest loginReq = new LoginRequest(ulong.Parse(PluginManager.PluginInstance<AccountCenter>().JoyId), PluginManager.PluginInstance<AccountCenter>().Token);
            loginReq.Send(Connector);
        }
        else if (message.GetProtocol() == NetProtocols.MESSAGE_PROTOCOL)
        {
            IMessageData data = message.GetMessageData();
            if (data != null)
            {
                //dispatch to subscribers
                foreach (KeyValuePair<Type, MessageHandler> pair in mHandlerList)
                {
                    if (data.GetType() == pair.Key)
                    {
                        pair.Value(message);
                    }
                }
            }
        }
        else
        {
            ADebug.Log("unknown protocol: {0}" , message.GetProtocol());
        }
    }

    public void RegisterHandler<T>(MessageHandler handler) where T : Response
    {
        mHandlerList.Add(new KeyValuePair<Type, MessageHandler>(typeof(T), handler));
    }

    public void UnregisterHandler<T>(MessageHandler handler) where T : Response
    {
        List<KeyValuePair<Type, MessageHandler>> toRemove = new List<KeyValuePair<Type, MessageHandler>>();

        foreach (KeyValuePair<Type, MessageHandler> pair in mHandlerList)
        {
            if (pair.Key == typeof(T) && pair.Value == handler)
                toRemove.Add(pair);
        }

        foreach (KeyValuePair<Type, MessageHandler> pair in toRemove)
        {
            mHandlerList.Remove(pair);
        }
    }

    /// <summary>
    /// remove all handler via type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void UnregisterHandler<T>() where T : Response
    {
        List<KeyValuePair<Type, MessageHandler>> toRemove = new List<KeyValuePair<Type, MessageHandler>>();

        foreach (KeyValuePair<Type, MessageHandler> pair in mHandlerList)
        {
            if (pair.Key == typeof(T))
                toRemove.Add(pair);
        }

        foreach (KeyValuePair<Type, MessageHandler> pair in toRemove)
        {
            mHandlerList.Remove(pair);
        }
    }

    public void RegisterAll()
    {
        //register all responses
        ProtoManager.Instance.AddProtocol<ServerRedirResponse>(0xFF, 0);
        ProtoManager.Instance.AddProtocol<LoginResponse>(NetProtocols.MESSAGE_PROTOCOL, NetProtocols.LOGIN_GAME_RESP);
        //ProtoManager.Instance.AddProtocol<LiteResponse>(NetProtocols.MESSAGE_PROTOCOL, NetProtocols.GAME_NETLOGIC_REQ);
        ProtoManager.Instance.AddProtocol<LiteResponse>(NetProtocols.MESSAGE_PROTOCOL, NetProtocols.GAME_NETLOGIC_RESP);
        ProtoManager.Instance.AddProtocol<ModuleResponse>(NetProtocols.MESSAGE_PROTOCOL, NetProtocols.GAME_MODULE_RESP);

        //ProtoManager.Instance.AddProtocol<UpdateRoomListResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.UPDATE_ROOM_LIST_RESP);
        //ProtoManager.Instance.AddProtocol<EnterRoomResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.ENTER_ROOM_RESP);
        //ProtoManager.Instance.AddProtocol<ReadyGameResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.READY_RESP);
        //ProtoManager.Instance.AddProtocol<StartLoadingResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.START_LOADING_RESP);
        //ProtoManager.Instance.AddProtocol<EndLoadingResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.END_LOADING_RESP);

        //ProtoManager.Instance.AddProtocol<PositionResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.MOVE_RESP);
        //ProtoManager.Instance.AddProtocol<SyncTimeResponse>(NetProtocols.MESSAGE_PROTOCOL, (uint)NetProtocols.TIME_RESP);
    }

}
