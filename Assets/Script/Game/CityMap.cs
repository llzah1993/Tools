using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CityMap : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetMouseButtonDown(0) || (Input.touchCount >0 && Input.GetTouch(0).phase == TouchPhase.Began))
//		{
//			#if IPHONE || ANDROID
//				if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
//			#else
//				if (EventSystem.current.IsPointerOverGameObject())
//			#endif
//				{
//					Debug.Log("当前触摸在UI上");
//				}
//				else 
//				{
//					Debug.Log("当前没有触摸在UI上");
//				}
//		}
	}
	
	public void ImageClick()
	{
		ADebug.Log("what the fuck");
	}
}
