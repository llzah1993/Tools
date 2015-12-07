using UnityEngine;
using System.Collections;


namespace Framework
{
    public class TabToggleObject : MonoBehaviour
    {

        public GameObject pageObject;

        public bool posSetting = false;
        public Vector3 pageLocalPostion = new Vector3(0, 0, 0);
        public Vector3 pageLocalScale = new Vector3(1, 1, 1);
        public Vector3 pageLocalRotation = new Vector3(0, 0, 0);
       
        public void Load(Transform parent)
        {
            if (pageObject == null)
            {
                ADebug.LogError("pageObject is null please setting in Inspector");
                return;
            }
            if (pageObject.transform.parent != parent)
            {
                pageObject = (GameObject)GameObject.Instantiate(pageObject);
                pageObject.transform.parent = parent;
                pageObject.transform.localPosition = Vector3.zero;
                pageObject.transform.localScale = Vector3.one;
                pageObject.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                pageObject.transform.SetParent(parent);
            }
            if (posSetting)
            {
                pageObject.transform.localPosition = pageLocalPostion;
                pageObject.transform.localScale = pageLocalScale;
                pageObject.transform.localEulerAngles = pageLocalRotation;
            }
            pageObject.transform.SetAsFirstSibling();
            pageObject.SetActive(false);
        }
    }

}
