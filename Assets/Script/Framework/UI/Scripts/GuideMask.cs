using UnityEngine;
using System.Collections;
using Framework;

namespace Framework
{
    [ExecuteInEditMode]
    public class GuideMask : MonoBehaviour
    {

        public      RectTransform   top;
        public      RectTransform   bottom;
        public      RectTransform   left;
        public      RectTransform   right;

        public      CameraScaler    cameraScaler;

        public      Vector2         RectCenter;
        public      Vector2         RectSize;
        public      bool            debugInEditor = true;

        protected   Vector2         pos_tl;
        protected   Vector2         pos_br;

        void Start()
        {

        }


        void Update()
        {
#if UNITY_EDITOR

            if (debugInEditor && !Application.isPlaying)
            {
                setRectCenter(RectCenter, RectSize);
            }
#endif
        }


        public void setRectCenter(Vector2 center, Vector2 size)
        {
            Vector2 tl = center - size * 0.5f;
            Vector2 br = center + size * 0.5f;
            setRectTopLeft(tl, br);
        }
        public void setRectTopLeft(Vector2 tl, Vector2 br)
        {
            pos_tl                  = tl;
            pos_br                  = br;
            float half_width        = cameraScaler.desginWidth / 2;
            float half_height       = cameraScaler.desginHeight / 2;

            top.localPosition       = new Vector3(0, get_top() + half_height, 0);
            bottom.localPosition    = new Vector3(0, get_bottom() - half_height, 0);

            left.localPosition      = new Vector3(get_left() - half_width, 0, 0);
            right.localPosition     = new Vector3(half_width + get_right(), 0, 0);
        }


        public float get_top()
        {
            return pos_tl.y > pos_br.y ? pos_tl.y : pos_br.y;
        }
        public float get_bottom()
        {
            return pos_tl.y < pos_br.y ? pos_tl.y : pos_br.y;
        }
        public float get_left()
        {
            return pos_tl.x < pos_br.x ? pos_tl.x : pos_br.x;
        }
        public float get_right()
        {
            return pos_tl.x > pos_br.x ? pos_tl.x : pos_br.x;
        }


    }
}

