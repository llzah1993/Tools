using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;

public class UIDooberTest : MonoBehaviour {
    public GameObject go;
    public GameObject go2;
    public Transform destination;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Doober()
    {
        Doober doober1 = AUIManager.instance.CreateDooberForResource("guanqia_star1", transform, go2.transform, 0.0f).GetComponent<Doober>();
        doober1.collectPath = CollectPath.arc_up;
        //doober1.DooberCollected += dooberCollectedcallback;
        Doober doober2 = AUIManager.instance.CreateDooberForResource("guanqia_star1", go.transform, destination, 0.0f, 0).GetComponent<Doober>();
        doober2.collectPath = CollectPath.arc_down;
        doober2.dooberDropped += dooberDispersedcallback;
        doober2.dooberCollected += dooberCollectedcallback;
        //UIDooberManager.instance.CreateDooberForResource("guanqia_star1", go.transform, destination, 0.0f, 1);
        //UIDooberManager.instance.CreateDooberForResource("guanqia_star1", go.transform, destination, 0.0f, 2);
        //transform.gameObject.layer
    }

    private void dooberCollectedcallback()
    {
        Debug.Log("collected!" + Time.time);
    }

    private void dooberDispersedcallback()
    {
        Debug.Log("dispersed!" + Time.time);
    }
}
