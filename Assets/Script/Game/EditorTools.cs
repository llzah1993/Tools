using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;

#endif
public class EditorTools  
{

    public static void ExportMesh(Mesh meshf)
    {
        #if UNITY_EDITOR
                if (meshf != null) {
                    string exportpath = "Assets/Meshes/" + meshf.name + ".asset";
                    Mesh newmesh = new Mesh();
                    EditorUtility.CopySerialized(meshf, newmesh);
                    AssetDatabase.CreateAsset(newmesh, exportpath);
                }
     #endif
    }
}
