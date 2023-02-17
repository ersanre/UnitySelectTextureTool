using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class ModeFilterPopupWindow:PopupWindowContent 
    {
        public float mRectX;
        public TextureGroupBox mTextureGroupBox;
        private bool ToggleValue;
        private int SelectSize;
      
        public static bool IsAllIsFalse;
        public List<TextureWrapMode> TextureWrapMode = new List<TextureWrapMode>();
        
        public override void OnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical();
   
            for (int i = 0; i < TextureWrapMode.Count; i++)
            {
                SelectTextureWindow.MyData.TextureWrapModes[TextureWrapMode[i]]=  GUILayout.Toggle(  SelectTextureWindow.MyData.TextureWrapModes[TextureWrapMode[i]],TextureWrapMode[i].ToString());
            }
            GUILayout.EndVertical();
            if (SelectTextureWindow.MyData.TextureWrapModes.ContainsValue(true))
            {
                IsAllIsFalse = false;
            }
            else
            {
                IsAllIsFalse = true;
            }
            if (EditorGUI.EndChangeCheck())
            {
                SelectTextureWindow.RefreshFilter();//刷新筛选
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(mRectX,TextureWrapMode.Count*17+5) ;
        }
    }
}