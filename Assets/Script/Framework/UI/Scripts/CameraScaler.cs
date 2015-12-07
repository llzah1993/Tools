
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Framework
{
    [ExecuteInEditMode]
    public class CameraScaler : MonoBehaviour
    {
        [DisplayAttribute(typeof(int), "设计宽度")]
        public int desginWidth = 768;

        [DisplayAttribute(typeof(int), "设计高度")]
        public int desginHeight = 1024;

        protected int lastw = 0;
        protected int lasth = 0;
        void Start()
        {
        }

        void Update()
        {
            if (lastw != Screen.width
                || lasth != Screen.height)
            {
                InvalidateScale();
            }
        }

        protected void InvalidateScale()
        {
            //float scalex = (float)Screen.width / (float)desginWidth;
            //float scaley = (float)Screen.height / (float)desginHeight;
            //gameObject.transform.localScale = new Vector3(scalex, scaley, 1);
            //lastw = Screen.width;
            //lasth = Screen.height;
        }
    }
}

