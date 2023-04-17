using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    public  class TextureSizePlugin : SelectTextureWindowPlugins
    {
        public static bool IsSDButtonValue;
        public static bool IsHDButtonValue;
        public static bool IsFHDButtonValue;
        static bool ISChang;
        public static int TempInt = 0;
        public Rect mRect;

        public override string PluginName { get => "ML_自定义贴图大小"; set => throw new System.NotImplementedException(); }
        public override string PluginTips { get => "根据规则快速选择筛选贴图大小"; set => throw new System.NotImplementedException(); }
        private bool _isEnlabe;
        public override bool IsEnlabe { get => _isEnlabe; set => _isEnlabe=value; }




        //  public Rect mRect;
        public override void Draw()
        {
            GUILayout.BeginArea(mRect);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            IsSDButtonValue = GUILayout.Toggle(IsSDButtonValue, "SD", "LargeButtonMid", GUILayout.ExpandWidth(false), GUILayout.Width(100));
            IsHDButtonValue = GUILayout.Toggle(IsHDButtonValue, "HD", "LargeButtonMid", GUILayout.ExpandWidth(false), GUILayout.Width(100));
            IsFHDButtonValue = GUILayout.Toggle(IsFHDButtonValue, "FHD", "LargeButtonMid", GUILayout.ExpandWidth(false), GUILayout.Width(100));
            ISChang = EditorGUI.EndChangeCheck();


            if (IsSDButtonValue && TempInt != 1)
            {
                TempInt = 1;
                IsHDButtonValue = false;
                IsFHDButtonValue = false;
                // SizeFilterPopupWindow<int>.PropetrtySelect(())
                for (int i = 0; i < SelectTextureWindow.MyData.TextureSizeTypes.Count; i++)
                {
                    SelectTextureWindow.MyData.TextureSizeTypes[SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i]] = SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i] <= 128 ? true : false;

                }
                SizeFilterPopupWindow<int>.IsAllIsFalse = false;
                SelectTextureWindow.RefreshFilter();
            };
            if (IsHDButtonValue && TempInt != 2)
            {
                TempInt = 2;
                IsSDButtonValue = false;
                IsFHDButtonValue = false;
                for (int i = 0; i < SelectTextureWindow.MyData.TextureSizeTypes.Count; i++)
                {
                    if (SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i] == 256)
                    {
                        SelectTextureWindow.MyData.TextureSizeTypes[SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i]] = true;
                    }
                    else
                    {
                        SelectTextureWindow.MyData.TextureSizeTypes[SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i]] = false;
                    }

                }
                SizeFilterPopupWindow<int>.IsAllIsFalse = false;
                SelectTextureWindow.RefreshFilter();
            };
            if (IsFHDButtonValue && TempInt != 3)
            {
                TempInt = 3;
                IsHDButtonValue = false;
                IsSDButtonValue = false;
                for (int i = 0; i < SelectTextureWindow.MyData.TextureSizeTypes.Count; i++)
                {
                    if (SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i] > 256)
                    {
                        SelectTextureWindow.MyData.TextureSizeTypes[SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i]] = true;
                    }
                    else
                    {
                        SelectTextureWindow.MyData.TextureSizeTypes[SelectTextureWindow.MyData.TextureSizeTypes.Keys.ToArray()[i]] = false;
                    }

                }
                SizeFilterPopupWindow<int>.IsAllIsFalse = false;
                SelectTextureWindow.RefreshFilter();
            };
            if (ISChang && !IsSDButtonValue && !IsHDButtonValue && !IsFHDButtonValue)
            {
                TempInt = 0;
                SizeFilterPopupWindow<int>.IsAllIsFalse = true;
                SelectTextureWindow.RefreshFilter();
            }
            //GUILayout.Space(50);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }


    }
}