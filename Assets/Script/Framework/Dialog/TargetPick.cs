using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework;

namespace Framework
{
    public class TargetPick : MonoBehaviour
    {//llllll 叫我zynga的搬运工

        public Image icon;

        public delegate void UITargetPickClickHandler();

        public event UITargetPickClickHandler ClickHandler;

        public long endTime;

        public int duration;

        private Slider slider;

        public Text progressLabel;

        private bool isHideProgressbar;

        public string readyIconName;

        private bool isHidePick;

        private void OnClick()
        {
            if (this.ClickHandler != null )
            {
                this.ClickHandler();
            }
        }

        public void SetIcon(string iconName)
        {
            icon.overrideSprite.name = iconName;
            ATargetPickManager instance = ATargetPickManager.instance;
            Vector2 vector = this.icon.rectTransform.sizeDelta;
            if (vector.x > vector.y)
            {
                vector.y = (vector.y * instance.iconWidth) / vector.x;
                vector.x = instance.iconWidth;
            }
            else
            {
                vector.x = (vector.x * instance.iconHeight) / vector.y;
                vector.y = instance.iconHeight;
            }
            this.icon.rectTransform.sizeDelta = vector;
            Vector3 vector2 = this.icon.gameObject.transform.localPosition;
            vector2.x = 0f;
            this.icon.gameObject.transform.localPosition = vector2;
        }

        private void Start()
        {
            this.slider = base.gameObject.GetComponentInChildren<Slider>();
            this.UpdateActive();
        }

        private void UpdateActive()
        {
            bool flag = this.progress >= 1.0f;
            if (this.slider != null)
            {
                bool flag2 = !flag && !isHideProgressbar;
                this.slider.gameObject.SetActive(flag2);
            }
        }

        protected void Update()
        {
            if(this.slider!=null && this.slider.value <= 1)
            {
                this.slider.value = (this.progress < 1.0) ? (this.progress) : 1f;
                if (this.progressLabel != null)
                {
                    this.progressLabel.text = this.progressStr;
                }
                if (this.progress >= 1.0)
                {
                    ADebug.Assert(ATargetPickManager.instance!=null);
                    if (this.readyIconName != null)
                    {
                        this.SetIcon(this.readyIconName);
                    }
                }
            }
            
            UpdateActive();
        }

        private void OnDestroy()
        {
            ATargetPickManager.instance.Remove(this);
        }

        public bool IsHidePick
        {
            get
            {
                return this.isHidePick;
            }
            set
            {
                this.isHidePick = value;
                if (this.isHidePick)
                {
                    this.gameObject.SetActive(false);
                }
                else
                {
                    if (ATargetPickManager.isShow)
                    {
                        this.gameObject.SetActive(true);
                    }
                }
            }
        }

        public float progress
        {
            get
            {
                float num = RATime.SecondsUntil(this.endTime);
                return ((this.duration <= 0) ? 1f : ((this.duration - num) / ((float)this.duration)));
            }
        }

        public string progressStr
        {
            get
            {
                float num = RATime.SecondsUntil(this.endTime);
                return RATime.SecondsAsFormattedString((int)num);
            }
        }
    }
}

