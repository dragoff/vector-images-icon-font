using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VectorIconImages
{
    public class IconCssManipulator : EditorWindow
    {
        private static readonly char[] suffix = new char[] {':', '{', '}'};

        private static string[] cssStrings;
        private static string prefix;

        [MenuItem("Tools/Vector Icons/Load font with style.css file", false, 15)]
        public static void OpenCssFile()
        {
            string cssFile = EditorUtility.OpenFilePanel("Select .css file", Application.dataPath, "css");
            if (string.IsNullOrEmpty(cssFile)) return;
            cssStrings = File.ReadAllLines(cssFile);
            //CREATE PREFIX WINDOW
            IconCssManipulator window = CreateInstance<IconCssManipulator>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 50);
            window.ShowUtility();
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Type your Prefix:", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            prefix = EditorGUILayout.TextField(prefix);
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Dataset"))
            {
                CreateCodepointsFile();
                Close();
            }

            if (GUILayout.Button("Cancel")) Close();
            EditorGUILayout.EndHorizontal();
        }

        private static string ParseString(string text, string startAt, char[] stopAt)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int startIndex = text.IndexOf(startAt, StringComparison.Ordinal);
                if (startIndex < 0) return string.Empty;
                text = text.Substring(startIndex + startAt.Length);

                int[] stopIndex = new int[stopAt.Length];
                for (int i = 0; i < stopAt.Length; i++)
                {
                    int index = text.IndexOf(stopAt[i]);
                    stopIndex[i] = index < 0 ? int.MaxValue : index;
                }

                if (startIndex != stopIndex.Min())
                    return text.Substring(0, stopIndex.Min());
            }

            return string.Empty;
        }

        void CreateCodepointsFile()
        {
            string content = "";

            string temp = "";
            foreach (string s in cssStrings)
            {
                if (s.Contains($".{prefix}"))
                {
                    temp = "";
                    temp += ParseString(s, $".{prefix}", suffix);
                }

                if (s.Contains("content:"))
                {
                    string tempcodes = ParseString(s, "\"", new char[] {'\"'}).Trim('/', '\\');
                    if (string.IsNullOrEmpty(tempcodes))
                        tempcodes = ParseString(s, "\'", new char[] {'\''}).Trim('/', '\\');
                    if (string.IsNullOrEmpty(tempcodes))
                        continue;
                    temp += " " + tempcodes;
                    content += temp + '\n';
                }
            }

            var path = Application.dataPath + "/VectorIconImages/Font/codepoints";
            System.IO.File.WriteAllText(path, content);
            IconDataManipulator.CreateIconDataset(path);
        }
    }
}