using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace Framework 
{
	public class Dialog : MonoBehaviour {

		public enum TransitionStyle
		{
			zoom,
			slide_left,
			slide_right,
			slide_top,
			slide_bottom,
		}

		public string          dialogName;
		public int             width            = 0;
		public int             height           = 0;
		public bool            isModal          = true;
		public bool            isTouchFallThrough;
		public bool            isCloseOnFocusOutside;

		public TransitionStyle openTransition   = TransitionStyle.zoom;
		public Ease            openEase         = Ease.OutBack;
		public float           openDuration     = 0.618f;
		public int             openPosOffsetX   = 0;
		public int             openPosOffsetY   = 0;

		public TransitionStyle closeTransition  = TransitionStyle.zoom;
		public Ease            closeEase        = Ease.OutBack;
		public float           closeDuration    = 0.618f;

		public float           autoCloseTime    = 0f;
		public Button          closeButton;



		[HideInInspector]
		public GameObject     container;
		[HideInInspector]
		public GameObject     scrim;
		[HideInInspector]
		public Action<Dialog> onOpend;
		[HideInInspector]
		public Action<Dialog> onClosed;


		
		void Start()
		{
			if (this.closeButton != null) 
			{
				this.closeButton.onClick.AddListener
				(
					() => 
					{
						AUIManager.CloseDialog(this);
					}
				);
			}
		}
	}
}


