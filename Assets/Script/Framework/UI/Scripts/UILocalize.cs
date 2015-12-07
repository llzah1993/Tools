using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class UILocalize : MonoBehaviour
    {
        public string key;

        private string mLanguage;

        private bool mStarted;

        public void Localize()
        {
            ALocalization localization = ALocalization.instance;
            MaskableGraphic component = gameObject.GetComponent<MaskableGraphic>();
            Text uILabel = component as Text;
            Image uISprite = component as Image;
            if (string.IsNullOrEmpty(this.mLanguage) && string.IsNullOrEmpty(this.key) && uILabel != null)
            {
                this.key = uILabel.text;
            }
            string str = (!string.IsNullOrEmpty(this.key) ? localization.Get(this.key) : localization.Get(component.name));
            if (uILabel != null)
            {
                uILabel.text = str;
            }
            else if (uISprite != null)
            {
                uISprite.sprite = Resources.Load(str, typeof(Sprite)) as Sprite; ;
            }
            this.mLanguage = localization.currentLanguage;
        }

        private void OnEnable()
        {
            if (this.mStarted && ALocalization.instance != null)
            {
                this.Localize();
            }
        }

        private void Start()
        {
            this.mStarted = true;
            if (ALocalization.instance != null)
            {
                this.Localize();
            }
        }
    }
}
