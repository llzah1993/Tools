using UnityEngine;
using System.Collections;

public class NetProtocols
{

    public static readonly uint SERVER_INSTANCE = 0x0501;
    public static readonly byte MESSAGE_PROTOCOL = 0x1;
    public static readonly byte P2P_MESSAGE_PROTOCOL = 0x2;

    public static readonly uint GAME_NETLOGIC_REQ = 123456;
    public static readonly uint GAME_NETLOGIC_RESP = 123456;

    public static readonly uint GAME_MODULE_REQ = 1111;
    public static readonly uint GAME_MODULE_RESP = 1111;

    public static readonly uint C_HEART_BEAT = 0x10050014;	// 发送心跳测试下心跳吧


    public static readonly uint LOGIN_GAME_REQ = 0x2;
    public static readonly uint LOGIN_GAME_RESP = 0x3;

    public static readonly uint ENTER_GAME_REQ = 0x00010001;
    public static readonly uint ENTER_GAME_RESP = 0x00010002;

    public static readonly uint UPDATE_ROOM_LIST_REQ = 0x00020001;
    public static readonly uint UPDATE_ROOM_LIST_RESP = 0x00020002;

    public static readonly uint ENTER_ROOM_REQ = 0x00020003;
    public static readonly uint ENTER_ROOM_RESP = 0x00020004;
    public static readonly uint LEAVE_ROOM_REQ = 0x00020005;
    public static readonly uint LEAVE_ROOM_RESP = 0x00020006;

    public static readonly uint READY_REQ = 0x00040001;
    public static readonly uint READY_RESP = 0x00040002;

    public static readonly uint START_LOADING_REQ = 0x00040003;//暂时没有用到
    public static readonly uint START_LOADING_RESP = 0x00040004;

    public static readonly uint END_LOADING_REQ = 0x00040005;
    public static readonly uint END_LOADING_RESP = 0x00040006;

    public static readonly uint TIME_REQ = 0x00040009;
    public static readonly uint TIME_RESP = 0x00040010;

    public static readonly uint MOVE_REQ = 0x00040007;
    public static readonly uint MOVE_RESP = 0x00040008;

    /// <summary>
    /// 模块号，请求查看大地图上的建筑、怪物信息，李鹏定的
    /// </summary>
    public static readonly uint HANDLE_PROTO_MAP_VIEW = 0x00001001;
}
