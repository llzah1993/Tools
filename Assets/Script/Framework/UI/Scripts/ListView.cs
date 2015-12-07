using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Framework;

namespace Framework
{
    public class ListView : GridLayoutGroup
    {

        Axis Direction
        {
            get
            {
                ScrollRect sr = transform.parent.GetComponent<ScrollRect>();

                if (sr != null && sr.horizontal)
                {
                    this.constraint = Constraint.FixedRowCount;
                    return Axis.Horizontal;
                }
                else if (sr != null && sr.vertical)
                {
                    this.constraint = Constraint.FixedColumnCount;
                    return Axis.Vertical;
                }

                return Axis.Vertical;
            }
        }

        public int Count
        {
            get
            {
                if (_adapter == null)
                {
                    return 0;
                }
                return Adapter.GetCount();
            }
        }

        public string       cellname;
        public GameObject   cellprefab;
        public bool         fitCellPrefab = true;

        public int onepagecount;
        [SerializeField, SetProperty("Adapter")]
        private ListViewAdapter _adapter;

        public ListViewAdapter Adapter
        {
            get
            {
                return _adapter;
            }
            set
            {
                _adapter = value;
                if (_adapter != null)
                {
                    if (Application.isPlaying)
                    {
                        Initialize();
                    }
                }
            }
        }

        protected List<GameObject> itemCache = new List<GameObject>();

        public void RefreshData(bool reset)
        {
            FillData();
            if (reset)
            {
                goBegin();
            }
        }
        ScrollRect sr;
        IEnumerator Countdown()
        {
            yield return 10;
            sr.elasticity = 0.1f;
        }


        public void set_center(int index)
        {
            if (Direction == Axis.Vertical)
            {
                float height = base.cellSize.y * Count / 2 + base.spacing.y * (Count / 2);
                RectTransform parent_tran = transform.parent.GetComponent<RectTransform>();
                float parent_heigh = parent_tran.sizeDelta.y / 2;
                float posy = parent_heigh - height;
                posy = posy + index * (base.cellSize.y + base.spacing.y);
                rectTransform.position = new Vector3(rectTransform.position.x, posy, rectTransform.position.z);
            }
            else if (Direction == Axis.Horizontal)
            {
                float width = base.cellSize.x * Count / 2 + base.spacing.x * (Count / 2);
                RectTransform parent_tran = transform.parent.GetComponent<RectTransform>();
                float parent_width = parent_tran.sizeDelta.x / 2;
                float posx = width - parent_width;
                posx = posx - index * (base.cellSize.x + base.spacing.x);
                rectTransform.localPosition = new Vector3(posx, rectTransform.position.y, rectTransform.position.z);

                sr = transform.parent.GetComponent<ScrollRect>();
                sr.elasticity = 0;
                if (CoroutineManager.Instance != null)
                {
                    CoroutineManager.Instance.StartCoroutine(Countdown());
                }
                else
                {
                    StartCoroutine(Countdown());
                }
            }
        }


        public void goBegin()
        {
            if (Direction == Axis.Vertical)
            {
                float height = base.cellSize.y * Count / 2 + base.spacing.y * (Count / 2);
                RectTransform parent_tran = transform.parent.GetComponent<RectTransform>();
                float parent_heigh = parent_tran.sizeDelta.y / 2;
                float posy = parent_heigh - height;
                rectTransform.position = new Vector3(rectTransform.position.x, posy, rectTransform.position.z);
                sr = transform.parent.GetComponent<ScrollRect>();
                sr.elasticity = 0;
                if (CoroutineManager.Instance != null)
                {
                    CoroutineManager.Instance.StartCoroutine(Countdown());
                }
                else
                {
                    StartCoroutine(Countdown());
                }

            }
            else if (Direction == Axis.Horizontal)
            {

                float width = base.cellSize.x * Count / 2 + base.spacing.x * (Count / 2);
                RectTransform parent_tran = transform.parent.GetComponent<RectTransform>();
                float parent_width = parent_tran.sizeDelta.x / 2;
                float posx = width - parent_width;
                rectTransform.position = new Vector3(posx, rectTransform.position.y, rectTransform.position.z);

                sr = transform.parent.GetComponent<ScrollRect>();
                sr.elasticity = 0;
                if (CoroutineManager.Instance != null)
                {
                    CoroutineManager.Instance.StartCoroutine(Countdown());
                }
                else
                {
                    StartCoroutine(Countdown());
                }
            }
        }
          

        public void goEnd()
        {
            if (Direction == Axis.Vertical)
            {
                float height = base.cellSize.y * Count / 2 + base.spacing.y * (Count / 2);

                rectTransform.position = new Vector3(rectTransform.position.x, height * 0.5f, rectTransform.position.z);

            }
            else if (Direction == Axis.Horizontal)
            {
                float width = base.cellSize.x * Count / 2 + base.spacing.x * (Count / 2);
                rectTransform.position = new Vector3(width * 0.5f, rectTransform.position.y, rectTransform.position.z);
            }
        }

        public void Clear()
        {
            int count = transform.childCount;
            for (int j = 0; j < count; j++)
            {
                Transform child = transform.GetChild(j);
                GameObject.Destroy(child.gameObject);
            }
            itemCache.Clear();
        }


        public void RemoveAtIndex(int index)
        {
            if (index >= transform.childCount)
            {
                return;
            }
            Transform trans = transform.GetChild(index);
            GameObject.DestroyImmediate(trans.gameObject);

            RecalculateSize();
        }

        public void InsertAtIndex(int index, object data)
        {
            if (index < 0
                || index > transform.childCount)
            {
                return;
            }

            int count = transform.childCount;
            GameObject item = Instantiate(cellprefab);
            item.transform.parent = transform;
            item.name = "new" + index.ToString();
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            ListViewCell lvc = item.GetComponent<ListViewCell>();
            if (lvc == null)
            {
                lvc = item.AddComponent<ListViewCell>();
            }
            lvc.listView = this;

            for (int i = index; i < count - 1; i++)
            {
                Transform child = transform.GetChild(i);
                child.SetSiblingIndex(i);
            }
            item.transform.SetSiblingIndex(index);
            _adapter.FillItemData(item, index);
            RecalculateSize();
        }

        public void AddItem(object data)
        {
            if (cellprefab == null)
            {
                return;
            }
            int count = transform.childCount;
            GameObject item = Instantiate(cellprefab);
            item.name = count.ToString();
            item.transform.parent = gameObject.transform;
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            ListViewCell lvc = item.GetComponent<ListViewCell>();
            if (lvc == null)
            {
                lvc = item.AddComponent<ListViewCell>();
            }
            lvc.listView = this;
            _adapter.FillItemData(item, count);
            RecalculateSize();
        }

        public int GetItemIndex(GameObject item)
        {
            int i = 0;
            foreach (Transform child in transform)
            {
                if (item.transform == child)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        protected override void  Start()
        {
            if (Application.isPlaying)
            {
                Initialize();
            }
        }

        protected void FillData()
        {
            Clear();
            if (_adapter != null)
            {
                for (int i = 0; i < _adapter.GetCount(); i++)
                {
                    GameObject item = Instantiate(cellprefab);
                    item.name = i.ToString();
                    item.transform.SetParent(gameObject.transform);
                    item.transform.localScale = Vector3.one;
                    item.transform.localPosition = Vector3.zero;
                    ListViewCell lvc = item.GetComponent<ListViewCell>();
                    if (lvc == null)
                    {
                        lvc = item.AddComponent<ListViewCell>();
                    }
                    lvc.listView = this;
                    itemCache.Add(item);
                    _adapter.FillItemData(item, i);
                }
            }
            RecalculateSize();
        }

        protected void RecalculateSize()
        {
            int count = _adapter.GetCount();
            if (Direction == Axis.Vertical)
            {
                float height = base.cellSize.y * count + base.spacing.y * (count - 1);
                Transform parent_tran = transform.parent;
                RectTransform rect_tran = parent_tran.GetComponent<RectTransform>();
                if (height < rect_tran.sizeDelta.y)
                {
                    height = rect_tran.sizeDelta.y;
                }
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
                base.CalculateLayoutInputVertical();

            }
            else if (Direction == Axis.Horizontal)
            {
                float width = base.cellSize.x * count + base.spacing.x * (count - 1);
                Transform parent_tran = transform.parent;
                RectTransform rect_tran = parent_tran.GetComponent<RectTransform>();
                if (width < rect_tran.sizeDelta.x)
                {
                    width = rect_tran.sizeDelta.x;
                }
                rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
                base.CalculateLayoutInputHorizontal();
            }
        }


        protected void Initialize()
        {

            RectTransform trf = cellprefab.GetComponent<RectTransform>();

            if (fitCellPrefab)
            {
                base.cellSize = new Vector2(trf.sizeDelta.x, trf.sizeDelta.y);
            }

            if (base.cellSize == Vector2.zero)
            {
                ADebug.Log("ListView cellSize is Zero!!!");
            }
            if (_adapter != null)
            {
                _adapter.listview = this;
                _adapter.Initialize();
            }

            FillData();
            goBegin();
        }
    }
}




