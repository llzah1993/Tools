using UnityEngine;
using System.Collections;
using Framework;

namespace Framework
{
    public abstract class LTGridPackageAdapter : MonoBehaviour
    {

        [HideInInspector]
        public LTGridPackage gridPackage;

        public virtual void initialize()
        {

        }

        public abstract int getCount();


        public abstract void FillItemData(GameObject item, int cellindex);


        public virtual void AddItem(int insertIndex, object data)
        {
        }

        public virtual void RemoveItem(int insertIndex)
        {

        }

        public virtual void ClearItem()
        {

        }
    }
}
