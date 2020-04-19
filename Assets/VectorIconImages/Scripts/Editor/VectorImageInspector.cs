﻿using UnityEngine;
using UnityEditor;

 namespace VectorIconImages
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VectorImage))]
    public class VectorImageInspector : Editor
    {
        private SerializedProperty
            m_Size,
            m_Color,
            m_IsSwappable,
            m_ExtraName,
            m_ExtraText,
            m_ExtraSize,
            m_ExtraColor;

        private VectorImage _image;

        private static GUIStyle iconStyle;
        private static GUIStyle nameStyle;

        void OnEnable()
        {
            _image = target as VectorImage;
            _image.font = Definitions.Font;

            m_Size = serializedObject.FindProperty("m_Size");
            m_Color = serializedObject.FindProperty("m_Color");
            m_IsSwappable = serializedObject.FindProperty("m_isSwappable");
            //EXTRA
            m_ExtraSize = serializedObject.FindProperty("m_ExtraSize");
            m_ExtraColor = serializedObject.FindProperty("m_ExtraColor");
            m_ExtraName = serializedObject.FindProperty("m_ExtraName");
            m_ExtraText = serializedObject.FindProperty("m_ExtraText");


            //STYLES
            iconStyle = new GUIStyle();
            iconStyle.font = Definitions.Font;
            ;
            iconStyle.normal.textColor = Color.gray;
            iconStyle.fontSize = 60;
            iconStyle.alignment = TextAnchor.MiddleCenter;

            nameStyle = new GUIStyle();
            nameStyle.alignment = TextAnchor.UpperCenter;
            nameStyle.fontStyle = FontStyle.Bold;
        }

        private void InitExtraIcon()
        {
            m_ExtraSize.floatValue = _image.size;
            m_ExtraColor.colorValue = _image.color;
            m_ExtraName.stringValue = _image.iconName;
            m_ExtraText.stringValue = _image.text;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //MAIN
            GUILayout.BeginVertical();
            {
                EditorGUILayout.PropertyField(m_Size, new GUIContent("Size"));
                if (_image.size < 0) _image.size = 0;
                if (_image.size > 300) _image.size = 300;
                iconStyle.fontSize = (int) _image.size;
                _image.UpdateScale();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_Color, new GUIContent("Color"));
                iconStyle.normal.textColor = _image.color;
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(_image.text, iconStyle))
                    IconPickerWindow.Show(_image.iconName, (myname, unicode) =>
                    {
                        _image.text = unicode;
                        _image.iconName = myname;
                    });

                EditorGUILayout.LabelField(_image.iconName, nameStyle);
            }
            GUILayout.EndVertical();
            //EXTRA
            GUILayout.BeginHorizontal("Box");
            {
                m_IsSwappable.boolValue = EditorGUILayout.Foldout(m_IsSwappable.boolValue, new GUIContent("Extra Icon", "Enable Extra Icon"));
                //EditorGUILayout.PropertyField(m_IsSwappable, new GUIContent("Extra Icon", "Enable Extra Icon"));
                if(m_IsSwappable.boolValue)
                    if (GUILayout.Button("Swap", GUILayout.Width(100)))
                        _image.SwapIcon();

                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();
            if (m_IsSwappable.boolValue)
            {
                if(string.IsNullOrEmpty(m_ExtraText.stringValue)) InitExtraIcon();
                GUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(m_ExtraSize, new GUIContent("Size"));
                    if (_image.size < 0) _image.size = 0;
                    if (_image.size > 300) _image.size = 300;
                    iconStyle.fontSize = (int) m_ExtraSize.floatValue;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(m_ExtraColor, new GUIContent("Color"));
                    iconStyle.normal.textColor = m_ExtraColor.colorValue;
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button(m_ExtraText.stringValue, iconStyle))
                        IconPickerWindow.Show(m_ExtraName.stringValue, (newName, unicode) =>
                        {
                            m_ExtraText.stringValue = unicode;
                            m_ExtraName.stringValue = newName;
                            serializedObject.ApplyModifiedProperties();
                        });
                    EditorGUILayout.LabelField(m_ExtraName.stringValue, nameStyle);
                }
                GUILayout.EndVertical();
            }

            EditorUtility.SetDirty(_image);
            serializedObject.ApplyModifiedProperties();
        }
    }
}