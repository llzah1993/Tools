using UnityEngine;
using UnityEngine.UI;
using System;

namespace Framework
{
    /// <summary>
    /// 下拉列表条目
    /// </summary>
	[Serializable]
	public class ComboBoxItem
	{
        /// <summary>
        /// 显示信息
        /// </summary>
		[SerializeField]
		private string _caption;
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
				if (OnUpdate != null)
					OnUpdate();
			}
		}
        /// <summary>
        /// 列表左侧图片
        /// </summary>
		[SerializeField]
		private Sprite _image;
		public Sprite Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
				if (OnUpdate != null)
					OnUpdate();
			}
		}
        /// <summary>
        /// 是否可用
        /// </summary>
		[SerializeField]
		private bool _isDisabled;
		public bool IsDisabled
		{
			get
			{
				return _isDisabled;
			}
			set
			{
				_isDisabled = value;
				if (OnUpdate != null)
					OnUpdate();
			}
		}

		public Action OnSelect;

		internal Action OnUpdate;

        //构造函数
		public ComboBoxItem(string caption)
		{
			_caption = caption;
		}

		public ComboBoxItem(Sprite image)
		{
			_image = image;
		}

		public ComboBoxItem(string caption, bool disabled)
		{
			_caption = caption;
			_isDisabled = disabled;
		}

		public ComboBoxItem(Sprite image, bool disabled)
		{
			_image = image;
			_isDisabled = disabled;
		}

		public ComboBoxItem(string caption, Sprite image, bool disabled)
		{
			_caption = caption;
			_image = image;
			_isDisabled = disabled;
		}

		public ComboBoxItem(string caption, Sprite image, bool disabled, Action onSelect)
		{
			_caption = caption;
			_image = image;
			_isDisabled = disabled;
			OnSelect = onSelect;
		}

		public ComboBoxItem(string caption, Sprite image, Action onSelect)
		{
			_caption = caption;
			_image = image;
			OnSelect = onSelect;
		}

		public ComboBoxItem(string caption, Action onSelect)
		{
			_caption = caption;
			OnSelect = onSelect;
		}

		public ComboBoxItem(Sprite image, Action onSelect)
		{
			_image = image;
			OnSelect = onSelect;
		}
	}
}