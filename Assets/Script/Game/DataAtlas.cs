using System;
using System.Collections.Generic;



public class Buildcampdata : DataBase
{

    public int id;

    public string name;

}


public class Buildqualitydata : DataBase
{

    public int id;

    public string name;

}


public class Mapdata : DataBase
{

    public int id;

    public string name;

    public int level;

    public int type;

    public int coordinate;

    public int iconid;

    public int model;

    public int material;

    public string quality;

    public string camp;

    public int area;

    public Buildqualitydata getQuality()
    {

        return DataAtlasManager.Instance.getDataWithTypeById<Buildqualitydata>(quality.ToString());

    }

    public Buildcampdata getCamp()
    {

        return DataAtlasManager.Instance.getDataWithTypeById<Buildcampdata>(camp.ToString());

    }

}


public class Shipdata : DataBase
{

    public int id;

    public string Icon;

    public int IsPlayer;

    public string Model;

    public string Material;

    public string Name;

    public int Rarity;

    public string Type;

    public int Hp;

    public int AirDef;

    public int AntiSubmarine;

    public int Radius;

    public int CanAtk;

    public int CanAtkSpd;

    public int AirAtk;

    public int AirAtkSpd;

    public int TorAtk;

    public int TorAtkSpd;

    public int Armor;

    public int Strength;

    public int Accuracy;

    public int Caliber;

    public int Mobility;

    public int Capacity;

    public Shiptypes getType()
    {

        return DataAtlasManager.Instance.getDataWithTypeById<Shiptypes>(Type.ToString());

    }

}


public class Shipskills : DataBase
{

    public int id;

    public string Name;

    public string Des;

    public int Type;

    public int TriggerProbility;

    public int BuffId;

    public double skillAddition;

    public int TargetType;

    public int TargetPosition;

    public int TargetNum;

}


public class Shiptypes : DataBase
{

    public int id;

    public string Name;

}


public class Skillbuffs : DataBase
{

    public int id;

    public string Name;

    public string Des;

    public double Hp;

    public double CanAtk;

    public double CanAtkSpd;

    public double TorAtk;

    public double TorAtkSpd;

    public double AirAtk;

    public double AirAtkSpd;

    public double AirDef;

    public double AntiSubmarine;

    public double Radius;

    public double Capacity;

    public double CannonDef;

    public double TorpedoDef;

    public double AircraftDef;

    public double CanCriRed;

    public double CanCriDmgRed;

    public double TorCriRed;

    public double TorCriDmgRed;

    public double AirCriRed;

    public double AirCriDmgRed;

    public double CanHit;

    public double CanCritic;

    public double TorHit;

    public double TorCritic;

    public double AirHit;

    public double AirCritic;

    public double CanCriDmg;

    public double TorCriDmg;

    public double AirCriDmg;

    public double CanEva;

    public double TorEva;

    public double AirEva;

    public double Speed;

}


public class Testbool : DataBase
{

    public int id;

    public string name;

    public double attack;

    public double defence;

    public bool export;

}


public class Testequiptable : DataBase
{

    public string id;

    public string name;

    public string icon;

    public double price;

}


public class Testplayer : DataBase
{

    public string id;

    public string name;

    public string value;

    public double gold;

    public string money;

    public string skill;

    public string levelproperties;

    public Testplayer getSkill()
    {

        return DataAtlasManager.Instance.getDataWithTypeById<Testplayer>(skill.ToString());

    }

    public List<Testproperties> getLevelproperties()
    {

        string prefix = levelproperties;

        if (prefix == null || prefix == string.Empty)
        {

            prefix = id;

        }

        return DataAtlasManager.Instance.getDataWithTypeWithPrefix<Testproperties>(prefix.ToString());

    }

}


public class Testproperties : DataBase
{

    public string id;

    public string name;

    public string icon;

    public string equip;

    public Testequiptable getEquip()
    {

        return DataAtlasManager.Instance.getDataWithTypeById<Testequiptable>(equip.ToString());

    }

}


public class Testship : DataBase
{

    public int id;

    public string name;

    public int level;

    public int needlevel;

    public int iconid;

    public int model;

    public int needtime;

    public int type;

    public int typeparam1;

    public int typeparam2;

    public int typeparam3;

    public int typeparam4;

    public int activePreBulidID1;

    public int activePreBulidID2;

    public int activePreBulidID3;

    public int activePreBulidID4;

    public int activeNeedOil;

    public int activeNeedSteel;

    public int activeNeedAlu;

    public int activeNeedTun;

    public string Basepoint;

}


public class Testskilltable : DataBase
{

    public string id;

    public string name;

    public double attack;

    public double defence;

    public bool export;

    public string rangle;

}


public class DataAtlas : DataBase
{

    public List<Buildcampdata> buildcampdataList;

    public List<Buildqualitydata> buildqualitydataList;

    public List<Mapdata> mapdataList;

    public List<Shipdata> shipdataList;

    public List<Shipskills> shipskillsList;

    public List<Shiptypes> shiptypesList;

    public List<Skillbuffs> skillbuffsList;

    public List<Testbool> testboolList;

    public List<Testequiptable> testequiptableList;

    public List<Testplayer> testplayerList;

    public List<Testproperties> testpropertiesList;

    public List<Testship> testshipList;

    public List<Testskilltable> testskilltableList;

    public void initialize(Dictionary<string, Dictionary<string, DataBase>> dataDic)
    {

        Dictionary<string, DataBase> buildcampdataDic;

        if (dataDic.ContainsKey("Buildcampdata"))
        {
            buildcampdataDic = dataDic["Buildcampdata"];
        }
        else
        {
            buildcampdataDic = new Dictionary<string, DataBase>();
            dataDic["Buildcampdata"] = buildcampdataDic;
        }

        for (int i = 0; i < buildcampdataList.Count; i++)
        {
            Buildcampdata cell = buildcampdataList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                buildcampdataDic = dataDic[id];
            }
            else
            {

                buildcampdataDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> buildqualitydataDic;

        if (dataDic.ContainsKey("Buildqualitydata"))
        {
            buildqualitydataDic = dataDic["Buildqualitydata"];
        }
        else
        {
            buildqualitydataDic = new Dictionary<string, DataBase>();
            dataDic["Buildqualitydata"] = buildqualitydataDic;
        }

        for (int i = 0; i < buildqualitydataList.Count; i++)
        {
            Buildqualitydata cell = buildqualitydataList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                buildqualitydataDic = dataDic[id];
            }
            else
            {

                buildqualitydataDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> mapdataDic;

        if (dataDic.ContainsKey("Mapdata"))
        {
            mapdataDic = dataDic["Mapdata"];
        }
        else
        {
            mapdataDic = new Dictionary<string, DataBase>();
            dataDic["Mapdata"] = mapdataDic;
        }

        for (int i = 0; i < mapdataList.Count; i++)
        {
            Mapdata cell = mapdataList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                mapdataDic = dataDic[id];
            }
            else
            {

                mapdataDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> shipdataDic;

        if (dataDic.ContainsKey("Shipdata"))
        {
            shipdataDic = dataDic["Shipdata"];
        }
        else
        {
            shipdataDic = new Dictionary<string, DataBase>();
            dataDic["Shipdata"] = shipdataDic;
        }

        for (int i = 0; i < shipdataList.Count; i++)
        {
            Shipdata cell = shipdataList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                shipdataDic = dataDic[id];
            }
            else
            {

                shipdataDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> shipskillsDic;

        if (dataDic.ContainsKey("Shipskills"))
        {
            shipskillsDic = dataDic["Shipskills"];
        }
        else
        {
            shipskillsDic = new Dictionary<string, DataBase>();
            dataDic["Shipskills"] = shipskillsDic;
        }

        for (int i = 0; i < shipskillsList.Count; i++)
        {
            Shipskills cell = shipskillsList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                shipskillsDic = dataDic[id];
            }
            else
            {

                shipskillsDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> shiptypesDic;

        if (dataDic.ContainsKey("Shiptypes"))
        {
            shiptypesDic = dataDic["Shiptypes"];
        }
        else
        {
            shiptypesDic = new Dictionary<string, DataBase>();
            dataDic["Shiptypes"] = shiptypesDic;
        }

        for (int i = 0; i < shiptypesList.Count; i++)
        {
            Shiptypes cell = shiptypesList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                shiptypesDic = dataDic[id];
            }
            else
            {

                shiptypesDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> skillbuffsDic;

        if (dataDic.ContainsKey("Skillbuffs"))
        {
            skillbuffsDic = dataDic["Skillbuffs"];
        }
        else
        {
            skillbuffsDic = new Dictionary<string, DataBase>();
            dataDic["Skillbuffs"] = skillbuffsDic;
        }

        for (int i = 0; i < skillbuffsList.Count; i++)
        {
            Skillbuffs cell = skillbuffsList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                skillbuffsDic = dataDic[id];
            }
            else
            {

                skillbuffsDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> testboolDic;

        if (dataDic.ContainsKey("Testbool"))
        {
            testboolDic = dataDic["Testbool"];
        }
        else
        {
            testboolDic = new Dictionary<string, DataBase>();
            dataDic["Testbool"] = testboolDic;
        }

        for (int i = 0; i < testboolList.Count; i++)
        {
            Testbool cell = testboolList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                testboolDic = dataDic[id];
            }
            else
            {

                testboolDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> testequiptableDic;

        if (dataDic.ContainsKey("Testequiptable"))
        {
            testequiptableDic = dataDic["Testequiptable"];
        }
        else
        {
            testequiptableDic = new Dictionary<string, DataBase>();
            dataDic["Testequiptable"] = testequiptableDic;
        }

        for (int i = 0; i < testequiptableList.Count; i++)
        {
            Testequiptable cell = testequiptableList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                testequiptableDic = dataDic[id];
            }
            else
            {

                testequiptableDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> testplayerDic;

        if (dataDic.ContainsKey("Testplayer"))
        {
            testplayerDic = dataDic["Testplayer"];
        }
        else
        {
            testplayerDic = new Dictionary<string, DataBase>();
            dataDic["Testplayer"] = testplayerDic;
        }

        for (int i = 0; i < testplayerList.Count; i++)
        {
            Testplayer cell = testplayerList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                testplayerDic = dataDic[id];
            }
            else
            {

                testplayerDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> testpropertiesDic;

        if (dataDic.ContainsKey("Testproperties"))
        {
            testpropertiesDic = dataDic["Testproperties"];
        }
        else
        {
            testpropertiesDic = new Dictionary<string, DataBase>();
            dataDic["Testproperties"] = testpropertiesDic;
        }

        for (int i = 0; i < testpropertiesList.Count; i++)
        {
            Testproperties cell = testpropertiesList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                testpropertiesDic = dataDic[id];
            }
            else
            {

                testpropertiesDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> testshipDic;

        if (dataDic.ContainsKey("Testship"))
        {
            testshipDic = dataDic["Testship"];
        }
        else
        {
            testshipDic = new Dictionary<string, DataBase>();
            dataDic["Testship"] = testshipDic;
        }

        for (int i = 0; i < testshipList.Count; i++)
        {
            Testship cell = testshipList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                testshipDic = dataDic[id];
            }
            else
            {

                testshipDic.Add(id, cell);
            }

        }

        Dictionary<string, DataBase> testskilltableDic;

        if (dataDic.ContainsKey("Testskilltable"))
        {
            testskilltableDic = dataDic["Testskilltable"];
        }
        else
        {
            testskilltableDic = new Dictionary<string, DataBase>();
            dataDic["Testskilltable"] = testskilltableDic;
        }

        for (int i = 0; i < testskilltableList.Count; i++)
        {
            Testskilltable cell = testskilltableList[i];
            string id = cell.id.ToString();
            if (dataDic.ContainsKey(id))
            {
                testskilltableDic = dataDic[id];
            }
            else
            {

                testskilltableDic.Add(id, cell);
            }

        }

    }
}
