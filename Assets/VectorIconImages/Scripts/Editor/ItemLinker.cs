﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
 
 namespace VectorIconImages
{
    public class ItemLinker : Editor
    {
        [MenuItem("GameObject/UI/Vector Icon")]
        public static void CreateVectorIcon()
        {
            var p = GetOrCreateCanvasGameObject().transform;

            var obj = new GameObject("Icon");
            obj.transform.SetParent(p, false);
            obj.layer = LayerMask.NameToLayer("UI");
            var vi = obj.AddComponent<VectorImage>();

            Font font = Definitions.Font;

            vi.font = font;
            vi.iconName= Data.IconData.ElementAt(0).Key;
            vi.text= char.ConvertFromUtf32(
                int.Parse(Data.IconData[vi.iconName], System.Globalization.NumberStyles.HexNumber));
            vi.size = 40f;
            vi.color= Color.gray;
        }

        public static GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return selectedGo;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        private static GameObject CreateNewUI()
        {
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            CreateEventSystem(false, null);
            return root;
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }
    }
}