using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Framework
{



	public class MyImage : Image
	{
		public PolygonCollider2D polyCollider;

		void Awake()
		{
			this.polyCollider = this.GetComponent<PolygonCollider2D>();
		}

		override public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			Vector3 world = eventCamera.ScreenToWorldPoint(new Vector3(sp.x, sp.y, 0));
			return this.polyCollider.bounds.Contains(world);
		}
	}

}
