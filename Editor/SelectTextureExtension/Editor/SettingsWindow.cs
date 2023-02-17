using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private bool IsToggleValue;
        public float mRectX;
        private static Rect WindowRect;
        private static int InstanceID;

        //private static Rect WindowRect;

        public static bool IsAllIsFalse;
        public List<TextureWrapMode> TextureWrapMode = new List<TextureWrapMode>();
        public static void Open(Rect r)
        {
            var go = GetWindow<SettingsWindow>(true,"设置");
            go.position = r;
            WindowRect =r;
            go.Show();
            
            //go.Show();
        }

        private void OnGUI()
        {
            IsToggleValue = EditorGUILayout.BeginFoldoutHeaderGroup(IsToggleValue, "界面设置");
            if (IsToggleValue)
            {

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("窗口背景颜色:");
                SelectTextureWindow.MyData.WindowBackgroundColor = EditorGUILayout.ColorField(SelectTextureWindow.MyData.WindowBackgroundColor);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("选择框颜色:");
                SelectTextureWindow.MyData.SelectColor = EditorGUILayout.ColorField(SelectTextureWindow.MyData.SelectColor);
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal("box");
                GUILayout.Label("贴图背景:");
                SelectTextureWindow.skin.customStyles[0].normal.background = EditorGUILayout.ObjectField(SelectTextureWindow.skin.customStyles[0].normal.background, typeof(Texture2D), false) as Texture2D;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("窗口背景图片:");
                SelectTextureWindow.MyData.WindowBackgroundTexture = EditorGUILayout.ObjectField(SelectTextureWindow.MyData.WindowBackgroundTexture, typeof(Texture2D), false) as Texture2D;
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUILayout.Button("Save"))
            {
                SelectTextureWindow.SaveData();
            }
            
        }
        void OnLostFocus()
        {
               // Open(WindowRect);
        }



    }
}