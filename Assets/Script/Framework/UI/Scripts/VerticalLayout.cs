/****************************************************** 
CopyRight：LeTang

FileName: VerticalLayout.cs

Writer: Karajan

Create Date: 2015-11-03

Main Content(Function Name、parameters、returns) 

 ******************************************************/
using UnityEngine;
using System.Collections;

///〈summary〉 

///Description：控制图文混排控件中每行的位置 

///Author：甄扬 

///Create Date：2015-11-03

///〈/summary〉
/// 
namespace Framework
{
    public class VerticalLayout : MonoBehaviour
    {

        public int LineSpacing;
        public int TopSpacing;
        public int LeftSpacing; //暂不支持
        public int RightSpacing;    //暂不支持

        private int mTopDistance;

        public int TopDistance { get { return mTopDistance; } set { mTopDistance = value; } }

        public void SetLinePosition(GameObject line, int width, int height)
        {
            if (mTopDistance != TopSpacing)
                mTopDistance += height;
            else
                mTopDistance += 1;
            RectTransform rt = line.GetComponent<RectTransform>();
            rt.anchorMax = rt.anchorMin = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(width, height);
            rt.localScale = new Vector3(1f, 1f, 1f);
            rt.localPosition = new Vector3(0f, -mTopDistance, 0f);
        }

        /// <summary>
        /// 重置行的位置
        /// </summary>
        public void ResetLinePosition()
        {
            mTopDistance = TopSpacing;
        }

        // Use this for initialization
        void Start()
        {
            mTopDistance = TopSpacing;
        }

    }
}


