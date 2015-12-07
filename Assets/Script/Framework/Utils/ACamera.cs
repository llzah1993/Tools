using UnityEngine;
using System.Collections;

namespace Framework
{
    public class ACamera : MonoBehaviour
    {
        public static ACamera instance;

        public static float depthSortZ = 1f;

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// return a viewport vector3 due to vec that is in world, with a offset in y.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="uiOffset"></param>
        /// <returns></returns>
        public static Vector3 WorldToUI(Vector3 vec, float uiOffset = 0.0f)
        {
            vec = vec + new Vector3(0f, uiOffset, 0f);
            vec = Camera.main.WorldToViewportPoint(vec);
            vec = AUIManager.instance.uiCamera.ViewportToWorldPoint(vec);
            vec.z = depthSortZ;
            return vec;
        }
    }
}
