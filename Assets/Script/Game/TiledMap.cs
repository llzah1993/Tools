using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TiledMap : MonoBehaviour {

    #region Singleton

    private TiledMap() { }

    private static TiledMap instance;

    public static TiledMap Instance { get { if (instance == null) { instance = new TiledMap(); } return instance; } }

    #endregion

    /// <summary>
    /// tile中有用部分的宽度
    /// </summary>
    private float mUsefulWidth;

    /// <summary>
    /// tile中有用部分的高度
    /// </summary>
    private float mUsefulHeight;

    /// <summary>
    /// tile的宽度
    /// </summary>
    private float mTileWidth;

    /// <summary>
    /// tile的高度
    /// </summary>
    private float mTileHeight;

    /// <summary>
    /// 地图的宽度，由mMapWidth个tile构成，方向为right-down
    /// </summary>
    private int mMapWidth;

    public int MapWidth { get { return mMapWidth; } }

    /// <summary>
    /// 地图的高度，由mMapHeight个tile构成，方向为left-down
    /// </summary>
    private int mMapHeight;

    public int MapHeight { get { return mMapHeight; } }

    /// <summary>
    /// 负责存放所有tile的图片的宽度
    /// </summary>
    private float mSpriteWidth;

    /// <summary>
    /// 负责存放所有tile的图片的高度
    /// </summary>
    private float mSpriteHeight;

    /// <summary>
    /// ground层tile的数目
    /// </summary>
    private int mGroundTileNumber;

    /// <summary>
    /// house&people层tile的数目
    /// </summary>
    //private int mHousePeopleTileNumber;

    /// <summary>
    /// 切片时需要用到的参数，很难解释，你们不需要懂
    /// </summary>
    private float mSpace;

    /// <summary>
    /// 切片时需要用到的参数，很难解释，你们不需要懂
    /// </summary>
    private float mMargin;

    /// <summary>
    /// 大地图上的tile编号(由X,Y坐标计算得到，范围[0-mMapWidth*mMapHeight-1])=>ground层tile编号[1-mGroundTileNumber]之间的转换关系，UpdateGroundMesh负责填充数据
    /// </summary>
    private int[] mMapPosition2GroundTileID; 

    /// <summary>
    /// ground层tile编号[1-mGroundTileNumber]=>UV坐标之间的转换关系
    /// </summary>
    private Vector2[] mGroundTileID2UV;

    /// <summary>
    /// 大地图上的tile编号(由X,Y坐标计算得到，范围[0-mMapWidth*mMapHeight-1])=>house&people层tile编号[1-mHousePeopleTileNumber]之间的转换关系，UpdateHousePeopleMesh负责填充数据
    /// </summary>
    [HideInInspector]
    public int[] mapPosition2HousePeopleTileID;

    /// <summary>
    /// house&people层tile编号[1-mHousePeopleTileNumber]=>UV坐标之间的转换关系
    /// </summary>
    //private Vector2[] mHousePeopleTileID2UV;

    /// <summary>
    /// 创建由mMeshRow * mMeshCol个tile构成的Mesh
    /// </summary>
    private int mMeshRow;
    private int mMeshCol;

    /// <summary>
    /// 用于判断摄像机中心点是否在当前mesh之内的参数
    /// </summary>
    private float mMeshHalfWidth;
    private float mMeshHalfHeight;

    /// <summary>
    /// 四边形的顶点数
    /// </summary>
    private const int quad_num = 4;

    /// <summary>
    /// 三角形的顶点数
    /// </summary>
    private const int triangle_num = 6;

    /// <summary>
    /// 玩家的HOME坐标(相对于大地图)，横坐标的范围是[0,mMapWidth-1]，纵坐标的范围是[0,mMapHeight-1]
    /// </summary>
    private Point2D mHomePosition;

    /// <summary>
    /// 指示玩家当前在大地图上的位置
    /// </summary>
    private Point2D mCurrentPosition;
    
    //摄像机的宽高(世界坐标系)
    private float mCameraHalfWidth;
    private float mCameraHalfHeight;

    //设计的屏幕大小
    private float designW = 768;
    private float designH = 1024;

    /// <summary>
    /// 4块GroundMesh对应的hierarchy中的transform
    /// </summary>
    private Transform mGroundMeshsTransform;

    /// <summary>
    /// 4块HousePeopleMesh对应的hierarchy中的transform
    /// </summary>
    private Transform mHousePeopleMeshsTransform;

    /// <summary>
    /// 大地图上的坐标
    /// </summary>
    class Point2D {
        public Point2D(int xx, int yy) {
            x = xx;
            y = yy;
        }
        public int x;
        public int y;
    }

    /// <summary>
    /// 用于判断点是否在菱形内的数据结构
    /// </summary>
    class Inside {
        public Inside() {
            insideX = insideY = insideXY = 0f;
        }
        public float insideX;
        public float insideY;
        public float insideXY;
    }

    /// <summary>
    /// 摄像机有4个顶点，需要计算它们和菱形之间的位置关系
    /// </summary>
    private Inside[] cameraInside;

    /// <summary>
    /// 游戏地图很大，没法一次性全部展现，所以我选择通过变换4块mesh的位置和UV的方式来模拟整张地图，IMesh是用于存储相关信息的数据结构
    /// </summary>
    class IMesh {
        public IMesh(int ID, Transform trans) {
            id = ID;
            transform = trans;
            x = 0;
            y = 0;
            neigubours = new Dictionary<Direction, IMesh>();
        }
        public int id;
        public Transform transform;
        public int x;
        public int y;
        public Dictionary<Direction, IMesh> neigubours;
    }

    /// <summary>
    /// 全部QUAD_NUM个ground mesh的集合
    /// </summary>
    private IMesh[] allGroundMeshes;  

    /// <summary>
    /// 当前活跃的ground mesh
    /// </summary>
    private IMesh mActiveGroundMesh;

    /// <summary>
    /// 全部QUAD_NUM个house&people mesh的集合
    /// </summary>
    private IMesh[] allHousePeopleMeshes;

    /// <summary>
    /// 当前活跃的house&people mesh
    /// </summary>
    private IMesh mActiveHousePeopleMesh;

    /// <summary>
    /// 记录大地图上的建筑、怪物信息
    /// </summary>
    public class HousePeople {
        public HousePeople(uint id,ushort xx,ushort yy,byte tp,ulong uid,string uname,byte lev) {
            tileId = id;
            x = xx;
            y = yy;
            type = tp;
            userId = uid;
            userName = uname;
            lev = level;
        }
        public uint tileId;    //暂时不用
        public ushort x;   //在大地图上的横坐标
        public ushort y;   //在大地图上的纵坐标
        public byte type;   //建筑、怪物的类型,通过它对应到tile id
        public ulong userId;   //玩家的id
        public string userName;    //玩家的名字
        public byte level;  //建筑、怪物的等级
    }

    /// <summary>
    /// 方向
    /// </summary>
    private enum Direction {
        left = 1,
        leftdown = 2,
        down = 3,
        rightdown = 4,
        right = 9,
        rightup = 8,
        up = 7,
        leftup = 6,
        nodirection = 10,
    };

    /// <summary>
    /// 测试用
    /// </summary>
    private Vector2[] specialUV;

    void Start() {
        Init();
    }

    /// <summary>
    /// 初始化必要的数据
    /// </summary>
    public void Init() {

        instance = this;

        specialUV = new Vector2[quad_num];
        specialUV[0] = new Vector2(272f / 2048, 763f / 2048);
        specialUV[1] = new Vector2(272f / 2048, 623f / 2048);
        specialUV[2] = new Vector2(528f / 2048, 623f / 2048);
        specialUV[3] = new Vector2(528f / 2048, 763f / 2048);

        mGroundMeshsTransform = transform.GetChild(0);
        mHousePeopleMeshsTransform = transform.GetChild(1);

        //可以调大小
        mMeshRow = 11;
        mMeshCol = 11;

        //从服务器获取
        mHomePosition = new Point2D(103,301);   //随便写的，测试用

        //当前位置初始化为HOME坐标
        mCurrentPosition = new Point2D(mHomePosition.x, mHomePosition.y);

        float percent = designW / designH;
        mCameraHalfWidth = Camera.main.orthographicSize * percent;
        mCameraHalfHeight = Camera.main.orthographicSize;
        cameraInside = new Inside[quad_num];
        for (int i = 0; i < quad_num; ++i)
            cameraInside[i] = new Inside();

        ReadMap();

        ReadUVs();

        mMeshHalfWidth = mMeshRow * mUsefulWidth / 2;
        mMeshHalfHeight = mMeshCol * mUsefulHeight / 2;

        allGroundMeshes = new IMesh[quad_num];
        for (int i = 0; i < quad_num; ++i)
            allGroundMeshes[i] = new IMesh(i, mGroundMeshsTransform.GetChild(i));
        SetActive(allGroundMeshes[0], mHomePosition.x, mHomePosition.y);  //根据mHomeX和mHomeY计算出来
        UpdateGroundMesh(mHomePosition.x, mHomePosition.y, mActiveGroundMesh, Direction.nodirection); //定位到home所在mesh   //根据mHomePosition计算出来

        allHousePeopleMeshes = new IMesh[quad_num];
        for (int i = 0; i < quad_num; ++i)
            allHousePeopleMeshes[i] = new IMesh(i, mHousePeopleMeshsTransform.GetChild(i));

        Camera.main.transform.position = new Vector3(0f, -((int)(mMeshCol / 2)) * mUsefulHeight, -100f);  //初始化camera位置

    }

    /// <summary>
    /// 将imesh设置为mActiveGroundMesh
    /// </summary>
    /// <param name="imesh"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void SetActive(IMesh imesh, int x, int y) {
        mActiveGroundMesh = imesh;
        mActiveGroundMesh.x = x;
        mActiveGroundMesh.y = y;
    }

    /// <summary>
    /// 更新mActiveGroundMesh自己或者添加周围的mesh
    /// </summary>
    /// <param name="homex"></param>
    /// <param name="homey"></param>
    void UpdateGroundMesh(int homex, int homey, IMesh imesh, Direction dir) {
        if (mActiveGroundMesh.neigubours.ContainsKey(dir) && mActiveGroundMesh.neigubours[dir].id == imesh.id) {
            UpdateHousePeopleMesh(homex, homey, imesh);
            return;
        }
        Transform transform0 = imesh.transform;
        Transform transform1 = mHousePeopleMeshsTransform.GetChild(imesh.id);
        int startx = homex - mMeshRow / 2;
        int starty = homey - mMeshCol / 2;
        Vector2[] tempUV0 = new Vector2[mMeshRow * mMeshCol * 4];

        int count = 0;
        int startTileId;
        int tempstarty = starty;
        for (int j = starty; j < starty + mMeshCol; ++j) {
            startTileId = startx + tempstarty * mMapWidth;
            for (int i = startx; i < startx + mMeshRow; ++i) {

                //ground
                if (i < 0 || j < 0 || i >= mMapWidth || j >= mMapHeight || startTileId < 0 || startTileId >= mMapWidth * mMapHeight || mMapPosition2GroundTileID[startTileId] == 0) {
                    tempUV0[count + 0] = new Vector2(0f, 0f);
                    tempUV0[count + 1] = new Vector2(0f, 0f);
                    tempUV0[count + 2] = new Vector2(0f, 0f);
                    tempUV0[count + 3] = new Vector2(0f, 0f);
                } else {
                    tempUV0[count + 0] = mGroundTileID2UV[0 + (mMapPosition2GroundTileID[startTileId] - 1) * 4];
                    tempUV0[count + 1] = mGroundTileID2UV[1 + (mMapPosition2GroundTileID[startTileId] - 1) * 4];
                    tempUV0[count + 2] = mGroundTileID2UV[2 + (mMapPosition2GroundTileID[startTileId] - 1) * 4];
                    tempUV0[count + 3] = mGroundTileID2UV[3 + (mMapPosition2GroundTileID[startTileId] - 1) * 4];
                }
                    
                count += 4;
                startTileId++;
            }
            tempstarty++;
        }

        if (transform0 == null)
            return;
        transform0.GetComponent<MeshFilter>().mesh.uv = tempUV0;

        switch (dir) {
            case Direction.leftdown: transform0.position = new Vector3(-mMeshHalfWidth, -mMeshHalfHeight, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.down: transform0.position = new Vector3(0f, -2 * mMeshHalfHeight, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.rightdown: transform0.position = new Vector3(mMeshHalfWidth, -mMeshHalfHeight, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.right: transform0.position = new Vector3(2 * mMeshHalfWidth, 0f, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.rightup: transform0.position = new Vector3(mMeshHalfWidth, mMeshHalfHeight, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.up: transform0.position = new Vector3(0f, 2 * mMeshHalfHeight, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.leftup: transform0.position = new Vector3(-mMeshHalfWidth, mMeshHalfHeight, 0f) + mActiveGroundMesh.transform.position; break;
            case Direction.left: transform0.position = new Vector3(-2 * mMeshHalfWidth, 0f, 0f) + mActiveGroundMesh.transform.position; break;
        }
        transform1.position = transform0.position + new Vector3(0f, 0f, -1f);

        mActiveGroundMesh.neigubours.Clear();
        mActiveGroundMesh.neigubours.Add(dir, imesh);
        mActiveGroundMesh.neigubours[dir].x = homex;
        mActiveGroundMesh.neigubours[dir].y = homey;

        Direction reverseDir = (Direction)(Direction.nodirection - dir);
        if (imesh.neigubours.ContainsKey(reverseDir)) {
            imesh.neigubours[reverseDir] = mActiveGroundMesh;
        } else {
            imesh.neigubours.Add(reverseDir, mActiveGroundMesh);
        }

        UpdateHousePeopleMesh(homex, homey, imesh);

    }

    /// <summary>
    /// 更新mActiveHousePeopleMesh及其周围mesh上的建筑、怪物信息
    /// </summary>
    void UpdateHousePeopleMesh(int homex, int homey, IMesh imesh) {
        Transform transform = mHousePeopleMeshsTransform.GetChild(imesh.id);
        int startx = homex - mMeshRow / 2;
        int starty = homey - mMeshCol / 2;
        Vector2[] tempUV1 = new Vector2[mMeshRow * mMeshCol * 4];
        int count = 0;
        int startTileId;
        int tempstarty = starty;
        for (int j = starty; j < starty + mMeshCol; ++j) {
            startTileId = startx + tempstarty * mMapWidth;
            for (int i = startx; i < startx + mMeshRow; ++i) {
                if (i < 0 || j < 0 || i >= mMapWidth || j >= mMapHeight || startTileId < 0 || startTileId >= mMapWidth * mMapHeight || mapPosition2HousePeopleTileID[startTileId] == 0) {
                    tempUV1[count + 0] = new Vector2(0f, 0f);
                    tempUV1[count + 1] = new Vector2(0f, 0f);
                    tempUV1[count + 2] = new Vector2(0f, 0f);
                    tempUV1[count + 3] = new Vector2(0f, 0f);
                } else {
                    tempUV1[count + 0] = specialUV[0];
                    tempUV1[count + 1] = specialUV[1];
                    tempUV1[count + 2] = specialUV[2];
                    tempUV1[count + 3] = specialUV[3];
                }

                count += 4;
                startTileId++;
            }
            tempstarty++;
        }
        if (transform == null)
            return;
        transform.GetComponent<MeshFilter>().mesh.uv = tempUV1;
    }

    //void OnEnable() {
    //    Init();
    //    RequestUpdateBigMap();
    //}

    void OnDisable() {
        StopAllCoroutines();
    }

    private float updateHousePeopleFrequence = 1f;
    private float timer = 0f;

    /// <summary>
    /// 处理拖拽事件
    /// </summary>
    public void OnDragCamera() {

            //更新mCurrentPosition
            Vector2 cameraPosition = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
            Vector2 origionPosition = new Vector2(mActiveGroundMesh.transform.position.x, mActiveGroundMesh.transform.position.y - ((int)(mMeshCol / 2)) * mUsefulHeight);
            float deltaX = cameraPosition.x - origionPosition.x;
            float deltaY = cameraPosition.y - origionPosition.y;
            int xx = 0, yy = 0;
            if (Mathf.Abs(deltaX) >= mUsefulWidth / 2) {
                int delta = 1 + (int)((Mathf.Abs(deltaX) - mUsefulWidth / 2) / mUsefulWidth);
                if (deltaX >= 0) { xx += delta; yy -= delta; } else { xx += delta; yy -= delta; }
            }
            if (Mathf.Abs(deltaY) >= mUsefulHeight / 2) {
                int delta = 1 + (int)((Mathf.Abs(deltaY) - mUsefulHeight / 2) / mUsefulHeight);
                if (deltaY >= 0) { xx -= delta; yy -= delta; } else { xx += delta; yy += delta; }
            }
            mCurrentPosition = new Point2D(mActiveGroundMesh.x + xx, mActiveGroundMesh.y + yy);
            //用户已经离开当前tile，请求更新建筑、怪物信息
            //if (xx != 0 && yy != 0) {
            //    RequestUpdateBigMap();
            //}

            //Debug.Log("xx = " + mCurrentPosition.x.ToString() + ",yy = " + mCurrentPosition.y.ToString());

            //更新mActiveGroundMesh
            mActiveGroundMesh = null;
            for (int i = 0; i < quad_num; i++) {
                CalculateCameraCorner(allGroundMeshes[i]);
                if (cameraInside[0].insideXY <= 1f && cameraInside[1].insideXY <= 1f && cameraInside[2].insideXY <= 1f && cameraInside[3].insideXY <= 1f) {  //摄像机完全位于AllMeshes[i]内
                    SetActive(allGroundMeshes[i], allGroundMeshes[i].x, allGroundMeshes[i].y);  //摄像机的四个顶点全部在AllMeshes[i]内，所以更新mActiveGroundMesh
                    break;
                }
            }

            Debug.Log(cameraInside[0].insideXY.ToString() + "," + cameraInside[1].insideXY.ToString() + "," + cameraInside[2].insideXY.ToString() + "," + cameraInside[3].insideXY.ToString() + ",");

            if (mActiveGroundMesh != null) {
                AnalyzeCameraInside(mActiveGroundMesh);
            } else {
                for (int i = 0; i < quad_num; i++) {
                    if (CalculateCameraCenter(allGroundMeshes[i]) == true) {
                        SetActive(allGroundMeshes[i], allGroundMeshes[i].x, allGroundMeshes[i].y);  //摄像机的中心点在imesh内，所以更新mActiveGroundMesh
                        AnalyzeCameraOutside(mActiveGroundMesh);
                        break;
                    }
                }
            }

    }

    void Update() {

        #region 定时更新建筑、怪物信息，针对不在移动或移动缓慢的情况

        if (timer >= updateHousePeopleFrequence) {
            UpdateHousePeopleMesh(mActiveGroundMesh.x, mActiveGroundMesh.y, mActiveGroundMesh);
            timer = 0f;
        } else {
            timer += Time.deltaTime;
        }

        #endregion
    }

    /// <summary>
    /// 向服务器请求一次大地图上的建筑、怪物信息，只是发送消息，真正负责更新显示的地方在UpdateHousePeopleMesh
    /// </summary>
    public void RequestUpdateBigMap() {
        //LiteRequest enterReq = new LiteRequest(NetProtocols.HANDLE_PROTO_MAP_VIEW, "hhb", new object[] { mCurrentPosition.x, mCurrentPosition.y, 5 });
        LiteRequest enterReq = new LiteRequest(NetProtocols.HANDLE_PROTO_MAP_VIEW, "lhhb", new object[] { DateTime.Now.Ticks, mCurrentPosition.x, mCurrentPosition.y, mMeshRow }); //发送当前时间戳
        enterReq.Send(NetworkManager.Connector);
        //Debug.Log("Message sent");    //过多的log会让帧率急剧下降哦
    }

    /// <summary>   
    /// 摄像机的四个顶点不全在imesh的情况下，根据摄像机的中心点决定是否扩充mesh。不管是否需要扩充，都需要对新添加的mesh更新建筑、怪物信息
    /// </summary>
    /// <param name="imesh"></param>
    void AnalyzeCameraOutside(IMesh imesh) {

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 center = new Vector3(0, -((int)(mMeshCol / 2)) * mUsefulHeight, 0) + imesh.transform.position;    //center of imesh
        float insideX = (cameraPosition.x - center.x) / mMeshHalfWidth;
        float insideY = (cameraPosition.y - center.y) / mMeshHalfHeight;
        if (insideX <= -0.3f) { AddMeshLeft(); } else if (insideX >= 0.3f) { AddMeshRight(); } else if (insideY >= 0.3f) { AddMeshUp(); } else if (insideY <= -0.3f) { AddMeshDown(); } //0.3这个数字是试出来的，但是没有理论依据
        else {
            foreach(Direction direction in mActiveGroundMesh.neigubours.Keys) {
                IMesh currentMesh = mActiveGroundMesh.neigubours[direction];
                UpdateHousePeopleMesh(currentMesh.x, currentMesh.y, currentMesh);
            }
        }

    }

    /// <summary>
    /// 摄像机四个顶点都在imesh的情况下，根据摄像机的四个顶点决定是否扩充mesh。如果需要扩充，则同时需要对新添加的mesh更新建筑、怪物信息，否则只需要更新当前mesh上的建筑、怪物信息。
    /// </summary>
    void AnalyzeCameraInside(IMesh imesh) {
        UpdateHousePeopleMesh(mActiveGroundMesh.x, mActiveGroundMesh.y, mActiveGroundMesh);
        if (cameraInside[3].insideX >= 0.45f && cameraInside[2].insideY <= -0.45f) {
            AddMeshDown();
        } else if (cameraInside[3].insideX >= 0.45f && cameraInside[3].insideY >= 0.45f) {
            AddMeshUp();
        } else if (cameraInside[3].insideX >= 0.45f && cameraInside[3].insideY < 0.45f && cameraInside[2].insideY > -0.45f) {
            AddMeshRight();
        } else if (cameraInside[0].insideX <= -0.45 && cameraInside[0].insideY >= 0.45f) {
            AddMeshUp();
        } else if (cameraInside[0].insideX <= -0.45 && cameraInside[1].insideY <= -0.45f) {
            AddMeshDown();
        } else if (cameraInside[0].insideX <= -0.45 && cameraInside[1].insideY > -0.45f && cameraInside[0].insideY < 0.45f) {
            AddMeshLeft();
        } else if (cameraInside[0].insideX > -0.45f && cameraInside[3].insideX < 0.45f && cameraInside[0].insideY >= 0.45f) {
            AddMeshUp();
        } else if (cameraInside[0].insideX > -0.45f && cameraInside[3].insideX < 0.45f && cameraInside[1].insideY <= -0.45f) {
            AddMeshDown();
        }
    }

    void AddMeshDown() {
        UpdateHousePeopleMesh(mActiveGroundMesh.x, mActiveGroundMesh.y, mActiveGroundMesh); 
        UpdateGroundMesh(mActiveGroundMesh.x, mActiveGroundMesh.y + mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 1) % quad_num], Direction.leftdown); //左下角mesh
        UpdateGroundMesh( mActiveGroundMesh.x + mMeshRow, mActiveGroundMesh.y + mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 2) % quad_num], Direction.down);  //下方mesh
        UpdateGroundMesh( mActiveGroundMesh.x + mMeshRow, mActiveGroundMesh.y, allGroundMeshes[(mActiveGroundMesh.id + 3) % quad_num], Direction.rightdown);  //右下角mesh
    }

    void AddMeshUp() {
        UpdateHousePeopleMesh(mActiveGroundMesh.x, mActiveGroundMesh.y, mActiveGroundMesh);
        UpdateGroundMesh(mActiveGroundMesh.x, mActiveGroundMesh.y - mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 1) % quad_num], Direction.rightup);  //右上角mesh
        UpdateGroundMesh( mActiveGroundMesh.x - mMeshRow, mActiveGroundMesh.y - mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 2) % quad_num], Direction.up);  //上方mesh
        UpdateGroundMesh( mActiveGroundMesh.x - mMeshRow, mActiveGroundMesh.y, allGroundMeshes[(mActiveGroundMesh.id + 3) % quad_num], Direction.leftup); //左上角mesh
    }

    void AddMeshRight() {
        UpdateHousePeopleMesh(mActiveGroundMesh.x, mActiveGroundMesh.y, mActiveGroundMesh);
        UpdateGroundMesh(mActiveGroundMesh.x + mMeshRow, mActiveGroundMesh.y, allGroundMeshes[(mActiveGroundMesh.id + 1) % quad_num], Direction.rightdown);  //右下角mesh
        UpdateGroundMesh( mActiveGroundMesh.x + mMeshRow, mActiveGroundMesh.y - mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 2) % quad_num], Direction.right);  //右方mesh
        UpdateGroundMesh( mActiveGroundMesh.x, mActiveGroundMesh.y - mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 3) % quad_num], Direction.rightup);  //右上角mesh
    }

    void AddMeshLeft() {
        UpdateHousePeopleMesh(mActiveGroundMesh.x, mActiveGroundMesh.y, mActiveGroundMesh);
        UpdateGroundMesh(mActiveGroundMesh.x - mMeshRow, mActiveGroundMesh.y, allGroundMeshes[(mActiveGroundMesh.id + 1) % quad_num], Direction.leftup); //左上角mesh
        UpdateGroundMesh( mActiveGroundMesh.x - mMeshRow, mActiveGroundMesh.y + mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 2) % quad_num], Direction.left);  //左方mesh
        UpdateGroundMesh( mActiveGroundMesh.x, mActiveGroundMesh.y + mMeshCol, allGroundMeshes[(mActiveGroundMesh.id + 3) % quad_num], Direction.leftdown); //左下角mesh
    }

    /// <summary>
    /// 计算摄像机四个顶点距离当前mesh中心点的位置
    /// </summary>
    void CalculateCameraCorner(IMesh imesh) {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraLeftupPosition = new Vector3(cameraPosition.x - mCameraHalfWidth, cameraPosition.y + mCameraHalfHeight, 0);
        Vector3 center = new Vector3(0, -((int)(mMeshCol / 2)) * mUsefulHeight, 0) + imesh.transform.position;    //center of imesh
        cameraInside[0].insideX = (cameraLeftupPosition.x - center.x) / mMeshHalfWidth;
        cameraInside[0].insideY = (cameraLeftupPosition.y - center.y) / mMeshHalfHeight;
        cameraInside[0].insideXY = Mathf.Abs(cameraInside[0].insideX) + Mathf.Abs(cameraInside[0].insideY);
        Vector3 cameraLeftbottomPosition = new Vector3(cameraPosition.x - mCameraHalfWidth, cameraPosition.y - mCameraHalfHeight, 0);
        cameraInside[1].insideX = (cameraLeftbottomPosition.x - center.x) / mMeshHalfWidth;
        cameraInside[1].insideY = (cameraLeftbottomPosition.y - center.y) / mMeshHalfHeight;
        cameraInside[1].insideXY = Mathf.Abs(cameraInside[1].insideX) + Mathf.Abs(cameraInside[1].insideY);
        Vector3 cameraRightbottomPosition = new Vector3(cameraPosition.x - mCameraHalfWidth, cameraPosition.y - mCameraHalfHeight, 0);
        cameraInside[2].insideX = (cameraRightbottomPosition.x - center.x) / mMeshHalfWidth;
        cameraInside[2].insideY = (cameraRightbottomPosition.y - center.y) / mMeshHalfHeight;
        cameraInside[2].insideXY = Mathf.Abs(cameraInside[2].insideX) + Mathf.Abs(cameraInside[2].insideY);
        Vector3 cameraRightupPosition = new Vector3(cameraPosition.x + mCameraHalfWidth, cameraPosition.y + mCameraHalfHeight, 0);
        cameraInside[3].insideX = (cameraRightupPosition.x - center.x) / mMeshHalfWidth;
        cameraInside[3].insideY = (cameraRightupPosition.y - center.y) / mMeshHalfHeight;
        cameraInside[3].insideXY = Mathf.Abs(cameraInside[3].insideX) + Mathf.Abs(cameraInside[3].insideY);
    }

    /// <summary>
    /// 判断摄像机中心是否在imesh内
    /// </summary>
    /// <param name="imesh"></param>
    /// <returns></returns>
    bool CalculateCameraCenter(IMesh imesh) {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 center = new Vector3(0, -((int)(mMeshCol / 2)) * mUsefulHeight, 0) + imesh.transform.position;    //center of imesh
        float InsideXY = Mathf.Abs((cameraPosition.x - center.x) / mMeshHalfWidth) + Mathf.Abs((cameraPosition.y - center.y) / mMeshHalfHeight);
        if (InsideXY <= 1f)
            return true;
        return false;
    }

    /// <summary>
    ///  从地图文件读取需要的信息
    /// </summary>
    void ReadMap() {
        string filepath = Application.dataPath + @"/Resources/Game/Data/map_data.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filepath);
        XmlNode record = xmlDoc.SelectSingleNode("/root/map");
        XmlElement xe = (XmlElement)record;
        mMapWidth = int.Parse(xe.GetAttribute("mapwidth"));
        mMapHeight = int.Parse(xe.GetAttribute("mapheight"));
        mUsefulWidth = float.Parse(xe.GetAttribute("unitwidth"));
        mUsefulHeight = float.Parse(xe.GetAttribute("unitheight"));

        record = xmlDoc.SelectSingleNode("/root/image");
        xe = (XmlElement)record;
        mSpriteWidth = float.Parse(xe.GetAttribute("width"));
        mSpriteHeight = float.Parse(xe.GetAttribute("height"));

        record = xmlDoc.SelectSingleNode("/root/tiles");
        xe = (XmlElement)record;
        mTileWidth = float.Parse(xe.GetAttribute("tilewidth"));
        mTileHeight = float.Parse(xe.GetAttribute("tileheight"));
        mSpace = float.Parse(xe.GetAttribute("spacing"));
        mMargin = float.Parse(xe.GetAttribute("margin"));
        mGroundTileNumber = int.Parse(xe.GetAttribute("tilecount"));
        //mHousePeopleTileNumber = 10;    //随便填的，测试用
        byte[] mapdata = Convert.FromBase64String(xe.GetAttribute("data"));
        mMapPosition2GroundTileID = new int[mapdata.Length / sizeof(int)];
        for (int i = 0; i < mapdata.Length; i += 4) {
            int temp = mapdata[i] + (mapdata[i + 1] << 8) + (mapdata[i + 2] << 16) + (mapdata[i + 3] << 24);
            mMapPosition2GroundTileID[i / 4] = temp;
        }
        mapPosition2HousePeopleTileID = new int[mapdata.Length / sizeof(int)];
        for (int i = 0; i < mapdata.Length; i += 4) {
            mapPosition2HousePeopleTileID[i / 4] = 0;   //暂时都填0，测试用
        }
    }

    /// <summary>
    /// 读取每一个tile的uv给mTileID2UV,方向为:左上角=>右下角
    /// </summary>
    void ReadUVs() {
        mGroundTileID2UV = new Vector2[mGroundTileNumber * 4];
        Vector2 _uv = new Vector2(mMargin / mSpriteWidth, (mMargin + mSpace) / mSpriteHeight);
        int count = 0;
        int rowNum = 14, colNum = 7;
        for (int row = 0; row < rowNum; row++) {
            for (int col = 0; col < colNum; col++) {
                mGroundTileID2UV[count++] = new Vector2(_uv.x + col * (mTileWidth + mSpace) / mSpriteWidth, 1 - _uv.y - row * (mTileHeight + mSpace) / mSpriteHeight);    //左上角
                mGroundTileID2UV[count++] = new Vector2(_uv.x + col * (mTileWidth + mSpace) / mSpriteWidth, 1 - _uv.y - (row * (mTileHeight + mSpace) + mTileHeight) / mSpriteHeight);  //左下角
                mGroundTileID2UV[count++] = new Vector2(_uv.x + (col * (mTileWidth + mSpace) + mTileWidth) / mSpriteWidth, 1 - _uv.y - (row * (mTileHeight + mSpace) + mTileHeight) / mSpriteHeight);    //右下角
                mGroundTileID2UV[count++] = new Vector2(_uv.x + (col * (mTileWidth + mSpace) + mTileWidth) / mSpriteWidth, 1 - _uv.y - row * (mTileHeight + mSpace) / mSpriteHeight);    //右上角
            }
        }
    }

    #region 可以退休的函数，因为需要的4块mesh已经被创建好并放置在游戏场景

    /// <summary>
    /// 生成row * col个tile构成的mesh
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    void CreateMesh(int row, int col) {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int count = 0;
        for (int index = 0; index < row; index++) {
            float offsetx = -index * mUsefulWidth / 2;
            float offsety = -index * mUsefulHeight / 2;

            for (int j = 0; j < col; j++) {
                Vector2 pos = new Vector2(j * mUsefulWidth / 2 + offsetx, -j * mUsefulHeight / 2 + offsety);   //1号tile的中心点坐标(位于模型坐标系)，从它开始创建row * col个tile
                Vector2 _uv = new Vector2(0f, 0f); //图片左下角的uv坐标，待更改
                AddTile(pos, _uv, vertices, uvs, triangles, ref count);
                //AddTile2Mesh(vertices, uvs, triangles);
            }

        }

        //统一赋值给mMesh
        //mMesh.vertices = vertices.ToArray();
        //mMesh.uv = uvs.ToArray();
        //mMesh.triangles = triangles.ToArray();
    }

    /// <summary>
    /// 创建一个tile，添加到mesh
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="_uv"></param>
    /// <param name="verticesList"></param>
    /// <param name="uvsList"></param>
    /// <param name="trianglesList"></param>
    /// <param name="count"></param>
    void AddTile(Vector2 pos, Vector2 _uv, List<Vector3> verticesList, List<Vector2> uvsList, List<int> trianglesList, ref int count) {

        Vector3[] vertices = new Vector3[quad_num];
        vertices[0] = new Vector3(pos.x - mTileWidth / 2, pos.y + mTileHeight / 2, 0);     //左上角顶点
        vertices[1] = new Vector3(pos.x - mTileWidth / 2, pos.y - mTileHeight / 2, 0);     //左下角顶点
        vertices[2] = new Vector3(pos.x + mTileWidth / 2, pos.y - mTileHeight / 2, 0);     //右下角顶点
        vertices[3] = new Vector3(pos.x + mTileWidth / 2, pos.y + mTileHeight / 2, 0);     //右上角顶点
        verticesList.Add(vertices[0]);
        verticesList.Add(vertices[1]);
        verticesList.Add(vertices[2]);
        verticesList.Add(vertices[3]);

        Vector2[] uvs = new Vector2[quad_num];
        uvs[0] = new Vector2(0f, 0f);
        uvs[1] = new Vector2(0f, 0f);
        uvs[2] = new Vector2(0f, 0f);
        uvs[3] = new Vector2(0f, 0f);
        uvsList.Add(uvs[0]);
        uvsList.Add(uvs[1]);
        uvsList.Add(uvs[2]);
        uvsList.Add(uvs[3]);

        //顺时针排列三角形
        int[] triangles = new int[triangle_num];
        triangles[0] = 0 + 4 * count;
        triangles[1] = 3 + 4 * count;
        triangles[2] = 2 + 4 * count;
        triangles[3] = 0 + 4 * count;
        triangles[4] = 2 + 4 * count;
        triangles[5] = 1 + 4 * count;
        trianglesList.Add(triangles[0]);
        trianglesList.Add(triangles[1]);
        trianglesList.Add(triangles[2]);
        trianglesList.Add(triangles[3]);
        trianglesList.Add(triangles[4]);
        trianglesList.Add(triangles[5]);

        count++;
    }

    #endregion

}
