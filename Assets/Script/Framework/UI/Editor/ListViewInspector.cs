using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using Framework;

namespace Framework
{
    [CustomEditor(typeof(ListView), true), CanEditMultipleObjects]
    public class ListViewInspector : GridLayoutGroupEditor
    {

        //private SerializedProperty mname;
        //private SerializedProperty cellprefab;
        protected override void OnEnable()
        {
            //mname = serializedObject.FindProperty("cellname");
            //cellprefab = serializedObject.FindProperty("cellprefab");
            base.OnEnable();


        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

    }
}


