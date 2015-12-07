using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Framework
{
    public enum EventTriggerType
    {
        EventType_None = -1,
        EventType_OnBeginDrag = 0,
        EventType_OnCancel = 2,
        EventType_OnDeselect,
        EventType_OnDrag,
        EventType_OnDrop,
        EventType_OnEndDrag,
        EventType_OnInitializePotentialDrag,
        EventType_OnMove,
        EventType_OnPointerClick,
        EventType_OnPointerDown,
        EventType_OnPointerEnter,
        EventType_OnPointerExit,
        EventType_OnPointerUp,
        EventType_OnScroll,
        EventType_OnSubmit,
        EventType_OnUpdateSelected,
    }

    public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);

        public VoidDelegate OnBeginDragDelegate;
        public VoidDelegate OnCancelDelegate;
        public VoidDelegate OnDeselectDelegate;
        public VoidDelegate OnDragDelegate;
        public VoidDelegate OnDropDelegate;
        public VoidDelegate OnEndDragDelegate;
        public VoidDelegate OnInitializePotentialDragDelegate;
        public VoidDelegate OnMoveDelegate;
        public VoidDelegate OnPointerClickDelegate;
        public VoidDelegate OnPointerDownDelegate;
        public VoidDelegate OnPointerEnterDelegate;
        public VoidDelegate OnPointerExitDelegate;
        public VoidDelegate OnPointerUpDelegate;
        public VoidDelegate OnScrollDelegate;
        public VoidDelegate OnSelectDelegate;
        public VoidDelegate OnSubmitDelegate;
        public VoidDelegate OnUpdateSelectedDelegate;


        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null)
            {
                listener = go.AddComponent<EventTriggerListener>();
            }
            return listener;
        }


        // Summary:
        //     See [[IBeginDragHandler.OnBeginDrag]].
        //
        // Parameters:
        //   eventData:
        public override void OnBeginDrag(PointerEventData eventData)
        {

            if (OnBeginDragDelegate != null)
            {
                OnBeginDragDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[ICancelHandler.OnCancel]].
        //
        // Parameters:
        //   eventData:
        public override void OnCancel(BaseEventData eventData)
        {
            if (OnCancelDelegate != null)
            {
                OnCancelDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IDeselectHandler.OnDeselect]].
        //
        // Parameters:
        //   eventData:
        public override void OnDeselect(BaseEventData eventData)
        {
            if (OnDeselectDelegate != null)
            {
                OnDeselectDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IDragHandler.OnDrag]].
        //
        // Parameters:
        //   eventData:
        public override void OnDrag(PointerEventData eventData)
        {
            if (OnDragDelegate != null)
            {
                OnDragDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IDropHandler.OnDrop]].
        //
        // Parameters:
        //   eventData:
        public override void OnDrop(PointerEventData eventData)
        {
            if (OnDropDelegate != null)
            {
                OnDropDelegate(gameObject);
            }
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (OnEndDragDelegate != null)
            {
                OnEndDragDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     Called by a [[BaseInputModule]] when a drag has been found but before it
        //     is valid to begin the drag.
        //
        // Parameters:
        //   eventData:
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (OnInitializePotentialDragDelegate != null)
            {
                OnInitializePotentialDragDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IMoveHandler.OnMove]].
        //
        // Parameters:
        //   eventData:
        public override void OnMove(AxisEventData eventData)
        {
            if (OnMoveDelegate != null)
            {
                OnMoveDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IPointerClickHandler.OnPointerClick]].
        //
        // Parameters:
        //   eventData:
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (OnPointerClickDelegate != null)
            {
                OnPointerClickDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IPointerDownHandler.OnPointerDown]].
        //
        // Parameters:
        //   eventData:
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (OnPointerDownDelegate != null)
            {
                OnPointerDownDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IPointerEnterHandler.OnPointerEnter]].
        //
        // Parameters:
        //   eventData:
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (OnPointerEnterDelegate != null)
            {
                OnPointerEnterDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IPointerExitHandler.OnPointerExit]].
        //
        // Parameters:
        //   eventData:
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExitDelegate != null)
            {
                OnPointerExitDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IPointerUpHandler.OnPointerUp]].
        //
        // Parameters:
        //   eventData:
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (OnPointerUpDelegate != null)
            {
                OnPointerUpDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IScrollHandler.OnScroll]].
        //
        // Parameters:
        //   eventData:
        public override void OnScroll(PointerEventData eventData)
        {
            if (OnScrollDelegate != null)
            {
                OnScrollDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[ISelectHandler.OnSelect]].
        //
        // Parameters:
        //   eventData:
        public override void OnSelect(BaseEventData eventData)
        {
            if (OnSelectDelegate != null)
            {
                OnSelectDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[ISubmitHandler.OnSubmit]].
        //
        // Parameters:
        //   eventData:
        public override void OnSubmit(BaseEventData eventData)
        {
            if (OnSubmitDelegate != null)
            {
                OnSubmitDelegate(gameObject);
            }
        }
        //
        // Summary:
        //     See [[IUpdateSelectedHandler.OnUpdateSelected]].
        //
        // Parameters:
        //   eventData:
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (OnUpdateSelectedDelegate != null)
            {
                OnUpdateSelectedDelegate(gameObject);
            }
        }

    }
}

/*
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;.
using UnityEngine.Events;
public class UIMain : MonoBehaviour {
	Button	button;
	Image image;
	void Start () 
	{
		button = transform.Find("Button").GetComponent<Button>();
		image = transform.Find("Image").GetComponent<Image>();
		EventTriggerListener.Get(button.gameObject).onClick =OnButtonClick;
		EventTriggerListener.Get(image.gameObject).onClick =OnButtonClick;
	}
 
	private void OnButtonClick(GameObject go){
		//在这里监听按钮的点击事件
		if(go == button.gameObject){
			Debug.Log ("DoSomeThings");
		}
	}
}
*/