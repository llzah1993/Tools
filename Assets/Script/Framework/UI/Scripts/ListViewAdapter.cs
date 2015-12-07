using UnityEngine;
using System.Collections;
using Framework;

namespace Framework
{
    public abstract class ListViewAdapter : MonoBehaviour
    {

        [HideInInspector]
        public ListView listview;

        public virtual void Initialize()
        {

        }

        public abstract int GetCount();


        public abstract void FillItemData(GameObject item, int cellindex);


        public virtual void AddItem(int index, object data)
        {
        }

        public virtual void RemoveItem(int index)
        {

        }

        public virtual void ClearItem()
        {

        }

    }
}


