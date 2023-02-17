using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class SizeFilterPopupWindow:PopupWindowContent 
    {
        public float mRectX;
        public TextureGroupBox mTextureGroupBox;
        private bool ToggleValue;
        private int SelectSize;
       // public event Action<int> SelectSizeTypeEvent;

        public static bool IsAllIsFalse;

        public List<int> TextureSize = new List<int>();

        public override void OnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical();
            for (int i = 0; i < TextureSize.Count; i++)
            {
              
              SelectTextureWindow.MyData.TextureSizeTypes [TextureSize[i]]=  GUILayout.Toggle( SelectTextureWindow.MyData.TextureSizeTypes[TextureSize[i]],TextureSize[i].ToString());
              
            }
            
           // SelectTextureWindow.MyData.TextureSizeTypes.Values.All(p => p);
            if (SelectTextureWindow.MyData.TextureSizeTypes.ContainsValue(true))
            {
                IsAllIsFalse = false;
            }
            else
            {
                IsAllIsFalse = true;
            }
            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                SelectTextureWindow.RefreshFilter();//刷新筛选
            }
            
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(mRectX,TextureSize.Count*17+5) ;
        }
    }
}