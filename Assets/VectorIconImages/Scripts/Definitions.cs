#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VectorIconImages
{
    public static class Definitions
    {
        private static readonly string FONT_NAME = "VectorFont";
        private static Font m_Font;
        public static string FontName => FONT_NAME;

        public static Font Font
        {
            get
            {
                if (m_Font != null) return m_Font;
                var fontGUID = AssetDatabase.FindAssets(FONT_NAME)[0];
                m_Font =  AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));
                return m_Font;
            }
        }
    }
}
#endif