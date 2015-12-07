using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework;

namespace Framework
{
    public class LTGridPackage : MonoBehaviour, IEndDragHandler
    {
        public enum InvalidDataType
        {
            InvalidDataType_FrontPage = 0,
            InvalidDataType_NextPage = 1,
            InvalidDataType_All = 2,
        }

        [DisplayAttribute(typeof(int), "起始页")]
        public int startIndex = 0;

        [SerializeField, SetProperty("Adapter")]
        private LTGridPackageAdapter _adapter;


        [DisplayAttribute(typeof(LTGridPackageAdapter), "数据适配器")]
        public LTGridPackageAdapter Adapter
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

        public float animationTime = 2f;

        [HideInInspector]
        public GameObject scrollView;

        [HideInInspector]
        public LTGridPageItem[] pageItem;

        [DisplayAttribute(typeof(Vector2), "宽高")]
        [HideInInspector]
        public Vector2 packageSize = new Vector2(400, 400);

        [HideInInspector]
        public Vector2 cellSize = new Vector2(100, 100);

        [DisplayAttribute(typeof(Vector2), "间隔")]
        [HideInInspector]
        public Vector2 spaceSize = new Vector2(0, 0);

        [DisplayAttribute(typeof(int), "行")]
        [HideInInspector]
        public int column = 4;

        [DisplayAttribute(typeof(int), "列")]
        [HideInInspector]
        public int row = 4;



        [DisplayAttribute(typeof(GameObject), "cellItem")]
        [HideInInspector]
        public GameObject cellItem;

        [HideInInspector]
        public int pageIndex = 0;
        [HideInInspector]
        public int pageCount;

        [HideInInspector]
        public bool viewfillCell = true;


        public delegate void OnPageChangeDelegate(GameObject pageview, int pageindex);
        public OnPageChangeDelegate OnPageChange;


        protected GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft;
        protected GridLayoutGroup.Axis startAxis = GridLayoutGroup.Axis.Vertical;
        protected TextAnchor childAlignment = TextAnchor.UpperLeft;
        protected GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.Flexible;
        protected RectOffset padding = new RectOffset();

        protected LTGridPageItem frontPage;
        protected LTGridPageItem centerPage;
        protected LTGridPageItem behindPage;


        protected float scrollZeroOffest;
        protected float gridZeroOffest;
        protected float aniTimeIndex = 0.0f;

        protected int dataIndex = 0;
        protected int dataCount = 0;

        protected Vector3 behindBounds;
        protected Vector3 frontBounds;

        protected bool animateScroll = true;

        protected bool animatePage = false;
        protected bool autoPageFront = false;

        // Use this for initialization
        void Start()
        {
            Initialize();
        }
        // Update is called once per frame
        void Update()
        {
            if (animatePage)
            {
                AnimatePageView();
            }

            if (animateScroll)
            {
                AnimateScrollView();
            }
        }

        public void OnScrollValueChange(Vector2 v)
        {
            CheckAnimateBounds();
        }

        public void FilleDataInEditor()
        {
            LTGridPageItem pageitem = pageItem[1];

            for (int j = 0; j < pageitem.transform.childCount; j++)
            {
                Transform child = pageitem.transform.GetChild(j);
                GameObject.DestroyImmediate(child.gameObject);

            }

            int cellcount = row * column;
            if (cellItem != null)
            {
                for (int i = 0; i < cellcount; i++)
                {
                    GameObject item = GameObject.Instantiate(cellItem);
                    item.transform.SetParent(pageitem.transform, false);
                }
            }

        }

        public LTGridPageItem getPageItem(LTGridPageItem.PagePosition pos)
        {
            for (int i = 0; i < pageItem.Length; i++)
            {
                if (pageItem[i] != null
                    && pageItem[i].order == pos)
                {
                    return pageItem[i];
                }

            }
            return null;
        }

        public int GetItemIndex(GameObject item)
        {
            Transform transform = centerPage.gameObject.transform;
            int i = 0;
            foreach (Transform child in transform)
            {
                if (item.transform == child)
                {
                    int cellcount = row * column;
                    cellcount = pageIndex * cellcount;
                    return cellcount + i;
                }
                i++;
            }
            return -1;
        }

        public void ClearAll()
        {
            for (int i = 0; i < pageItem.Length; i++)
            {
                pageItem[i].order = (LTGridPageItem.PagePosition)i;
                ClearPage(pageItem[i]);
            }
        }

        public void RefreshGridPackage(bool reset, int startindex = 0)
        {
            if (reset)
            {
                pageIndex = startindex;
                Initialize();
            }
            else
            {

                if (_adapter != null)
                {
                    dataCount = _adapter.getCount();
                }
                else
                {
                    ADebug.Log("LTGridPackage Must setDataAdapter");
                }

                int cellcount = row * column;
                cellSize.x = (packageSize.x - spaceSize.x * (column - 1)) / column;
                cellSize.y = (packageSize.y - spaceSize.y * (row - 1)) / row;
                pageCount = (int)Math.Ceiling((float)dataCount / cellcount);

                RectTransform gridPackageRect = transform.GetComponent<RectTransform>();
                gridPackageRect.sizeDelta = packageSize;

                scrollZeroOffest = packageSize.x * (pageCount - 1) * 0.5f;
                gridZeroOffest = packageSize.x * (1 - pageCount) * 0.5f;

                if (scrollView != null)
                {
                    RectTransform recttrans = scrollView.GetComponent<RectTransform>();
                    recttrans.sizeDelta = new Vector2(packageSize.x * pageCount, packageSize.y);
                    float soffestx = scrollZeroOffest - pageIndex * packageSize.x;
                    recttrans.transform.localPosition = new Vector3(soffestx, 0, 0);
                }

                Vector3 scrollpos = scrollView.transform.localPosition;
                Vector3 frontdpos = scrollpos;
                frontdpos.x = gridZeroOffest + (pageIndex - 1) * packageSize.x;
                Vector3 centerpos = scrollpos;
                centerpos.x = gridZeroOffest + pageIndex * packageSize.x;
                Vector3 behindpos = scrollpos;
                behindpos.x = gridZeroOffest + (pageIndex + 1) * packageSize.x;

                frontPage.transform.localPosition = frontdpos;
                centerPage.transform.localPosition = centerpos;
                behindPage.transform.localPosition = behindpos;

                UpdateBounds();

                InvalidPageData(InvalidDataType.InvalidDataType_All);
            }
        }

        public void FrontPage()
        {
            if (pageIndex > 0)
            {
                animatePage = true;
                autoPageFront = true;
            }

        }

        public void NextPage()
        {
            if (pageIndex < pageCount - 1)
            {
                animatePage = true;
                autoPageFront = false;
            }
        }

        protected void Initialize()
        {
            pageIndex = startIndex;

            if (_adapter != null)
            {
                _adapter.gridPackage = this;
                _adapter.initialize();
                dataCount = _adapter.getCount();
            }
            else
            {
                ADebug.Log("LTGridPackage Must setDataAdapter");
            }

            int cellcount = row * column;
            cellSize.x = (packageSize.x - spaceSize.x * (column - 1)) / column;
            cellSize.y = (packageSize.y - spaceSize.y * (row - 1)) / row;
            pageCount = (int)Math.Ceiling((float)dataCount / cellcount);

            RectTransform gridPackageRect = transform.GetComponent<RectTransform>();
            gridPackageRect.sizeDelta = packageSize;

            scrollZeroOffest = packageSize.x * (pageCount - 1) * 0.5f;
            gridZeroOffest = packageSize.x * (1 - pageCount) * 0.5f;

            if (scrollView != null)
            {
                RectTransform recttrans = scrollView.GetComponent<RectTransform>();
                recttrans.sizeDelta = new Vector2(packageSize.x * pageCount, packageSize.y);
                recttrans.transform.localPosition = new Vector3(scrollZeroOffest, 0, 0);
            }

            for (int i = 0; i < pageItem.Length; i++)
            {
                RectTransform recttrans = pageItem[i].GetComponent<RectTransform>();
                GridLayoutGroup grid = pageItem[i].GetComponent<GridLayoutGroup>();
                pageItem[i].order = (LTGridPageItem.PagePosition)i;
                pageItem[i].transform.localPosition = new Vector3(gridZeroOffest + (i - 1) * packageSize.x, 0, 0);
                recttrans.sizeDelta = packageSize;
                grid.cellSize = cellSize;
                grid.spacing = spaceSize;

            }


            ClearAll();

            InvalidPageView();

            UpdateBounds();

            InvalidPageData(InvalidDataType.InvalidDataType_All);

            if (OnPageChange != null)
            {
                OnPageChange(centerPage.gameObject, pageIndex);
            }
        }

        protected void InvalidAdapter()
        {
            if (_adapter != null)
            {
                dataCount = _adapter.getCount();
            }
            else
            {
                ADebug.Log("LTGridPackage Must setDataAdapter");
            }

            int cellcount = row * column;
            cellSize.x = (packageSize.x - spaceSize.x * (column - 1)) / column;
            cellSize.y = (packageSize.y - spaceSize.y * (row - 1)) / row;
            pageCount = (int)Math.Ceiling((float)dataCount / cellcount);

        }

        protected void ClearPage(LTGridPageItem pageitem)
        {
            for (int j = 0; j < pageitem.transform.childCount; j++)
            {
                Transform child = pageitem.transform.GetChild(j);
                GameObject.Destroy(child.gameObject);
            }
        }


        protected void FillItem(LTGridPageItem pageitem, int pageindex)
        {
            int cellcount = row * column;
            int dataSIndex = pageindex * cellcount;
            int dataEIndex = 0;
            if (pageindex < pageCount - 1)
            {
                dataEIndex = (pageindex + 1) * cellcount;
            }
            else if (pageindex == pageCount - 1)
            {
                dataEIndex = dataCount;
            }

            if (cellItem != null)
            {
                for (int i = dataSIndex; i < dataEIndex; i++)
                {
                    GameObject item = GameObject.Instantiate(cellItem);
                    RectTransform recttrans = item.GetComponent<RectTransform>();
                    recttrans.sizeDelta = cellSize;
                    item.name += i.ToString();
                    item.transform.SetParent(pageitem.transform, false);
                    if (_adapter != null)
                    {
                        _adapter.FillItemData(item, i);
                    }
                    else
                    {
                        ADebug.Log("LTGridPackage Must setDataAdapter");
                    }
                }
            }
            else
            {
                ADebug.Log("LTGridPackage Must Set a cellItem");
            }

            GridLayoutGroup glayout = pageitem.gameObject.GetComponent<GridLayoutGroup>();
            glayout.CalculateLayoutInputHorizontal();
            glayout.CalculateLayoutInputVertical();
        }



        protected void InvalidPageData(InvalidDataType type)
        {
            int frontindex = pageIndex - 1;
            int curindex = pageIndex;
            int behindindex = pageIndex + 1;

            if (type == InvalidDataType.InvalidDataType_FrontPage)
            {
                if (frontindex >= 0)
                {
                    ClearPage(frontPage);
                    FillItem(frontPage, frontindex);
                }
                else
                {
                    ClearPage(frontPage);
                }
            }
            else if (type == InvalidDataType.InvalidDataType_NextPage)
            {
                if (behindindex < pageCount)
                {
                    ClearPage(behindPage);
                    FillItem(behindPage, behindindex);
                }
                else
                {
                    ClearPage(behindPage);
                }
            }
            else
            {
                if (frontindex >= 0)
                {
                    ClearPage(frontPage);
                    FillItem(frontPage, frontindex);
                }
                else
                {
                    ClearPage(frontPage);
                }

                ClearPage(centerPage);
                FillItem(centerPage, curindex);

                if (behindindex < pageCount)
                {
                    ClearPage(behindPage);
                    FillItem(behindPage, behindindex);
                }
                else
                {
                    ClearPage(behindPage);
                }
            }


        }
        protected void AnimatePageView()
        {
            Vector3 scrollpos = scrollView.transform.localPosition;
            Vector3 targetpos = scrollpos;
            int dir = autoPageFront ? 1 : -1;
            if (!autoPageFront)
            {
                targetpos.x = scrollZeroOffest - (pageIndex + 1) * packageSize.x;
            }
            else
            {
                targetpos.x = scrollZeroOffest - (pageIndex - 1) * packageSize.x;
            }

            Vector3 frontdpos = scrollpos;
            frontdpos.x = gridZeroOffest + (pageIndex - dir - 1) * packageSize.x;
            Vector3 centerpos = scrollpos;
            centerpos.x = gridZeroOffest + (pageIndex - dir) * packageSize.x;
            Vector3 behindpos = scrollpos;
            behindpos.x = gridZeroOffest + (pageIndex - dir + 1) * packageSize.x;

            aniTimeIndex += Time.deltaTime;
            if (aniTimeIndex >= animationTime)
            {
                //scroll page exchange animate

                scrollView.transform.localPosition = targetpos;
                aniTimeIndex = 0;
                animatePage = false;

                if (!autoPageFront)
                {
                    frontPage.transform.localPosition = behindpos;
                    centerPage.transform.localPosition = frontdpos;
                    behindPage.transform.localPosition = centerpos;

                    PageExchage(autoPageFront);
                }
                else
                {
                    frontPage.transform.localPosition = centerpos;
                    centerPage.transform.localPosition = behindpos;
                    behindPage.transform.localPosition = frontdpos;

                    PageExchage(autoPageFront);
                }
            }
            else
            {
                //scroll back animate
                float t = aniTimeIndex / animationTime;
                scrollView.transform.localPosition = Vector3.Lerp(scrollView.transform.localPosition, targetpos, t);
            }
        }

        protected void AnimateScrollView()
        {
            float x = scrollView.transform.localPosition.x - scrollZeroOffest + pageIndex * packageSize.x;

            if (Math.Abs(x) > packageSize.x * 0.5)
            {
                if (x < 0)
                {
                    if (pageIndex < pageCount - 1)
                    {
                        PageAnimate(false);
                    }
                    else
                    {
                        animateScroll = false;
                    }
                }
                else
                {
                    if (pageIndex > 0)
                    {
                        PageAnimate(true);
                    }
                    else
                    {
                        animateScroll = false;
                    }

                }
            }
            else
            {
                BackAnimation();
            }

        }

        //front true previous page otherwise next page
        protected void PageAnimate(bool front)
        {
            Vector3 scrollpos = scrollView.transform.localPosition;
            Vector3 targetpos = scrollpos;
            int dir = front ? 1 : -1;
            if (!front)
            {
                targetpos.x = scrollZeroOffest - (pageIndex + 1) * packageSize.x;
            }
            else
            {
                targetpos.x = scrollZeroOffest - (pageIndex - 1) * packageSize.x;
            }

            Vector3 frontdpos = scrollpos;
            frontdpos.x = gridZeroOffest + (pageIndex - dir - 1) * packageSize.x;
            Vector3 centerpos = scrollpos;
            centerpos.x = gridZeroOffest + (pageIndex - dir) * packageSize.x;
            Vector3 behindpos = scrollpos;
            behindpos.x = gridZeroOffest + (pageIndex - dir + 1) * packageSize.x;

            aniTimeIndex += Time.deltaTime;
            if (aniTimeIndex >= animationTime)
            {
                //scroll page exchange animate

                scrollView.transform.localPosition = targetpos;
                aniTimeIndex = 0;
                animateScroll = false;

                if (!front)
                {
                    frontPage.transform.localPosition = behindpos;
                    centerPage.transform.localPosition = frontdpos;
                    behindPage.transform.localPosition = centerpos;

                    PageExchage(front);
                }
                else
                {
                    frontPage.transform.localPosition = centerpos;
                    centerPage.transform.localPosition = behindpos;
                    behindPage.transform.localPosition = frontdpos;

                    PageExchage(front);
                }
            }
            else
            {
                //scroll back animate
                float t = aniTimeIndex / animationTime;
                scrollView.transform.localPosition = Vector3.Lerp(scrollView.transform.localPosition, targetpos, t);
            }

        }

        protected void PageExchage(bool front)
        {
            if (front)
            {
                if (pageIndex > 0)
                {
                    behindPage.order = LTGridPageItem.PagePosition.Pos_Front;
                    frontPage.order = LTGridPageItem.PagePosition.Pos_Center;
                    centerPage.order = LTGridPageItem.PagePosition.Pos_Behind;

                    pageIndex--;
                }
            }
            else
            {
                if (pageIndex < pageCount)
                {
                    centerPage.order = LTGridPageItem.PagePosition.Pos_Front;
                    behindPage.order = LTGridPageItem.PagePosition.Pos_Center;
                    frontPage.order = LTGridPageItem.PagePosition.Pos_Behind;

                    pageIndex++;
                }
            }

            InvalidPageView();

            if (front)
            {
                InvalidPageData(InvalidDataType.InvalidDataType_FrontPage);
            }
            else
            {
                InvalidPageData(InvalidDataType.InvalidDataType_NextPage);
            }

            UpdateBounds();

            if (OnPageChange != null)
            {
                OnPageChange(centerPage.gameObject, pageIndex);
            }

            UnityEngine.Debug.Log("PageIndex:" + pageIndex);

        }

        protected void BackAnimation()
        {

            Vector3 scrollpos = scrollView.transform.localPosition;
            Vector3 targetpos = scrollpos;
            targetpos.x = scrollZeroOffest - pageIndex * packageSize.x;

            aniTimeIndex += Time.deltaTime;
            if (aniTimeIndex >= animationTime)
            {
                scrollView.transform.localPosition = targetpos;
                aniTimeIndex = 0;
                animateScroll = false;
            }
            else
            {
                float t = aniTimeIndex / animationTime;
                scrollView.transform.localPosition = Vector3.Lerp(scrollView.transform.localPosition, targetpos, t);
            }

        }

        protected void CheckAnimateBounds()
        {
            Transform tf = scrollView.transform;
            float sx = tf.localPosition.x;
            //float sy = tf.localPosition.y;
            //float sz = tf.localPosition.z;
            if (sx < behindBounds.x)
            {
                scrollView.transform.localPosition = behindBounds;
            }

            if (sx > frontBounds.x)
            {
                scrollView.transform.localPosition = frontBounds;
            }
        }

        protected void InvalidPageView()
        {
            frontPage = getPageItem(LTGridPageItem.PagePosition.Pos_Front);
            centerPage = getPageItem(LTGridPageItem.PagePosition.Pos_Center);
            behindPage = getPageItem(LTGridPageItem.PagePosition.Pos_Behind);
        }

        protected void UpdateBounds()
        {
            Transform tf = scrollView.transform;
            //float sx = tf.localPosition.x;
            float sy = tf.localPosition.y;
            float sz = tf.localPosition.z;

            float frontPosX = scrollZeroOffest - (pageIndex - 1) * packageSize.x;
            float behindPosX = scrollZeroOffest - (pageIndex + 1) * packageSize.x;

            behindBounds = new Vector3(behindPosX, sy, sz);
            frontBounds = new Vector3(frontPosX, sy, sz);

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            animatePage = false;
            animateScroll = true;
            aniTimeIndex = 0;
        }
    }
}





