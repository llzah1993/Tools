﻿using LTNet;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestForModuleDispatch2 : MonoBehaviour
{

    private int a;

    //public Action action;

    // Use this for initialization
    void Start()
    {
        ModuleResponse.ModCode_ModBufferDic.Add(ModuleResponse.HANDLE_MODU_TESTS, new ModuleHandle(DealModule));
        a = 100;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DealModule(DataStream reader)
    {
        DateTime now = DateTime.Now;
        ulong elapsed_time = reader.ReadInt64();
        DateTime begin = new DateTime((long)elapsed_time);
        long elapsedTicks = now.Ticks - begin.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        Debug.Log("moddelay = " + elapsedSpan.TotalSeconds.ToString() + "  ;" + this.a);
        uint id = reader.ReadInt32(); //ID
        ushort xx = reader.ReadInt16(); //X
        ushort yy = reader.ReadInt16(); //Y
        byte tp = reader.ReadByte();  //type
        ulong uid = reader.ReadInt64(); //user id
        string uname = reader.ReadString16();  //player name
        byte lev = reader.ReadByte();  //level
    }
}
