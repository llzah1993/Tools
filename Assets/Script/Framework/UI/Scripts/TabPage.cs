using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class TabPage : MonoBehaviour
    {

        public List<TabToggleObject> pageList = new List<TabToggleObject>();
        public int startPageIndex = 0;

        public delegate void OnPageIndexChangeDelegate(GameObject page, int index);
        public OnPageIndexChangeDelegate OnPageIndexChange;

        [HideInInspector]
        public GameObject foucsPage;
        [HideInInspector]
        public int pageIndex = -1;

        void Start()
        {
            LoadPage();
            SetPageIndex(startPageIndex);
        }

        void Update()
        {

        }

        public void SetPageIndex(int index)
        {
            if (pageIndex != index)
            {

                pageIndex = index;
                for (int i = 0; i < pageList.Count; i++)
                {
                    TabToggleObject pv = pageList[i];
                    if (i == index)
                    {
                        Toggle btn = pv.gameObject.GetComponent<Toggle>();
                        btn.isOn = true;
                        btn.enabled = false;
                        pv.pageObject.SetActive(true);
                        foucsPage = pv.gameObject;
                    }
                    else
                    {
                        Toggle btn = pv.gameObject.GetComponent<Toggle>();
                        btn.isOn = false;
                        btn.enabled = true;
                        pv.pageObject.SetActive(false);
                    }
                }

                if (OnPageIndexChange != null)
                {
                    OnPageIndexChange(foucsPage, pageIndex);
                }

            }
            else
            {
                GameObject obj = EventSystem.current.currentSelectedGameObject;
                Toggle toggle = obj.GetComponent<Toggle>();
                toggle.isOn = true;
            }
        }


        protected void LoadPage()
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                TabToggleObject pv = pageList[i];
                pv.Load(transform);
            }
        }

        protected void Reload()
        {
            pageIndex = -1;
            SetPageIndex(startPageIndex);
        }

        public void OnToolgeValueChange(bool value)
        {
            GameObject obj = EventSystem.current.currentSelectedGameObject;
            if (obj != null)
            {
                int index = FindeToggleObjectIndex(obj);
                if (index != -1)
                {
                    SetPageIndex(index);
                }
                //             else
                //             {
                //                 UnityEngine.Debug.LogError("Can't Find Tab Page" + obj.name);
                //             }
            }
        }

        protected int FindeToggleObjectIndex(GameObject obj)
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                TabToggleObject lttogobj = pageList[i];
                if (lttogobj.gameObject == obj)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

