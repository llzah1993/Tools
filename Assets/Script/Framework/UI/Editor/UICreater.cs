using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Framework;

namespace Framework
{
    public static class UICreater
    {
        public static Vector2 designScreenSize = new Vector2(768, 1024);

        private const string uiPrefabResDir = "Prefabs/";

        // Fields
        private const string kBackgroundSpriteResourcePath = "UI/Skin/Background.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private const string kUILayerName = "UI";
        private const float kWidth = 160f;
        //private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        //private static Vector2 s_ImageGUIElementSize = new Vector2(100f, 100f);
        //private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color s_TextColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);
        //private static Vector2 s_ThickGUIElementSize = new Vector2(160f, 30f);
        //private static Vector2 s_ThinGUIElementSize = new Vector2(160f, 20f);

        [MenuItem("GameObject/UI/Extends/UIRoot")]
        public static void UIRoot(MenuCommand menuCommand)
        {
            GameObject obj = GameObject.Find("UIRoot(Clone)");
            if (obj == null)
            {
                GameObject root = Resources.Load("Prefabs/UIRoot") as GameObject;
                Transform tCamera = root.transform.Find("UICamera");
                CameraScaler cs = tCamera.GetComponent<CameraScaler>();
                PropertiesWindowsEditor.OpenWindow(cs, () => { GameObject.Instantiate(root); });
            }

        }


        [MenuItem("GameObject/UI/Extends/ListView")]
        public static void AddListView(MenuCommand menuCommand)
        {

            GameObject parent = Selection.activeGameObject;
            Object obj = Resources.Load(uiPrefabResDir + "ListView");
            GameObject listview = GameObject.Instantiate(obj) as GameObject;
            if (parent == null)
            {
                parent = GameObject.Find("UIRoot(Clone)");
                if (parent == null)
                {
                    Object root = Resources.Load("Prefabs/UIRoot");
                    parent = GameObject.Instantiate(root) as GameObject;
                }
            }
            listview.transform.parent = parent.transform;
        }

        [MenuItem("GameObject/UI/Extends/GridPackage")]
        public static void AddGridPackage(MenuCommand menuCommand)
        {

            GameObject parent = Selection.activeGameObject;
            GameObject gridObj = Resources.Load(uiPrefabResDir + "GridPackage") as GameObject;
            Image image = gridObj.GetComponent<Image>();
            LTGridPackage ltpackage = gridObj.GetComponent<LTGridPackage>();
            PropertiesWindowsEditor.OpenWindow(new Object[] { image, ltpackage },
            () =>
            {
                GameObject gridpackage = GameObject.Instantiate(gridObj) as GameObject;
                if (parent == null)
                {
                    parent = GameObject.Find("UIRoot(Clone)");
                    if (parent == null)
                    {
                        Object root = Resources.Load("Prefabs/UIRoot");
                        parent = GameObject.Instantiate(root) as GameObject;
                    }
                }
                gridpackage.transform.parent = parent.transform;

                RectTransform rect = gridpackage.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(designScreenSize.x, designScreenSize.y);
                gridpackage.transform.localPosition = Vector3.zero;
                gridpackage.transform.localScale = Vector3.one;
                gridpackage.transform.localEulerAngles = Vector3.zero;
            }
            );


        }

        [MenuItem("GameObject/UI/Extends/TabPage")]
        public static void AddTabPage(MenuCommand menuCommand)
        {
            GameObject parent = Selection.activeGameObject;
            Object obj = Resources.Load(uiPrefabResDir + "TabPage");
            GameObject gridpackage = GameObject.Instantiate(obj) as GameObject;
            if (parent == null)
            {
                parent = GameObject.Find("UIRoot(Clone)");
                if (parent == null)
                {
                    Object root = Resources.Load("Prefabs/UIRoot");
                    parent = GameObject.Instantiate(root) as GameObject;
                }
            }
            gridpackage.transform.parent = parent.transform;

            RectTransform rect = gridpackage.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(designScreenSize.x, designScreenSize.y);
            gridpackage.transform.localPosition = Vector3.zero;
            gridpackage.transform.localScale = Vector3.one;
            gridpackage.transform.localEulerAngles = Vector3.zero;
        }

        [MenuItem("GameObject/UI/Extends/StarControl")]
        public static void AddStarControl(MenuCommand menuCommand)
        {
            GameObject parent = Selection.activeGameObject;
            Object obj = Resources.Load(uiPrefabResDir + "StarControl");
            GameObject starControl = GameObject.Instantiate(obj) as GameObject;
            if (parent == null)
            {
                parent = GameObject.Find("UIRoot(Clone)");
                if (parent == null)
                {
                    Object root = Resources.Load("Prefabs/UIRoot");
                    parent = GameObject.Instantiate(root) as GameObject;
                }
            }
            starControl.transform.parent = parent.transform;

            //RectTransform rect = starControl.GetComponent<RectTransform>();
            starControl.transform.localPosition = Vector3.zero;
            starControl.transform.localScale = Vector3.one;
            starControl.transform.localEulerAngles = Vector3.zero;
        }

        [MenuItem("GameObject/UI/Extends/RichTextManager")]
        public static void AddRichTextControl(MenuCommand menuCommand)
        {
            GameObject parent = Selection.activeGameObject;
            GameObject obj = Resources.Load(uiPrefabResDir + "RichText") as GameObject;

            RichTextManager richTextComp = obj.GetComponent<RichTextManager>();
            PropertiesWindowsEditor.OpenWindow(richTextComp,
            () => {
                GameObject richText = GameObject.Instantiate(obj) as GameObject;
                if (parent == null)
                {
                    parent = GameObject.Find("UIRoot(Clone)");
                    if (parent == null)
                    {
                        Object root = Resources.Load("Prefabs/UIRoot");
                        parent = GameObject.Instantiate(root) as GameObject;
                    }
                }
                richText.transform.parent = parent.transform;

                //RectTransform rect = richText.GetComponent<RectTransform>();
                richText.transform.localPosition = Vector3.zero;
                richText.transform.localScale = Vector3.one;
                richText.transform.localEulerAngles = Vector3.zero;
            }
            );


        }

        [MenuItem("GameObject/UI/Extends/Attach/TweenAlpha")]
        public static void AddTweenAlpha()
        {
            //GameObject select = Selection.activeGameObject;
            //if(select != null)
            //{
            //    select.AddComponent<TweenAlpha>();
            //}

        }


        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            EventSystem system = Object.FindObjectOfType<EventSystem>();
            if (system == null)
            {
                GameObject child = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(child, parent);
                system = child.AddComponent<EventSystem>();
                child.AddComponent<StandaloneInputModule>();
                child.AddComponent<TouchInputModule>();
                Undo.RegisterCreatedObjectUndo(child, "Create " + child.name);
            }
            if (select && (system != null))
            {
                Selection.activeGameObject = system.gameObject;
            }
        }

        public static GameObject CreateNewUI()
        {
            GameObject objectToUndo = new GameObject("Canvas")
            {
                layer = LayerMask.NameToLayer("UI")
            };
            objectToUndo.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            objectToUndo.AddComponent<CanvasScaler>();
            objectToUndo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(objectToUndo, "Create " + objectToUndo.name);
            CreateEventSystem(false);
            return objectToUndo;
        }


        private static GameObject CreateUIElementRoot(string name, MenuCommand menuCommand, Vector2 size)
        {
            GameObject context = menuCommand.context as GameObject;
            if ((context == null) || (context.GetComponentInParent<Canvas>() == null))
            {
                context = GetOrCreateCanvasGameObject();
            }
            string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(context.transform, name);
            GameObject objectToUndo = new GameObject(uniqueNameForSibling);
            Undo.RegisterCreatedObjectUndo(objectToUndo, "Create " + uniqueNameForSibling);
            Undo.SetTransformParent(objectToUndo.transform, context.transform, "Parent " + objectToUndo.name);
            GameObjectUtility.SetParentAndAlign(objectToUndo, context);
            RectTransform itemTransform = objectToUndo.AddComponent<RectTransform>();
            itemTransform.sizeDelta = size;
            if (context != menuCommand.context)
            {
                SetPositionVisibleinSceneView(context.GetComponent<RectTransform>(), itemTransform);
            }
            Selection.activeGameObject = objectToUndo;
            return objectToUndo;
        }

        private static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject child = new GameObject(name);
            child.AddComponent<RectTransform>();
            GameObjectUtility.SetParentAndAlign(child, parent);
            return child;
        }

        public static GameObject GetOrCreateCanvasGameObject()
        {
            GameObject activeGameObject = Selection.activeGameObject;
            Canvas canvas = (activeGameObject == null) ? null : activeGameObject.GetComponentInParent<Canvas>();
            if ((canvas != null) && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if ((canvas != null) && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }
            return CreateNewUI();
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            lbl.color = s_TextColor;
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
            if ((lastActiveSceneView == null) && (SceneView.sceneViews.Count > 0))
            {
                lastActiveSceneView = SceneView.sceneViews[0] as SceneView;
            }
            if ((lastActiveSceneView != null) && (lastActiveSceneView.camera != null))
            {
                Vector2 vector;
                Camera cam = lastActiveSceneView.camera;
                Vector3 zero = Vector3.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2((float)(cam.pixelWidth / 2), (float)(cam.pixelHeight / 2)), cam, out vector))
                {
                    Vector3 vector3;
                    Vector3 vector4;
                    vector.x += canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                    vector.y += canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;
                    vector.x = Mathf.Clamp(vector.x, 0f, canvasRTransform.sizeDelta.x);
                    vector.y = Mathf.Clamp(vector.y, 0f, canvasRTransform.sizeDelta.y);
                    zero.x = vector.x - (canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x);
                    zero.y = vector.y - (canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y);
                    vector3.x = (canvasRTransform.sizeDelta.x * (0f - canvasRTransform.pivot.x)) + (itemTransform.sizeDelta.x * itemTransform.pivot.x);
                    vector3.y = (canvasRTransform.sizeDelta.y * (0f - canvasRTransform.pivot.y)) + (itemTransform.sizeDelta.y * itemTransform.pivot.y);
                    vector4.x = (canvasRTransform.sizeDelta.x * (1f - canvasRTransform.pivot.x)) - (itemTransform.sizeDelta.x * itemTransform.pivot.x);
                    vector4.y = (canvasRTransform.sizeDelta.y * (1f - canvasRTransform.pivot.y)) - (itemTransform.sizeDelta.y * itemTransform.pivot.y);
                    zero.x = Mathf.Clamp(zero.x, vector3.x, vector4.x);
                    zero.y = Mathf.Clamp(zero.y, vector3.y, vector4.y);
                }
                itemTransform.anchoredPosition = zero;
                itemTransform.localRotation = Quaternion.identity;
                itemTransform.localScale = Vector3.one;
            }
        }
    }
}


 
