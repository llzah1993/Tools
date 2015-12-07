using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using Framework;

namespace Framework
{
    [CustomEditor(typeof(LTGridPackage))]
    public class LTGridPackageInspector : Editor
    {
        LTGridPackage gridPackage;
        int tDataCount = 3;
        void OnEnable()
        {
            gridPackage = target as LTGridPackage;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.BeginVertical();


            gridPackage.packageSize = EditorGUILayout.Vector2Field("宽高", gridPackage.packageSize);

            gridPackage.row = EditorGUILayout.IntField("行", gridPackage.row);
            gridPackage.column = EditorGUILayout.IntField("列", gridPackage.column);
            EditorGUILayout.LabelField("单元大小");
            EditorGUILayout.LabelField("Width:" + gridPackage.cellSize.x + "           Height:" + gridPackage.cellSize.y);
            gridPackage.spaceSize = EditorGUILayout.Vector2Field("单元间隔", gridPackage.spaceSize);
            gridPackage.cellItem = EditorGUILayout.ObjectField("单元展示", gridPackage.cellItem, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            gridPackage.viewfillCell = EditorGUILayout.Toggle("填充测试", gridPackage.viewfillCell);

            if (GUILayout.Button("刷新"))
            {
                RectTransform gridPackageRect = gridPackage.GetComponent<RectTransform>();
                gridPackageRect.sizeDelta = gridPackage.packageSize;

                //Vector3 parentpos = gridPackage.transform.position;
                Vector2 packageSize = gridPackage.packageSize;
                Vector2 spaceSize = gridPackage.spaceSize;
                RectTransform scrollrect = gridPackage.scrollView.GetComponent<RectTransform>();
                scrollrect.sizeDelta = new Vector2(packageSize.x * tDataCount, packageSize.y);
                scrollrect.localPosition = new Vector3(packageSize.x * (tDataCount - 1) * 0.5f, 0, 0);

                if (gridPackage.viewfillCell)
                {
                    gridPackage.FilleDataInEditor();
                }

                LTGridPageItem[] pageItems = gridPackage.pageItem;
                for (int i = 0; i < pageItems.Length; i++)
                {
                    LTGridPageItem pageItem = pageItems[i];

                    RectTransform pageItemtrans = pageItem.gameObject.GetComponent<RectTransform>();
                    pageItemtrans.sizeDelta = gridPackage.packageSize;



                    GridLayoutGroup glayout = pageItem.gameObject.GetComponent<GridLayoutGroup>();


                    int row = gridPackage.row;
                    int column = gridPackage.column;
                    float x = (packageSize.x - spaceSize.x * (column - 1)) / column;
                    float y = (packageSize.y - spaceSize.y * (row - 1)) / row;
                    gridPackage.cellSize = glayout.cellSize = new Vector2(x, y);

                    glayout.spacing = gridPackage.spaceSize;
                    glayout.padding = new RectOffset();



                    pageItemtrans.localPosition = new Vector3(packageSize.x * (1 - tDataCount) * 0.5f + (i - 1) * packageSize.x, 0, 0);

                    glayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    glayout.startAxis = GridLayoutGroup.Axis.Vertical;
                    glayout.childAlignment = TextAnchor.UpperLeft;
                    glayout.constraint = GridLayoutGroup.Constraint.Flexible;


                    glayout.CalculateLayoutInputHorizontal();
                    glayout.CalculateLayoutInputVertical();

                }
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}


