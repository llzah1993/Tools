using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Framework;

namespace Framework
{
    public class PropertiesWindowsEditor : EditorWindow
    {

        //[MenuItem("GameObject/window")]
        //     static void test()
        //     {
        //         //创建窗口
        //         Rect wr = new Rect(0, 0, 500, 500);
        //         MyEditor window = (MyEditor)EditorWindow.GetWindowWithRect(typeof(MyEditor), wr, true, "widow name");
        //         window.target = (Selection.activeObject as GameObject).GetComponent<example>();
        //         window.Show();
        // 
        //     }

        public List<UnityEngine.Object> targetList = new List<UnityEngine.Object>();
        public Action CloseHandler = null;
        public string title1 = "--------------------------------属性配置------------------------------";
        protected static Rect wSize = new Rect(0, 0, 500, 500);

        /////////////////////////////////////////////////////////////////
        //Create Windows
        public static void OpenWindow(UnityEngine.Object target, Action closeHandler = null)
        {
            PropertiesWindowsEditor window = (PropertiesWindowsEditor)EditorWindow.GetWindowWithRect(typeof(PropertiesWindowsEditor), wSize, true, "属性编辑");
            window.targetList.Add(target);
            window.CloseHandler = closeHandler;
            window.Show();
        }

        public static void OpenWindow(UnityEngine.Object[] targets, Action closeHandler = null)
        {

            PropertiesWindowsEditor window = (PropertiesWindowsEditor)EditorWindow.GetWindowWithRect(typeof(PropertiesWindowsEditor), wSize, true, "属性编辑");
            window.targetList = new List<UnityEngine.Object>(targets);
            window.CloseHandler = closeHandler;
            window.Show();
        }
        /////////////////////////////////////////////////////////////////

        void OnGUI()
        {
            GUILayout.Label(title1);
            GUILayout.BeginScrollView(Vector2.zero);
            foreach (UnityEngine.Object target in targetList)
            {
                if (target != null)
                {

                    if (display_in_editor_special(target))
                    {
                        continue;
                    }

                    FieldInfo[] filelds = target.GetType().GetFields();
                    foreach (FieldInfo filed in filelds)
                    {
                        if (filed.IsDefined(typeof(DisplayAttribute), false))
                        {
                            object[] atts = filed.GetCustomAttributes(typeof(DisplayAttribute), false);
                            foreach (var att in atts)
                            {
                                DisplayAttribute dis_att = att as DisplayAttribute;
                                if (dis_att != null)
                                {
                                    display_in_editor(target, dis_att, filed);
                                }

                            }
                        }

                    }

                    PropertyInfo[] propertyInfos = target.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertyInfos)
                    {
                        if (pi.IsDefined(typeof(DisplayAttribute), false))
                        {
                            object[] atts = pi.GetCustomAttributes(typeof(DisplayAttribute), false);
                            foreach (var att in atts)
                            {
                                DisplayAttribute dis_att = att as DisplayAttribute;
                                if (dis_att != null)
                                {
                                    display_in_editor(target, dis_att, pi);
                                }

                            }
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        public bool display_in_editor_special(UnityEngine.Object target)
        {
            if (target.GetType() == typeof(Image))
            {
                Image image = (Image)target;
                image.sprite = (Sprite)EditorGUILayout.ObjectField("图片", image.sprite, typeof(Sprite), true);
                image.color = EditorGUILayout.ColorField("颜色", image.color);

                return true;
            }
            return false;
        }

        public void display_in_editor(UnityEngine.Object target, DisplayAttribute dis_att, FieldInfo filed)
        {

            string name = "";
            if (dis_att.DisplayName == "")
            {
                name = filed.Name;
            }
            else
            {
                name = dis_att.DisplayName;
            }

            if (dis_att.type == typeof(string))
            {
                var old_value = filed.GetValue(target) as string;
                string obj = EditorGUILayout.TextField(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type.IsEnum)
            {
                var old_value = filed.GetValue(target) as System.Enum;
                System.Enum obj = EditorGUILayout.EnumPopup(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type == typeof(int))
            {
                int old_value = (int)filed.GetValue(target);
                int obj = EditorGUILayout.IntField(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type == typeof(double))
            {
                double old_value = (double)filed.GetValue(target);
                double obj = EditorGUILayout.DoubleField(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type == typeof(float))
            {
                float old_value = (float)filed.GetValue(target);
                float obj = EditorGUILayout.FloatField(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type == typeof(bool))
            {
                bool old_value = (bool)filed.GetValue(target);
                bool obj = EditorGUILayout.Toggle(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type == typeof(Color))
            {
                Color old_value = (Color)filed.GetValue(target);
                Color obj = EditorGUILayout.ColorField(name, old_value);
                filed.SetValue(target, obj);
            }
            else if (dis_att.type == typeof(Vector2))
            {

                Vector2 old_value = (Vector2)filed.GetValue(target);
                Vector2 obj = EditorGUILayout.Vector2Field(name, old_value);
                filed.SetValue(target, obj);

            }
            else if (dis_att.type == typeof(Vector3))
            {

                Vector3 old_value = (Vector3)filed.GetValue(target);
                Vector3 obj = EditorGUILayout.Vector3Field(name, old_value);
                filed.SetValue(target, obj);

            }
            else
            {
                var old_value = filed.GetValue(target) as UnityEngine.Object;
                var obj = EditorGUILayout.ObjectField(name, old_value, dis_att.type, true);
                filed.SetValue(target, obj);
            }

        }

        public void display_in_editor(UnityEngine.Object target, DisplayAttribute dis_att, PropertyInfo pi)
        {

            string name = "";
            if (dis_att.DisplayName == "")
            {
                name = pi.Name;
            }
            else
            {
                name = dis_att.DisplayName;
            }

            if (dis_att.type == typeof(string))
            {

                var old_value = pi.GetValue(target, null) as string;
                string obj = EditorGUILayout.TextField(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type.IsEnum)
            {
                var old_value = pi.GetValue(target, null) as System.Enum;
                System.Enum obj = EditorGUILayout.EnumPopup(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type == typeof(int))
            {
                int old_value = (int)pi.GetValue(target, null);
                int obj = EditorGUILayout.IntField(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type == typeof(double))
            {
                double old_value = (double)pi.GetValue(target, null);
                double obj = EditorGUILayout.DoubleField(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type == typeof(float))
            {
                float old_value = (float)pi.GetValue(target, null);
                float obj = EditorGUILayout.FloatField(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type == typeof(bool))
            {
                bool old_value = (bool)pi.GetValue(target, null);
                bool obj = EditorGUILayout.Toggle(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type == typeof(Color))
            {
                Color old_value = (Color)pi.GetValue(target, null);
                Color obj = EditorGUILayout.ColorField(name, old_value);
                pi.SetValue(target, obj, null);
            }
            else if (dis_att.type == typeof(Vector2))
            {

                Vector2 old_value = (Vector2)pi.GetValue(target, null);
                Vector2 obj = EditorGUILayout.Vector2Field(name, old_value);
                pi.SetValue(target, obj, null);

            }
            else if (dis_att.type == typeof(Vector3))
            {

                Vector3 old_value = (Vector3)pi.GetValue(target, null);
                Vector3 obj = EditorGUILayout.Vector3Field(name, old_value);
                pi.SetValue(target, obj, null);

            }
            else
            {
                var old_value = pi.GetValue(target, null) as UnityEngine.Object;
                var obj = EditorGUILayout.ObjectField(name, old_value, dis_att.type, true);
                pi.SetValue(target, obj, null);
            }

        }


        void OnInspectorUpdate()
        {
            this.Repaint();
        }


        void OnDestroy()
        {
            if (CloseHandler != null)
            {
                CloseHandler();
            }
        }
    }
}
