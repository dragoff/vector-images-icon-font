using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

#pragma warning disable 0618

namespace VectorIconImages
{
    public class IconPickerWindow : EditorWindow
    {
        private static readonly int ICON_SIZE = 60;
        private static readonly int ICONS_ON_PAGE = 90;
        private static int elementsInLine = 4;


        static IconPickerWindow selfW;
        static GUIStyle iconStyle;
        static GUIStyle textStyle;


        private string selected = "";
        private System.Action<string, string> onSelectionChanged;
        private SearchField searchField;

        private List<string> tempList;
        
        Vector2 scrollRect;
        int selectedPage =-1;

        public static void Show(string preSelect, System.Action<string, string> callback)
        {
            selfW = GetWindow<IconPickerWindow>(true);

            selfW.title = "Pick an Icon";
            selfW.minSize = new Vector2(410, 560);
            selfW.maxSize = new Vector2(1000, 960);

            iconStyle = new GUIStyle();
            iconStyle.font = Definitions.Font;
            iconStyle.alignment = TextAnchor.MiddleCenter;
            iconStyle.normal.textColor = Color.gray;
            iconStyle.fontSize = ICON_SIZE;
            

            textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.normal.textColor = Color.gray;
            textStyle.fontSize = 12;
            

            selfW.onSelectionChanged = callback;
            selfW.selected = preSelect;
            selfW.ShowUtility();
            
        }

        private void OnEnable() => searchField = searchField ?? new SearchField();

        private void OnGUI()
        {
            if (selfW == null)
            {
                Close();
                return;
            }
            Rect windowRect = selfW.position;
            elementsInLine =(int) windowRect.size.x / 100;
            GUILayout.Space(10);
            DrawSearchField();

            GUILayout.Space(20);
            EditorGUI.DrawRect(new Rect(0, 60, windowRect.width, windowRect.height - 60), new Color(0.9f,0.9f,0.9f));;

            DrawIconsList();

        }

        #region PRIVATE METHODS
        
        private string DecodeUnicodeString(string s) => System.Text.RegularExpressions.Regex.Unescape(@"\u" + s);

        void DrawIconsList()
        {
            if (Data.IconData.Keys.Count == 0)
            {
                GUILayout.Label("- - Empty - -", textStyle);
                return;
            }

            if (string.IsNullOrEmpty(searchField.SearchString))
            {
                tempList = new List<string>();
                tempList.AddRange(Data.IconData.Keys);
            }
            else if (searchField.IsChanged)
            {
                tempList = new List<string>();
                selectedPage = 0;
                foreach (var name in Data.IconData.Keys)
                {
                    if (name.IndexOf(searchField.SearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                        tempList.Add(name);
                }
            }
            if (tempList.Count == 0)
            {
                GUILayout.Label("- - Empty - -", textStyle);
                return;
            }
            //PAGE CONTROLLER
            if (selectedPage == -1)
            {
                var numb = tempList.FindIndex((x) => x.Equals(selected));
                selectedPage = numb / ICONS_ON_PAGE;
            }
            
            string[] pages;
            int pageCount = tempList.Count / 90 + ((tempList.Count % ICONS_ON_PAGE)!=0 ? 1 : 0) ;
            pages = new string[pageCount];
            for (int i = 0; i < pageCount; i++)
                pages[i] = $"{i+1}";
            
            int from = ICONS_ON_PAGE * selectedPage;
            int to = ICONS_ON_PAGE * (selectedPage+1);
            if(to > tempList.Count) to= tempList.Count;
            
            GUILayout.BeginVertical();
            if(pages.Length>1)
                selectedPage = GUILayout.Toolbar(selectedPage, pages);
            GUILayout.EndVertical();
            //DRAW IMAGES
            ushort counter = 0;
            GUILayout.BeginVertical();
            scrollRect = EditorGUILayout.BeginScrollView(scrollRect);
            {
                for (var index = from; index < to; index++)
                {
                    var key = tempList[index];
                    string name2display = key.Length > 9 ? key.Substring(0, 9) : key;
                    string decoded = DecodeUnicodeString(Data.IconData[key]);

                    if (counter % elementsInLine == 0) GUILayout.BeginHorizontal();

                    if (key != selected)
                    {
                        if (DrawIconRect(name2display, decoded))
                        {
                            selected = key;
                            onSelectionChanged(key, decoded);
                        }
                    }
                    else if (DrawIconRect(name2display, decoded, true))
                        Close();

                    counter++;
                    if (counter % elementsInLine == 0)
                        GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                }
                if (counter % elementsInLine != 0) GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

        }
        private bool DrawIconRect(string name, string text, bool selected = false)
        {
            if(selected)
                GUILayout.BeginVertical(new GUIStyle {normal = {background = Texture2D.whiteTexture}},GUILayout.Width(ICON_SIZE+20));
            else
                GUILayout.BeginVertical(GUILayout.Width(ICON_SIZE+20));

            bool select = GUILayout.Button(text,iconStyle);
            GUILayout.Label(name, textStyle);
            GUILayout.EndVertical();
            return select;
        }
        #endregion

        #region SEARCH FIELD
        private void DrawSearchField()
        {            
            GUILayout.Label("Search", EditorStyles.boldLabel);
            searchField.OnGUI();
        }
        private class SearchField
        {
            public string SearchString { get; private set; } = "";
            public string PreviousSearch { get; private set; } = "";
                
            public bool IsChanged => SearchString != PreviousSearch;
        
            UnityEditor.IMGUI.Controls.SearchField searchField;
            public void OnGUI() => Draw();
        
            void Draw()
            {
                var rect = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();
                DoSearchField(rect);
                GUILayout.EndHorizontal();
                rect.y += 18;
            }
        
            void DoSearchField(Rect rect)
            {
                searchField = searchField?? new UnityEditor.IMGUI.Controls.SearchField();
                
                PreviousSearch = SearchString;
                SearchString = searchField.OnGUI(rect, SearchString);
        
                if (HasSearchbarFocused())
                {
                    RepaintFocusedWindow();
                }
            }
        
            bool HasSearchbarFocused() => GUIUtility.keyboardControl == searchField.searchFieldControlID;
        
            static void RepaintFocusedWindow()
            {
                if (EditorWindow.focusedWindow != null)
                {
                    EditorWindow.focusedWindow.Repaint();
                }
            }
        }

        #endregion
    }
}
