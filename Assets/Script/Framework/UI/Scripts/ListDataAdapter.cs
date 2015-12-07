using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Framework;

namespace Framework
{
    public class ListDataAdapter : ListViewAdapter
    {

        public Dictionary<int, Dictionary<int, string>> dic = new Dictionary<int, Dictionary<int, string>>();

        public override void FillItemData(GameObject item, int cellindex)
        {
            InfoListCell listCell = item.GetComponent<InfoListCell>();

            InitBoxText(listCell, dic[0].Count, cellindex);

            if (cellindex % 2 == 0)
            {
                listCell.bg.sprite = Resources.Load("Framework/Sprites/valueBg", typeof(Sprite)) as Sprite;
                listCell.bg.color = new Color(255, 255, 255, 255);
            }
            else
            {
                listCell.bg.sprite = Resources.Load("Framework/Sprites/valueBg", typeof(Sprite)) as Sprite;
                listCell.bg.color = new Color(255, 255, 255, 0.5f);
            }
            if (dic[cellindex][1] == "3")//当前等级背景框变换
            {
                listCell.bg.overrideSprite  = Resources.Load("Framework/Sprites/value1", typeof(Sprite)) as Sprite;
                listCell.bg.color           = new Color(255, 255, 255, 255);
            }
        }

        public void InitBoxText(InfoListCell listCell, int boxNum, int cellindex)
        {
            listCell.InitCellBox(boxNum);
            for (int i = 0; i < listCell.boxTextList.Count; i++)
            {
                listCell.boxTextList[i].text = dic[cellindex][i + 1];
            }
        }

        public override int GetCount()
        {
            return dic.Count;
        }

        public override void Initialize()
        {
            testInit();
        }

        // Use this for initialization
        public void testInit()
        {
            Dictionary<int, string> test0 = new Dictionary<int, string>();
            test0.Add(1, "等级"); test0.Add(2, "爆炸"); test0.Add(3, "基佬"); test0.Add(4, "希尔瓦娜斯");

            Dictionary<int, string> test1 = new Dictionary<int, string>();
            test1.Add(1, "1"); test1.Add(2, "10"); test1.Add(3, "10"); test1.Add(4, "10");
            Dictionary<int, string> test2 = new Dictionary<int, string>();
            test2.Add(1, "2"); test2.Add(2, "20"); test2.Add(3, "10"); test2.Add(4, "10");
            Dictionary<int, string> test3 = new Dictionary<int, string>();
            test3.Add(1, "3"); test3.Add(2, "30"); test3.Add(3, "10"); test3.Add(4, "10");
            Dictionary<int, string> test4 = new Dictionary<int, string>();
            test4.Add(1, "4"); test4.Add(2, "40"); test4.Add(3, "10"); test4.Add(4, "10");
            Dictionary<int, string> test5 = new Dictionary<int, string>();
            test5.Add(1, "5"); test5.Add(2, "50"); test5.Add(3, "10"); test5.Add(4, "10");

            dic.Add(0, test0);
            dic.Add(1, test1); dic.Add(2, test2); dic.Add(3, test3); dic.Add(4, test4); dic.Add(5, test5);
        }
    }
}


