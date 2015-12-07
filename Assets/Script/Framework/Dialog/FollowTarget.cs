using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

    public Transform target;

    public float UIOffset;

    private Transform transformSelf;

    private Vector3 position;

    private static Camera UICamera;

    public float DepthSortZ = 1f;

    private void LateUpdate()
    {
        Vector3 vector  = this.target.position + new Vector3(0f, this.UIOffset, 0f);
        this.position   = Camera.main.WorldToViewportPoint(vector);
        this.position   = UICamera.ViewportToWorldPoint(this.position);
        this.position.z = DepthSortZ;
        this.transformSelf.position = this.position;
    }    

    private void Start()
    {
        UICamera                    = GameObject.Find("UICamera").GetComponent<Camera>();
        this.transformSelf          = base.transform;
        this.transformSelf.parent   = UICamera.transform;

    }
}
