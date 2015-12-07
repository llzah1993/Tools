using UnityEngine;
using System.Collections;
using Framework;
using System.Collections.Generic;
using System;

namespace Framework
{
    /// <summary>
    /// 下拉列表管理
    /// </summary>
    public class ComboBoxManage : MonoBehaviour
    {

        //存放当前场景中所有下拉列表控件
        private List<ComboBox> L_ComboBoxControls;

        private void Start()
        {
            //初始化
            L_ComboBoxControls = new List<ComboBox>();
            var listControls = GameObject.FindGameObjectsWithTag("ComboBox");
            foreach (var item in listControls)
            {
                L_ComboBoxControls.Add(item.transform.GetComponent<ComboBox>());
            }

            //绑定事件

            //选项
            var itemComb1 = new ComboBoxItem("itemComb1");
            var itemComb2 = new ComboBoxItem("itemComb2");
            ComboBoxEventHandler.addHandler(itemComb1, ItemComb1);
            ComboBoxEventHandler.addHandler(itemComb2, ItemComb2);



            //将列表项添加进入各自的下拉列表
            foreach (var item in L_ComboBoxControls)
            {
                switch (item.name)
                {
                    case "iTEM":
                        addItems(item, new ArrayList { itemComb1, itemComb2 });
                        break;
                }
            }
        }


        /// <summary>
        /// 添加列表项
        /// </summary>
        private void addItems(ComboBox comb, ArrayList names)
        {
            foreach (var item in names)
            {
                comb.AddItems(item);
            }
        }

        private void ItemComb1()
        {
            ADebug.Log("hello1");
        }

        private void ItemComb2()
        {
            ADebug.Log("hello2");
        }
    }
}
