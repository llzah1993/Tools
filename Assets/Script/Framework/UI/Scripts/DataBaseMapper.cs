using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using LitJson;
using System;
using UnityEngine.EventSystems;

namespace Framework
{
    public class MapperUtility
    {
        public static void setData(GameObject obj, string datastr)
        {


        }


        public static int ColorToInt(Color color)
        {
            int a = ((int)(color.a * 255.0f) << 24);
            int r = ((int)(color.a * 255.0f) << 16);
            int g = ((int)(color.a * 255.0f) << 8);
            int b = ((int)(color.a * 255.0f));
            return r + a + g + b;
        }

        public static Color IntToColor(int color)
        {
            byte a = (byte)((color & 0xff000000) >> 24);
            byte r = (byte)((color & 0x00ff0000) >> 16);
            byte g = (byte)((color & 0x0000ff00) >> 8);
            byte b = (byte)((color & 0x000000ff));
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
    }


    public abstract class BaseMapper
    {
        public enum CompType
        {
            CompType_Text,
            CompType_Image,
        }

        public CompType comptype;

        protected Component comp;

        public BaseMapper()
        {

        }

        public BaseMapper(Component c)
        {
            comp = c;
        }

        public abstract string Serialize();

        public abstract void DeSerialize(string jdata);

        public abstract void SetData(Dictionary<string, object> dic);

        public abstract void UpdateData();

    }

    public class ImageCompMapper : BaseMapper
    {
        public string sImage;
        public int color;

        protected Image imageComp;
        public ImageCompMapper()
        {

        }
        public ImageCompMapper(Component c)
            : base(c)
        {
            imageComp = comp as Image;

        }

        public override void SetData(Dictionary<string, object> dic)
        {
            object a;
            if (dic.TryGetValue("sImage", out a))
            {
                sImage = (string)a;
            }

            object b;
            if (dic.TryGetValue("color", out b))
            {
                color = (int)b;
            }
            UpdateData();
        }

        public override void UpdateData()
        {

            if (sImage != null)
            {
                imageComp.sprite = Resources.Load<Sprite>(sImage);
            }

            imageComp.color = MapperUtility.IntToColor(color);


        }

        public override string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public override void DeSerialize(string jdata)
        {
            ImageCompMapper jobj = JsonMapper.ToObject<ImageCompMapper>(jdata);
            sImage = jobj.sImage;
            color = jobj.color;

            UpdateData();
        }
    }


    public class TextCompMapper : BaseMapper
    {
        public string text;
        public int color;

        protected Text textComp;

        public TextCompMapper()
        {

        }

        public TextCompMapper(Component c)
            : base(c)
        {
            textComp = (Text)c;
        }

        public override void SetData(Dictionary<string, object> dic)
        {
            object a;
            if (dic.TryGetValue("text", out a))
            {
                text = (string)a;
            }

            object b;
            if (dic.TryGetValue("color", out b))
            {
                color = (int)b;
            }

            UpdateData();
        }

        public override void UpdateData()
        {
            textComp.text = text;
            textComp.color = MapperUtility.IntToColor(color);
        }

        public override string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public override void DeSerialize(string jdata)
        {
            TextCompMapper jobj = JsonMapper.ToObject<TextCompMapper>(jdata);

            text = jobj.text;
            color = jobj.color;
            UpdateData();
        }
    }

    public class ButtoCompMapper : BaseMapper
    {
        public string onEventClassName;
        public string onEventFuncName;

        protected Button buttonComp;

        public ButtoCompMapper()
        {

        }
        public ButtoCompMapper(Component c)
            : base(c)
        {
            buttonComp = (Button)c;
        }

        public override string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public override void DeSerialize(string jdata)
        {
            ButtoCompMapper jobj = JsonMapper.ToObject<ButtoCompMapper>(jdata);
            onEventClassName = jobj.onEventClassName;
            onEventFuncName = jobj.onEventFuncName;
        }

        public override void SetData(Dictionary<string, object> dic)
        {
            object a;
            if (dic.TryGetValue("onEventClassName", out a))
            {
                onEventClassName = (string)a;
            }

            object b;
            if (dic.TryGetValue("onEventFuncName", out b))
            {
                onEventFuncName = (string)b;
            }

            UpdateData();
        }

        public override void UpdateData()
        {
            Type t = Type.GetType(onEventClassName);
            if (t == null)
            {
                ADebug.Log("Can't Find Type" + onEventClassName);
            }
            else
            {
                Component funccomp = buttonComp.gameObject.GetComponent(t);
                if (funccomp == null)
                {
                    buttonComp.gameObject.AddComponent(t);
                }
                buttonComp.onClick.AddListener(delegate ()
                {
                    buttonComp.gameObject.SendMessage(onEventFuncName);
                });
            }
        }

    }

    public class SliderCompMapper : BaseMapper
    {
        public double value;
        public double minValue;
        public double maxValue;

        protected Slider slider;
        public SliderCompMapper()
        {

        }

        public SliderCompMapper(Component c)
            : base(c)
        {
            slider = (Slider)c;
        }

        public override string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public override void DeSerialize(string jdata)
        {
            SliderCompMapper jobj = JsonMapper.ToObject<SliderCompMapper>(jdata);
            value = jobj.value;
            minValue = jobj.minValue;
            maxValue = jobj.maxValue;
        }

        public override void SetData(Dictionary<string, object> dic)
        {
            object a;
            if (dic.TryGetValue("value", out a))
            {
                value = (double)a;
            }

            object b;
            if (dic.TryGetValue("minValue", out b))
            {
                minValue = (double)b;
            }

            object c;
            if (dic.TryGetValue("maxValue", out c))
            {
                maxValue = (double)c;
            }

            UpdateData();
        }

        public override void UpdateData()
        {
            slider.value = (float)value;
            slider.minValue = (float)minValue;
            slider.maxValue = (float)maxValue;
        }



    }

    public class ToggleCompMapper : BaseMapper
    {
        public bool isOn;

        protected Toggle toggleComp;

        public ToggleCompMapper()
        {

        }

        public ToggleCompMapper(Component c)
            : base(c)
        {
            toggleComp = (Toggle)c;
        }

        public override string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public override void DeSerialize(string jdata)
        {
            ToggleCompMapper jobj = JsonMapper.ToObject<ToggleCompMapper>(jdata);
            isOn = jobj.isOn;
        }

        public override void SetData(Dictionary<string, object> dic)
        {
            object a;
            if (dic.TryGetValue("text", out a))
            {
                isOn = (bool)a;
            }

            UpdateData();
        }

        public override void UpdateData()
        {
            toggleComp.isOn = isOn;
        }
    }



    // public class GridAdpterMapper:BaseMapper
    // {
    //   
    // }
    /*
    public class GridPackageMapper : BaseMapper
    {

        public List<GridAdpterMapper> gridAdpterMapper = new List<GridAdpterMapper>();

        protected LTGridPackage gridPackageComp; 


        public GridPackageMapper()
        {

        }

        public GridPackageMapper(Component c)
            : base(c)
        {
            gridPackageComp = (LTGridPackage)c;
        }


        public override string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public override void DeSerialize(string jdata)
        {
            ToggleCompMapper jobj = JsonMapper.ToObject<ToggleCompMapper>(jdata);
            isOn = jobj.isOn;
        }

        public override void SetData(Dictionary<string, object> dic)
        {
            object a;
            if (dic.TryGetValue("text", out a))
            {
                isOn = (bool)a;
            }

            UpdateData();
        }

        public override void UpdateData()
        {
            toggleComp.isOn = isOn;
        }


    }
    */
}

