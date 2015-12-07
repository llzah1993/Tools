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

        //λ�ã�ռ�Ŀ�
    }

    public class TechTreeInfo
    {

        /// <summary>
        /// ӵ�еļ��ܵ���
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
        /// ʣ���VIP��Чʱ�䣬���������ж�VIP�Ƿ񱻼���
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
    /// ��ҵĳ���(λ�ڴ��ͼ)
    /// </summary>
    public class CityInfo
    {

        public string cityName;

        public int cityLevel;

        /// <summary>
        /// �������꣺<KindomID,X,Y>
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
        /// mesh��ÿһ����meshRow�鹹��
        /// </summary>
        public int meshRow;

        /// <summary>
        /// mesh��ÿһ����meshCol�鹹��
        /// </summary>
        public int meshCol;

    }

}