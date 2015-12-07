using UnityEngine;
using System.Collections;

public class PositionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Debug.Log(mousePosition.x.ToString() + "," + mousePosition.y.ToString() + "," + mousePosition.z.ToString());
        Vector3 mousePosition2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePosition2.x.ToString() + "," + mousePosition2.y.ToString() + "," + mousePosition2.z.ToString());
    }
}
