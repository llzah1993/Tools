using UnityEngine;
using System.Collections;

public class DragCamera : MonoBehaviour {

    public float dragSpeed = -10;
    public int minX = -892;
    public int maxX = 1111;
    public int minZ = -880;
    public int maxZ = 1145;

    public int bottomMargin = 80; // if you have some icons at the bottom (like an RPG game) this will help preventing the drag action at the bottom

    public float orthZoomStep = 10.0f;
    public int orthZoomMaxSize = 500;
    public int orthZoomMinSize = 300;

    //private bool orthographicView = true;
    private Vector3 dragOrigin;
    private Vector3 prevPosition;

    void Start()
    {
        //Debug.Log(Screen.height.ToString() + "," + Screen.width.ToString()+Screen.currentResolution.height.ToString()+","+Screen.currentResolution.width.ToString());
        //float designW = 1280;
        //float designH = 1920;
        //float percent = designW / designH;
        //float mCameraHalfWidth = Camera.main.orthographicSize*2 * percent;
        //float mCameraHalfHeight = Camera.main.orthographicSize*2;
        //Debug.Log(mCameraHalfHeight.ToString()+","+mCameraHalfWidth.ToString());
    }

    void Update() {
        //moveCamera();
        //zoomCamera();
        //Vector3 mousePosition = Input.mousePosition;
        //Debug.Log(mousePosition.x.ToString() + "," + mousePosition.y.ToString() + "," + mousePosition.z.ToString());
        //Vector3 mousePosition2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Debug.Log(mousePosition2.x.ToString() + "," + mousePosition2.y.ToString() + "," + mousePosition2.z.ToString());
    }

    void moveCamera() {
        if (Input.GetMouseButtonDown(0)) {
            prevPosition = dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        if (dragOrigin.y <= bottomMargin) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - prevPosition);
        prevPosition = Input.mousePosition;
        //Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        //if(move.x > 0)
        //{
        //    if(!isWithinRightBorder())
        //        move.x =0;
        //}
        //else
        //{
        //    if(!isWithinLeftBorder())
        //        move.x=0;
        //}

        //if(move.z > 0)
        //{
        //    if(!isWithinTopBorder())
        //        move.z=0;
        //}
        //else
        //{
        //    if(!isWithinBottomBorder())
        //        move.z=0;
        //}


        transform.Translate(move, Space.World);
    }

    //void zoomCamera()
    //{
    //    if(!isWithinBorders())
    //        return;

    //    // zoom out
    //    if (Input.GetAxis("Mouse ScrollWheel") <0)
    //    {
    //        if(orthographicView)
    //        {
    //            if (Camera.main.orthographicSize <=orthZoomMaxSize)
    //                Camera.main.orthographicSize += orthZoomStep;
    //        }
    //        else
    //        {
    //            if (Camera.main.fieldOfView<=150)
    //                   Camera.main.fieldOfView +=5;
    //        }
    //    }
    //    // zoom in
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0)
    //       {
    //        if(orthographicView)
    //        {
    //            if (Camera.main.orthographicSize >= orthZoomMinSize)
    //                 Camera.main.orthographicSize -= orthZoomStep;            
    //        }
    //        else
    //        {
    //            if (Camera.main.fieldOfView>2)
    //                Camera.main.fieldOfView -=5;
    //        }
    //       }
    //}

    bool isWithinBorders() {
        return (isWithinLeftBorder() && isWithinBottomBorder() && isWithinRightBorder() && isWithinTopBorder());
    }

    bool isWithinLeftBorder() {
        Vector3 currentTopLeftGlobal = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        if (currentTopLeftGlobal.x > minX)
            return true;
        else
            return false;

    }

    bool isWithinRightBorder() {
        Vector3 currentBottomRightGlobal = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        if (currentBottomRightGlobal.x < maxX)
            return true;
        else
            return false;
    }

    bool isWithinTopBorder() {
        Vector3 currentTopLeftGlobal = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        if (currentTopLeftGlobal.z < maxZ)
            return true;
        else
            return false;
    }

    bool isWithinBottomBorder() {
        Vector3 currentBottomRightGlobal = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        if (currentBottomRightGlobal.z > minZ)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 响应FingerGesture的Drag事件
    /// </summary>
    /// <param name="gesture"></param>
    void OnDrag(DragGesture gesture) {
        moveCamera2(gesture);
    }

    void moveCamera2(DragGesture gesture) {
        if (gesture.Phase == ContinuousGesturePhase.Started) {
            prevPosition = dragOrigin = gesture.Position;
            return;
        }

        //拖拽结束时发送更新大地图上建筑、怪物信息的请求
        if (gesture.Phase == ContinuousGesturePhase.Ended) { 
            //TiledMap.Instance.UpdateBigMapRequest();
        }

        if ((gesture.Phase == ContinuousGesturePhase.None)) return;

        if (dragOrigin.y <= bottomMargin) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(new Vector3(gesture.Position.x, gesture.Position.y, 0f) - prevPosition);
        prevPosition = new Vector3(gesture.Position.x, gesture.Position.y, 0f);
        //Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        if(TiledMap.Instance.isActiveAndEnabled == true)
            transform.Translate(move, Space.World);

        TiledMap.Instance.OnDragCamera();
    }

}
