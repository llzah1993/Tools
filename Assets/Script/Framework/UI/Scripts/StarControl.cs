using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Framework
{
    public class StarControl : MonoBehaviour
    {

        public Sprite bgSprite;
        public Sprite fillSprite;

        protected float lastnum;
        public float shownum;
        public float totalnum;

        public Image bgImage;
        public Image fillImage;


        protected Vector2 bgCell;
        protected Vector2 fiilCell;

        void Start()
        {
            General();
        }
        void Update()
        {

            UpdateShow();

        }

        public void General()
        {
            bgImage.sprite = bgSprite;
            fillImage.sprite = fillSprite;

            bgCell.x = bgSprite.textureRect.width;
            bgCell.y = bgSprite.textureRect.height;

            fiilCell.x = fillSprite.textureRect.width;
            fiilCell.y = fillSprite.textureRect.height;

            RectTransform bgtf = bgImage.GetComponent<RectTransform>();
            RectTransform filltf = fillImage.GetComponent<RectTransform>();
            float bgwidth = totalnum * bgCell.x;
            bgtf.sizeDelta = new Vector2(bgwidth, bgCell.y);
            float fillwidth = totalnum * fiilCell.x;
            filltf.sizeDelta = new Vector2(fillwidth, fiilCell.y);

            filltf.localPosition = new Vector3(0, 0, 0);

        }

        public void SetNum(float num)
        {
            shownum = num;
        }

        protected void UpdateShow()
        {
            if (shownum != lastnum)
            {
                RectTransform filltf = fillImage.GetComponent<RectTransform>();
                float fillwidth = shownum * fiilCell.x;
                filltf.sizeDelta = new Vector2(fillwidth, fiilCell.y);
                lastnum = shownum;
            }
        }

    }
}


