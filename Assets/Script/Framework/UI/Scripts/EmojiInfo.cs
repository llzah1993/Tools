/****************************************************** 
CopyRight：LeTang

FileName: EmojiInfo.cs

Writer: Karajan

Create Date: 2015-11-03

Main Content(Function Name、parameters、returns) 

 ******************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

///〈summary〉 

///Description：存储表情的ID和位置

///Author：甄扬 

///Create Date：2015-11-03

///〈/summary〉
/// 
namespace Framework
{
    public class EmojiInfo : MonoBehaviour
    {

        public List<SpriteInfo> spriteList;

        [Serializable]
        public class SpriteInfo
        {
            public Sprite spriteName;
            public string spriteID;
        }

    }

}
