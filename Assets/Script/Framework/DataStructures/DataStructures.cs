namespace Framework
{

    using System.Collections.Generic;
    using UnityEngine;

    public class UserInfo
    {

        public int level;

        public int experience;

        public int stamina;
    
    }

    public class BuildingInfo
    {

        public int buildingID;

        public int buildingType;

        public int buildingLevel;

        //位置，占的坑
    }

    public class TechTreeInfo
    {

        /// <summary>
        /// 拥有的技能点数
        /// </summary>
        public int techPoints;

    }
    
    public class TechInfo
    {

        public int techID;

        public string techName;

        public int techLevel;

    }

    public class RewardInfo
    {

        public int rewardType;

        public int rewardID;

    }

    public class VIPInfo
    {

        public int vipLevel;

        /// <summary>
        /// 剩余的VIP有效时间，可以用来判断VIP是否被激活
        /// </summary>
        long vipRemainingTime;

        int currentVipPoints;

    }

    public class ResourceInfo
    {

        public int resourceID;

        public int resourceType;

        public int resourceNumber;

    }

    public class FriendInfo
    {

        public long joyId;

        public string friendName;

        public string noteName;

        public bool isStared;

        public Vector3 friendCoordinate;

    }

    public class EquipmentInfo
    {

        public int equipType;

        public int equipID;

        public int equipQuality;

        public string equipName;

        public bool isEquiped;

    }

    public class TaskInfo
    {

        public int taskType;

        public int taskID;

    }

    /// <summary>
    /// 玩家的城市(位于大地图)
    /// </summary>
    public class CityInfo
    {

        public string cityName;

        public int cityLevel;

        /// <summary>
        /// 世界坐标：<KindomID,X,Y>
        /// </summary>
        public Vector3 cityCoodinate;

    }

    public class BuffInfo
    {

        public int buffType;

        public int buffID;

        public string buffName;

    }

    public class BigMapItemInfo
    {

        public int bigMapItemType;

        public int bigMapItemID;

        public int bigMapItemLevel;

        public Vector3 bigMapItemCoordinate;

    }

    public class MaterialInfo
    {

        public int materialType;

        public int materialID;

        public int materialQuality;

        public string materialName;

        public int materialNumbers;

    }

    public class BigMapInfo
    {

        /// <summary>
        /// mesh的每一行由meshRow块构成
        /// </summary>
        public int meshRow;

        /// <summary>
        /// mesh的每一列由meshCol块构成
        /// </summary>
        public int meshCol;

    }

}