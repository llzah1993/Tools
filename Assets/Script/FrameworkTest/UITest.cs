using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;

public class UITest : MonoBehaviour {


	public Dialog     prefab;
	public Dialog     dialog;
	public Dialog     tip;


	// Use this for initialization
	void Start () {
        //Vector3 screen1 = Camera.main.WorldToScreenPoint(transform.position);
        //Debug.Log(screen1.x.ToString() + "," + screen1.y.ToString() + "," + screen1.z.ToString());
        //Vector3 screen2 = AUIManager.instance.uiCamera.WorldToScreenPoint(transform.position);
        //Debug.Log(screen2.x.ToString() + "," + screen2.y.ToString() + "," + screen2.z.ToString());
    }
	
	// Update is called once per frame
	void Update () {
	
	}


	public void OpenDialog(GameObject go) 
	{
		this.dialog = AUIManager.OpenDialog(this.prefab, AUIManager.instance.uiCamera.gameObject);
	}

	public void ShowTip()
	{
		Dialog dialog  = AUIManager.ShowTip(this.tip);
		dialog.onOpend += (Dialog d) => 
		{
			ADebug.Log("show first");
		};
	}

	public void ShowQueuedTip()
	{
		AUIManager.ShowQueuedTip(this.tip, (Dialog d) => 
		{
			ADebug.Log("Create queue tip");
		});
	}
}
