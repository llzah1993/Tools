using UnityEngine;
using System.Collections;

namespace Framework
{
    public class CoroutineManager : MonoBehaviour
    {


        private static CoroutineManager instance;

        public static CoroutineManager Instance
        {
            get
            {
                return instance;
            }
        }

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

