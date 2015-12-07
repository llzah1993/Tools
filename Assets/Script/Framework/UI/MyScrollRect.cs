using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Framework
{
	public class MyScrollRect : ScrollRect 
	{

		public override void OnBeginDrag (PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);

		}

		public override void OnDrag (PointerEventData eventData)
		{
			base.OnDrag(eventData);
		}
	}
}
