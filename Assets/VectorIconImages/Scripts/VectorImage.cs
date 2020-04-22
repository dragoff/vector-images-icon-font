using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace VectorIconImages
{
    public class VectorImage : Text
    {
        [SerializeField] private string m_iconName;

        public string iconName
        {
            get => m_iconName;
            set => m_iconName = value;
        }

        [SerializeField] private float m_Size = 10;

        public float size
        {
            get => m_Size;
            set
            {
                m_Size = value;
                UpdateScale();
            }
        }

        [SerializeField] private string m_ExtraName;
        [SerializeField] private string m_ExtraText;
        [SerializeField] private Color m_ExtraColor;
        [SerializeField] private float m_ExtraSize;


        [SerializeField] private bool m_isSwappable = false;
        protected override void Start()
        {
            base.Start();
#if UNITY_EDITOR
            font = Definitions.Font;
            if (string.IsNullOrEmpty(iconName))
            {
                iconName = Data.IconData.ElementAt(0).Key;
                text = CreateUnicode(Data.IconData[iconName]);
                color = Color.gray;
            }
#endif
        }

        #region Public Methods

        public void UpdateScale()
        {
            rectTransform.sizeDelta = new Vector2(m_Size, m_Size);
            fontSize = (int) m_Size;
        }

        public void ChangeIcon(string newName)
        {
            foreach (var key in Data.IconData.Keys)
            {
                if (key.Equals(newName))
                {
                    iconName = newName;
                    text = CreateUnicode(Data.IconData[key]);
                    return;
                }
            }
        }

        public void SwapIcon()
        {
            if(!m_isSwappable) return;
            (iconName, m_ExtraName) = (m_ExtraName, iconName);
            (text, m_ExtraText) = (m_ExtraText, text);
            (size, m_ExtraSize) = (m_ExtraSize, size);
            (color, m_ExtraColor) = (m_ExtraColor, color);
        }
        #endregion

        readonly UIVertex[] m_TempVerts = new UIVertex[4];

protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if ((UnityEngine.Object) this.font == (UnityEngine.Object) null)
                return;
            this.m_DisableFontTextureRebuiltCallback = true;
            this.cachedTextGenerator.PopulateWithErrors(System.Text.RegularExpressions.Regex.Unescape(text),
                this.GetGenerationSettings(this.rectTransform.rect.size), this.gameObject);
            IList<UIVertex> verts = this.cachedTextGenerator.verts;
            float num1 = 1f / this.pixelsPerUnit;
#if UNITY_2019_1_OR_NEWER
            int num2 = verts.Count;
#else
            int num2 = verts.Count-4;
#endif
            if (num2 <= 0)
            {
                toFill.Clear();
            }
            else
            {
                Rect inputRect = rectTransform.rect;
                Vector2 point = new Vector2(inputRect.xMin, inputRect.yMax);
                Vector2 vector2 = this.PixelAdjustPoint(point) - point;
                toFill.Clear();
                if (vector2 != Vector2.zero)
                {
                    for (int index1 = 0; index1 < num2; ++index1)
                    {
                        int index2 = index1 & 3;
                        this.m_TempVerts[index2] = verts[index1];
                        this.m_TempVerts[index2].position *= num1;
                        this.m_TempVerts[index2].position.x += vector2.x;
                        this.m_TempVerts[index2].position.y += vector2.y;
                        if (index2 == 3)
                            toFill.AddUIVertexQuad(this.m_TempVerts);
                    }
                }
                else
                {
                    for (int index1 = 0; index1 < num2; ++index1)
                    {
                        int index2 = index1 & 3;
                        this.m_TempVerts[index2] = verts[index1];
                        this.m_TempVerts[index2].position *= num1;
                        if (index2 == 3)
                            toFill.AddUIVertexQuad(this.m_TempVerts);
                    }
                }
                this.m_DisableFontTextureRebuiltCallback = false;
            } 
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (font == null) return;
            ChangeIcon(iconName);
            base.OnValidate();
            SetLayoutDirty();
        }
#endif
        private string CreateUnicode(string s) =>
            char.ConvertFromUtf32(int.Parse(s, System.Globalization.NumberStyles.HexNumber));

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateScale();
        }
    }
}
