using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Framework;

namespace Framework
{
    [RequireComponent(typeof(RectTransform))]
    public class ComboBox : MonoBehaviour
    {
        //下拉列表背景UI图片
        public Sprite Sprite_UISprite;

        public Sprite Sprite_itemSprite;
        //下拉按钮背景UI图片
        public Sprite Sprite_Background;
        //下拉列表名称
        public string listName;
        //选择改变事件
        public Action<int> OnSelectionChanged;
        //列表项点击事件
        public Action<int> OnItemSelected;
        //是否可进行交互
        [SerializeField]
        private bool
            _interactable = true;
        /// <summary>
        /// 交互性
        /// </summary>
        public bool Interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;
                var button = comboButtonRectTransform.GetComponent<Button>();
                button.interactable = _interactable;
                var image = comboImageRectTransform.GetComponent<Image>();
                image.color = image.sprite == null ? new Color(1.0f, 1.0f, 1.0f, 0.0f) : _interactable ? button.colors.normalColor : button.colors.disabledColor;
                //若未在正常运行
                if (!Application.isPlaying)
                    return;
                //若不可交互
                if (!_interactable && overlayGO.activeSelf)
                    ToggleComboBox(false);
            }
        }

        //显示的下拉列表数目
        [SerializeField]
        private int
            _itemsToDisplay = 4;
        /// <summary>
        /// 列表数目
        /// </summary>
        public int ItemsToDisplay
        {
            get
            {
                return _itemsToDisplay;
            }
            set
            {
                if (_itemsToDisplay == value)
                    return;
                _itemsToDisplay = value;
                Refresh();
            }
        }
        //是否隐藏第一个列表项目
        [SerializeField]
        private bool
            _hideFirstItem;
        /// <summary>
        /// 隐藏第一个列表项
        /// </summary>
        public bool HideFirstItem
        {
            get
            {
                return _hideFirstItem;
            }
            set
            {
                if (value)
                    scrollOffset--;
                else
                    scrollOffset++;
                _hideFirstItem = value;
                Refresh();
            }
        }
        //当前选择的列表下标
        [SerializeField]
        private int
            _selectedIndex = 0;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex == value)
                    return;
                if (value > -1 && value < Items.Length)
                {
                    _selectedIndex = value;
                    RefreshSelected();
                }
            }
        }
        //下拉列表项数组
        [SerializeField]
        private ComboBoxItem[]
            _items;

        public ComboBoxItem[] Items
        {
            get
            {
                if (_items == null)
                    _items = new ComboBoxItem[0];
                return _items;
            }
            set
            {
                _items = value;
                Refresh();
            }
        }


        private GameObject overlayGO;
        private GameObject scrollPanelGO;
        Vector2 lastScreenSize;
        private int scrollOffset;
        private float _scrollbarWidth = 20.0f;
        private Transform _canvasTransform;
        /// <summary>
        /// 获取Canvas
        /// </summary>
        private Transform canvasTransform
        {
            get
            {
                if (_canvasTransform == null)
                {
                    _canvasTransform = transform.parent;
                    if(_canvasTransform == null)
                    {
                        CameraScaler cs = GameObject.FindObjectOfType<CameraScaler>();
                        _canvasTransform = cs.transform;
                    }

                }
                return _canvasTransform;
            }
        }
        //UGUI的物体变换
        private RectTransform _rectTransform;

        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
            set
            {
                _rectTransform = value;
            }
        }
        //按钮物体变换
        private RectTransform _buttonRectTransform;

        private RectTransform buttonRectTransform
        {
            get
            {
                if (_buttonRectTransform == null)
                    _buttonRectTransform = rectTransform.Find("Button").GetComponent<RectTransform>();
                return _buttonRectTransform;
            }
            set
            {
                _buttonRectTransform = value;
            }
        }
        //下拉框物体变换
        private RectTransform _comboButtonRectTransform;

        private RectTransform comboButtonRectTransform
        {
            get
            {
                if (_comboButtonRectTransform == null)
                    _comboButtonRectTransform = buttonRectTransform.Find("ComboButton").GetComponent<RectTransform>();
                return _comboButtonRectTransform;
            }
            set
            {
                _comboButtonRectTransform = value;
            }
        }
        //下拉列表图片物体变换
        private RectTransform _comboImageRectTransform;

        private RectTransform comboImageRectTransform
        {
            get
            {
                if (_comboImageRectTransform == null)
                    _comboImageRectTransform = comboButtonRectTransform.Find("Image").GetComponent<RectTransform>();
                return _comboImageRectTransform;
            }
            set
            {
                _comboImageRectTransform = value;
            }
        }
        //下拉列表文字物体变换
        private RectTransform _comboTextRectTransform;

        private RectTransform comboTextRectTransform
        {
            get
            {
                if (_comboTextRectTransform == null)
                    _comboTextRectTransform = comboButtonRectTransform.Find("Text").GetComponent<RectTransform>();
                return _comboTextRectTransform;
            }
            set
            {
                _comboTextRectTransform = value;
            }
        }
        //下拉列表标志物体变换
        private RectTransform _comboArrowRectTransform;

        private RectTransform comboArrowRectTransform
        {
            get
            {
                if (_comboArrowRectTransform == null)
                    _comboArrowRectTransform = buttonRectTransform.Find("Arrow").GetComponent<RectTransform>();
                return _comboArrowRectTransform;
            }
            set
            {
                _comboArrowRectTransform = value;
            }
        }
        //滚动面板物体变换
        private RectTransform _scrollPanelRectTransfrom;

        private RectTransform scrollPanelRectTransfrom
        {
            get
            {
                if (_scrollPanelRectTransfrom == null)
                    _scrollPanelRectTransfrom = overlayGO.transform.Find("ScrollPanel").GetComponent<RectTransform>();
                return _scrollPanelRectTransfrom;
            }
            set
            {
                _scrollPanelRectTransfrom = value;
            }
        }
        //条目物体变换
        private RectTransform _itemsRectTransfrom;

        private RectTransform itemsRectTransfrom
        {
            get
            {
                if (_itemsRectTransfrom == null)
                    _itemsRectTransfrom = scrollPanelRectTransfrom.Find("Items").GetComponent<RectTransform>();
                return _itemsRectTransfrom;
            }
            set
            {
                _itemsRectTransfrom = value;
            }
        }
        //滚动条物体变换
        private RectTransform _scrollbarRectTransfrom;

        private RectTransform scrollbarRectTransfrom
        {
            get
            {
                if (_scrollbarRectTransfrom == null)
                    _scrollbarRectTransfrom = scrollPanelRectTransfrom.Find("Scrollbar").GetComponent<RectTransform>();
                return _scrollbarRectTransfrom;
            }
            set
            {
                _scrollbarRectTransfrom = value;
            }
        }
        //滚动区域物体变换
        private RectTransform _slidingAreaRectTransform;

        private RectTransform slidingAreaRectTransform
        {
            get
            {
                if (_slidingAreaRectTransform == null)
                    _slidingAreaRectTransform = scrollbarRectTransfrom.Find("SlidingArea").GetComponent<RectTransform>();
                return _slidingAreaRectTransform;
            }
            set
            {
                _slidingAreaRectTransform = value;
            }
        }
        //处理物体变换
        private RectTransform _handleRectTransfrom;

        private RectTransform handleRectTransfrom
        {
            get
            {
                if (_handleRectTransfrom == null)
                    _handleRectTransfrom = slidingAreaRectTransform.Find("Handle").GetComponent<RectTransform>();
                return _handleRectTransfrom;
            }
            set
            {
                _handleRectTransfrom = value;
            }
        }
        //当唤醒时
        private void Awake()
        {
            //初始化控件
            InitControl();
            //获取屏幕大小宽度
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
        //当开始运行时
        private void Start()
        {
            //设置滚动面板的父级物体
            scrollPanelGO.transform.SetParent(overlayGO.transform, true);
        }
        /// <summary>
        /// 下拉列表中条目点击事件  
        /// </summary>
        /// <param listName="index">条目的下标</param>
        public void OnItemClicked(int index)
        {
            //选项是否进行变化
            var selectionChanged = index != SelectedIndex;
            //选择条目
            SelectItem(index);
            //设置下拉列表切换选项为true
            ToggleComboBox(true);
            //若选项有变化且不是空值
            if (selectionChanged && OnSelectionChanged != null)
                OnSelectionChanged(index);
        }
        /// <summary>
        /// 选中条目
        /// </summary>
        /// <param listName="index">条目的下标</param>
        public void SelectItem(int index)
        {
            SelectedIndex = index;
            if (OnItemSelected != null)
                OnItemSelected(index);
        }
        /// <summary>
        /// 添加条目
        /// </summary>
        /// <param listName="list">多条目列表</param>
        public void AddItems(params object[] list)
        {
            var cbItems = new List<ComboBoxItem>();
            foreach (var obj in list)
            {
                if (obj is ComboBoxItem)
                {
                    var item = (ComboBoxItem)obj;
                    cbItems.Add(item);
                    continue;
                }
                if (obj is string)
                {
                    var item = new ComboBoxItem((string)obj, null, false, null);
                    cbItems.Add(item);
                    continue;
                }
                if (obj is Sprite)
                {
                    var item = new ComboBoxItem(null, (Sprite)obj, false, null);
                    cbItems.Add(item);
                    continue;
                }
                throw new Exception("只允许下拉列表条目、精灵、字符串类型可以传入");
            }
            //初始化新条目
            var newItems = new ComboBoxItem[Items.Length + cbItems.Count];
            //将新条目放置于下拉列表中第一个位置
            Items.CopyTo(newItems, 0);
            //将传入多个条目转化为数组并拷贝
            cbItems.ToArray().CopyTo(newItems, Items.Length);
            Refresh();
            Items = newItems;
        }
        /// <summary>
        /// 清除条目
        /// </summary>
        public void ClearItems()
        {
            Items = new ComboBoxItem[0];
        }
        /// <summary>
        /// 创建控件
        /// </summary>
        public void CreateControl()
        {
            //获取当前UGUI的UI变换
            rectTransform = GetComponent<RectTransform>();
            //按钮初始化
            var buttonGO = new GameObject("Button");
            buttonGO.transform.SetParent(transform, false);
            buttonRectTransform = buttonGO.AddComponent<RectTransform>();
            buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
            buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
            buttonRectTransform.anchoredPosition = Vector2.zero;
            //下拉列表初始化
            var comboButtonGO = new GameObject("ComboButton");
            comboButtonGO.transform.SetParent(buttonRectTransform, false);
            comboButtonRectTransform = comboButtonGO.AddComponent<RectTransform>();
            comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonRectTransform.sizeDelta.x);
            comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonRectTransform.sizeDelta.y);
            comboButtonRectTransform.anchoredPosition = Vector2.zero;
            //下拉列表图片按钮初始化
            var comboButtonImage = comboButtonGO.AddComponent<Image>();
            comboButtonImage.sprite = Sprite_UISprite;
            comboButtonImage.type = Image.Type.Sliced;
            var comboButtonButton = comboButtonGO.AddComponent<Button>();
            comboButtonButton.targetGraphic = comboButtonImage;
            //设置多种事件时显示颜色和效果
            var comboButtonColors = new ColorBlock();
            comboButtonColors.normalColor = new Color32(255, 255, 255, 255);
            comboButtonColors.highlightedColor = new Color32(245, 245, 245, 255);
            comboButtonColors.pressedColor = new Color32(200, 200, 200, 255);
            comboButtonColors.disabledColor = new Color32(200, 200, 200, 128);
            comboButtonColors.colorMultiplier = 1.0f;
            comboButtonColors.fadeDuration = 0.1f;
            comboButtonButton.colors = comboButtonColors;
            //下拉列表图标初始化
            var comboArrowGO = new GameObject("Arrow");
            comboArrowGO.transform.SetParent(buttonRectTransform, false);
            var comboArrowText = comboArrowGO.AddComponent<Text>();
            comboArrowText.color = new Color32(0, 0, 0, 255);
            comboArrowText.alignment = TextAnchor.MiddleCenter;
            comboArrowText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            comboArrowText.text = "▼";
            comboArrowRectTransform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
            comboArrowRectTransform.pivot = new Vector2(1.0f, 0.5f);
            comboArrowRectTransform.anchorMin = Vector2.right;
            comboArrowRectTransform.anchorMax = Vector2.one;
            comboArrowRectTransform.anchoredPosition = Vector2.zero;
            comboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.y);
            comboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, comboButtonRectTransform.sizeDelta.y);
            var comboArrowCanvasGroup = comboArrowGO.AddComponent<CanvasGroup>();
            comboArrowCanvasGroup.interactable = false;
            comboArrowCanvasGroup.blocksRaycasts = false;
            //下拉列表图片初始化
            var comboImageGO = new GameObject("Image");
            comboImageGO.transform.SetParent(comboButtonRectTransform, false);
            var comboImageImage = comboImageGO.AddComponent<Image>();
            comboImageImage.color = new Color32(255, 255, 255, 0);
            comboImageRectTransform.pivot = Vector2.up;
            comboImageRectTransform.anchorMin = Vector2.zero;
            comboImageRectTransform.anchorMax = Vector2.up;
            comboImageRectTransform.anchoredPosition = new Vector2(4.0f, -4.0f);
            comboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.y - 8.0f);
            comboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, comboButtonRectTransform.sizeDelta.y - 8.0f);
            //下拉列表文字初始化
            var comboTextGO = new GameObject("Text");
            comboTextGO.transform.SetParent(comboButtonRectTransform, false);
            var comboTextText = comboTextGO.AddComponent<Text>();
            comboTextText.color = new Color32(0, 0, 0, 255);
            comboTextText.alignment = TextAnchor.MiddleLeft;
            comboTextText.lineSpacing = 1.2f;
            comboTextText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            comboTextRectTransform.pivot = Vector2.up;
            comboTextRectTransform.anchorMin = Vector2.zero;
            comboTextRectTransform.anchorMax = Vector2.one;
            comboTextRectTransform.anchoredPosition = new Vector2(10.0f, 0.0f);
            comboTextRectTransform.offsetMax = new Vector2(4.0f, 0.0f);
            comboTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, comboButtonRectTransform.sizeDelta.y);
        }
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControl()
        {
            var cbi = transform.Find("Button/ComboButton/Image");
            var cbt = transform.Find("Button/ComboButton/Text");
            var cba = transform.Find("Button/Arrow");
            //若不存在，则将当前物体下所有子物体删除
            if (cbi == null || cbt == null || cba == null)
            {
                foreach (Transform child in transform)
                    Destroy(child);
                //创建控件
                CreateControl();
            }
            //添加下拉列表按钮事件监听
            comboButtonRectTransform.GetComponent<Button>().onClick.AddListener(() => {
                ToggleComboBox(false); });
            //设置下拉列表的高度
            var dropdownHeight = comboButtonRectTransform.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));
            //将下拉列表的具体内容隐藏起来并将下拉列表内容生成
            overlayGO = new GameObject("CBOverlay");
            overlayGO.SetActive(false);
            var overlayImage = overlayGO.AddComponent<Image>();
            overlayImage.color = new Color32(0, 0, 0, 0);
            overlayGO.transform.SetParent(canvasTransform, false);
            var overlayRectTransform = overlayGO.GetComponent<RectTransform>();
            overlayRectTransform.anchorMin = Vector2.zero;
            overlayRectTransform.anchorMax = Vector2.one;
            overlayRectTransform.offsetMin = Vector2.zero;
            overlayRectTransform.offsetMax = Vector2.zero;
            var overlayButton = overlayGO.AddComponent<Button>();
            overlayButton.targetGraphic = overlayImage;
            overlayButton.onClick.AddListener(() => {
                ToggleComboBox(false); });

            scrollPanelGO = new GameObject("ScrollPanel");
            var scrollPanelImage = scrollPanelGO.AddComponent<Image>();
            scrollPanelImage.sprite = Sprite_UISprite;
            scrollPanelImage.type = Image.Type.Sliced;
            scrollPanelGO.transform.SetParent(overlayGO.transform, false);
            scrollPanelRectTransfrom.pivot = Vector2.zero;
            scrollPanelRectTransfrom.anchorMin = Vector2.zero;
            scrollPanelRectTransfrom.anchorMax = Vector2.one;
            scrollPanelGO.transform.SetParent(transform, false);
            scrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -rectTransform.sizeDelta.y * _itemsToDisplay);

            scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.x);
            scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            var scrollPanelScrollRect = scrollPanelGO.AddComponent<ScrollRect>();
            scrollPanelScrollRect.horizontal = false;
            scrollPanelScrollRect.elasticity = 0.0f;
            scrollPanelScrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollPanelScrollRect.inertia = false;
            scrollPanelScrollRect.scrollSensitivity = comboButtonRectTransform.sizeDelta.y;
            scrollPanelGO.AddComponent<Mask>();

            var scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > _itemsToDisplay ? _scrollbarWidth : 0.0f;

            var itemsGO = new GameObject("Items");
            itemsGO.transform.SetParent(scrollPanelGO.transform, false);
            itemsRectTransfrom = itemsGO.AddComponent<RectTransform>();
            itemsRectTransfrom.pivot = Vector2.up;
            itemsRectTransfrom.anchorMin = Vector2.up;
            itemsRectTransfrom.anchorMax = Vector2.one;
            itemsRectTransfrom.anchoredPosition = Vector2.right;
            itemsRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollPanelRectTransfrom.sizeDelta.x - scrollbarWidth);
            var itemsContentSizeFitter = itemsGO.AddComponent<ContentSizeFitter>();
            itemsContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            itemsContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var itemsGridLayoutGroup = itemsGO.AddComponent<GridLayoutGroup>();
            itemsGridLayoutGroup.cellSize = new Vector2(comboButtonRectTransform.sizeDelta.x - scrollbarWidth, comboButtonRectTransform.sizeDelta.y);
            itemsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            itemsGridLayoutGroup.constraintCount = 1;
            scrollPanelScrollRect.content = itemsRectTransfrom;

            var scrollbarGO = new GameObject("Scrollbar");
            scrollbarGO.transform.SetParent(scrollPanelGO.transform, false);
            var scrollbarImage = scrollbarGO.AddComponent<Image>();
            scrollbarImage.sprite = Sprite_Background;
            scrollbarImage.type = Image.Type.Sliced;
            var scrollbarScrollbar = scrollbarGO.AddComponent<Scrollbar>();
            var scrollbarColors = new ColorBlock();
            scrollbarColors.normalColor = new Color32(128, 128, 128, 128);
            scrollbarColors.highlightedColor = new Color32(128, 128, 128, 178);
            scrollbarColors.pressedColor = new Color32(88, 88, 88, 178);
            scrollbarColors.disabledColor = new Color32(64, 64, 64, 128);
            scrollbarColors.colorMultiplier = 2.0f;
            scrollbarColors.fadeDuration = 0.1f;
            scrollbarScrollbar.colors = scrollbarColors;
            scrollPanelScrollRect.verticalScrollbar = scrollbarScrollbar;
            scrollbarScrollbar.direction = Scrollbar.Direction.BottomToTop;
            scrollbarRectTransfrom.pivot = Vector2.one;
            scrollbarRectTransfrom.anchorMin = Vector2.one;
            scrollbarRectTransfrom.anchorMax = Vector2.one;
            scrollbarRectTransfrom.anchoredPosition = Vector2.zero;
            scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);

            var slidingAreaGO = new GameObject("SlidingArea");
            slidingAreaGO.transform.SetParent(scrollbarGO.transform, false);
            slidingAreaRectTransform = slidingAreaGO.AddComponent<RectTransform>();
            slidingAreaRectTransform.anchoredPosition = Vector2.zero;
            slidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            slidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - scrollbarRectTransfrom.sizeDelta.x);

            var handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(slidingAreaGO.transform, false);
            var handleImage = handleGO.AddComponent<Image>();
            handleImage.sprite = Sprite_UISprite;
            handleImage.type = Image.Type.Sliced;
            handleImage.color = new Color32(255, 255, 255, 150);
            scrollbarScrollbar.targetGraphic = handleImage;
            scrollbarScrollbar.handleRect = handleRectTransfrom;
            handleRectTransfrom.pivot = new Vector2(0.5f, 0.5f);
            handleRectTransfrom.anchorMin = new Vector2(0.5f, 0.5f);
            handleRectTransfrom.anchorMax = new Vector2(0.5f, 0.5f);
            handleRectTransfrom.anchoredPosition = Vector2.zero;
            handleRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            handleRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollbarWidth);

            Interactable = Interactable;
            //若条目不存在
            if (Items.Length < 1)
                return;
            Refresh();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        private void Refresh()
        {
            //获取UGUI的网格状布局
            var itemsGridLayoutGroup = itemsRectTransfrom.GetComponent<GridLayoutGroup>();
            //条目长度
            var itemsLength = Items.Length - (HideFirstItem ? 1 : 0);
            //下拉列表高度
            var dropdownHeight = comboButtonRectTransform.sizeDelta.y * Mathf.Min(_itemsToDisplay, itemsLength);
            //滚动条宽度
            var scrollbarWidth = itemsLength > ItemsToDisplay ? _scrollbarWidth : 0.0f;
            scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            slidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - scrollbarRectTransfrom.sizeDelta.x);
            itemsRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollPanelRectTransfrom.sizeDelta.x - scrollbarWidth);
            itemsGridLayoutGroup.cellSize = new Vector2(comboButtonRectTransform.sizeDelta.x - scrollbarWidth, comboButtonRectTransform.sizeDelta.y);
            //将条目列表中所有子物体立刻删除
            for (var i = itemsRectTransfrom.childCount - 1; i > -1; i--)
                DestroyImmediate(itemsRectTransfrom.GetChild(0).gameObject);
            //重新生成列表
            for (var i = 0; i < Items.Length; i++)
            {
                if (HideFirstItem && i == 0)
                    continue;
                var item = Items [i];
                item.OnUpdate = Refresh;
                var itemTransform = Instantiate(comboButtonRectTransform) as Transform;
                itemTransform.SetParent(itemsRectTransfrom, false);
                itemTransform.GetComponent<Image>().sprite = Sprite_itemSprite;
                var itemText = itemTransform.Find("Text").GetComponent<Text>();
                itemText.text = item.Caption;
                if (item.IsDisabled)
                    itemText.color = new Color32(174, 174, 174, 255);
                var itemImage = itemTransform.Find("Image").GetComponent<Image>();
                itemImage.sprite = item.Image;
                itemImage.color = item.Image == null ? new Color32(255, 255, 255, 0) : item.IsDisabled ? new Color32(255, 255, 255, 147) : new Color32(255, 255, 255, 255);
                var itemButton = itemTransform.GetComponent<Button>();
                itemButton.interactable = !item.IsDisabled;
                var index = i;
                itemButton.onClick.AddListener(
                    delegate()
                {
                    OnItemClicked(index);
                    if (item.OnSelect != null)
                        item.OnSelect();
                }
                );
            }
            //刷新选择物体
            RefreshSelected();
            //更新下拉列表图片
            UpdateComboBoxImages();
            //修正滚动部分偏移
            FixScrollOffset();
            //更新处理面板
            UpdateHandle();
        }
        /// <summary>
        /// 刷新选项
        /// </summary>
        public void RefreshSelected()
        {
            var comboButtonImage = comboImageRectTransform.GetComponent<Image>();
            var item = SelectedIndex > -1 && SelectedIndex < Items.Length ? Items [SelectedIndex] : null;
            var includeImage = item != null && item.Image != null;
            comboButtonImage.sprite = includeImage ? item.Image : null;
            var comboButtonButton = comboButtonRectTransform.GetComponent<Button>();
            comboButtonImage.color = includeImage ? (Interactable ? comboButtonButton.colors.normalColor : comboButtonButton.colors.disabledColor) : new Color(1.0f, 1.0f, 1.0f, 0);
            UpdateComboBoxImage(comboButtonRectTransform, includeImage);
            comboTextRectTransform.GetComponent<Text>().text = item != null ? item.Caption : "";
            if (!Application.isPlaying)
                return;
            var i = 0;
            foreach (Transform child in itemsRectTransfrom)
            {
                comboButtonImage = child.GetComponent<Image>();
                comboButtonImage.color = SelectedIndex == i + (HideFirstItem ? 1 : 0) ? comboButtonButton.colors.highlightedColor : comboButtonButton.colors.normalColor;
                i++;
            }
        }
        /// <summary>
        /// 更新下拉列表图片
        /// </summary>
        private void UpdateComboBoxImages()
        {
            var includeImages = false;
            foreach (var item in Items)
            {
                if (item.Image != null)
                {
                    includeImages = true;
                    break;
                }
            }
            foreach (Transform child in itemsRectTransfrom)
                UpdateComboBoxImage(child, includeImages);
        }
        /// <summary>
        /// 更新下拉列表图片
        /// </summary>
        /// <param listName="comboButton">下拉列表按钮</param>
        /// <param listName="includeImage">是否含有图片</param>
        private void UpdateComboBoxImage(Transform comboButton, bool includeImage)
        {
            comboButton.Find("Text").GetComponent<RectTransform>().offsetMin = Vector2.right * (includeImage ? comboImageRectTransform.rect.width + 8.0f : 10.0f);
        }
        /// <summary>
        /// 修正滚动面板偏移
        /// </summary>
        private void FixScrollOffset()
        {
            var selectedIndex = SelectedIndex + (HideFirstItem ? 1 : 0);
            if (selectedIndex < scrollOffset)
                scrollOffset = selectedIndex;
            else
                if (selectedIndex > scrollOffset + ItemsToDisplay - 1)
                scrollOffset = selectedIndex - ItemsToDisplay + 1;
            var itemsCount = Items.Length - (HideFirstItem ? 1 : 0);
            if (scrollOffset > itemsCount - ItemsToDisplay)
                scrollOffset = itemsCount - ItemsToDisplay;
            if (scrollOffset < 0)
                scrollOffset = 0;
            itemsRectTransfrom.anchoredPosition = new Vector2(0.0f, scrollOffset * rectTransform.sizeDelta.y);
        }
        /// <summary>
        /// 切换下拉列表选项
        /// </summary>
        /// <param listName="directClick"></param>
        private void ToggleComboBox(bool directClick)
        {
            overlayGO.SetActive(!overlayGO.activeSelf);
            if (overlayGO.activeSelf)
            {
                var itemsToDisplay = Mathf.Min(_itemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));

                scrollPanelRectTransfrom.SetParent(transform, false);
                var dropdownHeight = comboButtonRectTransform.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));
                scrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -rectTransform.sizeDelta.y * itemsToDisplay);
                scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.x);
                scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
                scrollPanelRectTransfrom.SetParent(overlayGO.GetComponent<RectTransform>(), true);

                FixScrollOffset();
            } else
            {
                if (directClick)
                    scrollOffset = (int)Mathf.Round(itemsRectTransfrom.anchoredPosition.y / rectTransform.sizeDelta.y);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            
            if (this.lastScreenSize != screenSize)
            {
                this.lastScreenSize = screenSize;
                if(overlayGO.activeSelf)
                    UpdateGraphics();
            }
        }
        /// <summary>
        /// 更新绘制
        /// </summary>
        public void UpdateGraphics()
        {
            //更新处理面板
            UpdateHandle();

      /*      if (rectTransform.sizeDelta != buttonRectTransform.sizeDelta && buttonRectTransform.sizeDelta == comboButtonRectTransform.sizeDelta)
            {
                buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
                buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
                comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
                comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
                comboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.y);
                comboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboImageRectTransform.rect.height);
                comboTextRectTransform.offsetMax = new Vector2(4.0f, 0.0f);
                if (overlayGO == null)
                    return;
                scrollPanelRectTransfrom.SetParent(transform, true);
                scrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -rectTransform.sizeDelta.y * _itemsToDisplay);
                var overlayRectTransform = overlayGO.GetComponent<RectTransform>();
                overlayRectTransform.SetParent(canvasTransform, false);
                overlayRectTransform.offsetMin = Vector2.zero;
                overlayRectTransform.offsetMax = Vector2.zero;
                scrollPanelRectTransfrom.SetParent(overlayRectTransform, true);
                scrollPanelRectTransfrom.GetComponent<ScrollRect>().scrollSensitivity = comboButtonRectTransform.sizeDelta.y;
                UpdateComboBoxImage(comboButtonRectTransform, Items [SelectedIndex].Image != null);
                Refresh();
            } else
            {
                if (overlayGO == null)
                    return;
                scrollPanelRectTransfrom.SetParent(transform, false);
                var dropdownHeight = comboButtonRectTransform.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));
                scrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -rectTransform.sizeDelta.y * _itemsToDisplay);
                scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.x);
                scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
                scrollPanelRectTransfrom.SetParent(overlayGO.GetComponent<RectTransform>(), true);
            }*/
        }
        /// <summary>
        /// 更新处理面板
        /// </summary>
        private void UpdateHandle()
        {
            if (overlayGO == null)
                return;
            var scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > ItemsToDisplay ? _scrollbarWidth : 0.0f;
            handleRectTransfrom.offsetMin = -scrollbarWidth / 2 * Vector2.one;
            handleRectTransfrom.offsetMax = scrollbarWidth / 2 * Vector2.one;
        }
    }
}