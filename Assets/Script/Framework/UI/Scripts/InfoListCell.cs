using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Framework
{
    public class InfoListCell : MonoBehaviour
    {

        public  Image           bg;

        public  GameObject      infoListCellBox;

        public  List<Text>      boxTextList = new List<Text>();

        private RectTransform   rtSelf;

        void Start()
        {
            
        }

        public void InitCellBox(int boxNum)
        {
            rtSelf = gameObject.GetComponent<RectTransform>();
            Vector2 v2 = transform.parent.GetComponents<ListView>()[0].cellSize;
            for (int i = 0; i < boxNum; i++)
            {
                GameObject      box         = Instantiate(infoListCellBox, transform.position, transform.rotation) as GameObject;
                RectTransform   boxRT       = box.GetComponent<RectTransform>();
                box.transform.SetParent(this.transform);     
                box.transform.localScale    = transform.localScale;
                boxRT.sizeDelta             = new Vector2(v2.x / boxNum, v2.y);//设置每个格子大小
                boxRT.localPosition         = new Vector3(rtSelf.position.x - v2.x / 2 + boxRT.sizeDelta.x * i + boxRT.sizeDelta.x / 2, rtSelf.position.y, rtSelf.position.z);

                boxTextList.Add(box.GetComponent<Text>());
            }
        }
    }
}

