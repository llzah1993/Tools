using UnityEngine;
using System.Collections;
using UnityEditor;
using Framework;

namespace Framework
{
    [CustomEditor(typeof(StarControl))]
    public class StarControlInspector : Editor
    {

        StarControl starControl;

        void OnEnable()
        {
            starControl = target as StarControl;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("刷新"))
            {
                starControl.General();
            }
        }
    }
}

