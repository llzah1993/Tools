/****************************************************** 
CopyRight：LeTang

FileName: UIAnimation.cs

Writer: Karajan

Create Date: 2015-11-03

Main Content(Function Name、parameters、returns) 

 ******************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///〈summary〉 

///Description：动画表情 

///Author：甄扬 

///Create Date：2015-11-03

///〈/summary〉
/// 
namespace Framework
{
    public class UIAnimation : MonoBehaviour
    {

        public bool isAnimation = false;
        public Sprite[] Sprites;
        public float TimeSpacing = 0.5f;

        private float mTimer;

        private int mCurrentIndex;

        private Image m_Image;

        public int CurrentIndex { get { return mCurrentIndex; } }

        void Start()
        {
            m_Image = GetComponent<Image>();
            mCurrentIndex = 0;
            mTimer = 0;
            if (Sprites != null && Sprites.Length > 0)
                m_Image.sprite = Sprites[0];
        }

        void Update()
        {
            mTimer += Time.deltaTime;
            if (isAnimation == true && TimeSpacing < mTimer)
            {
                if (mCurrentIndex < Sprites.Length)
                    m_Image.sprite = Sprites[mCurrentIndex++];
                else
                    mCurrentIndex = 0;
                mTimer = 0;
            }
        }

    }
}

