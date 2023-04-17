using System.Security.Cryptography;
using System.Reflection;
using System.IO;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private bool IsUISeting;
        private bool IsPluginSeting;
        public float mRectX;
        private static Rect WindowRect;
        private static int InstanceID;
        private static List<SelectTextureWindowPlugins> Plugins = new List<SelectTextureWindowPlugins>();
        private static List<bool> IsPluginsToggleValue = new List<bool>();

        //private static Rect WindowRect;

        public static bool IsAllIsFalse;
        public List<TextureWrapMode> TextureWrapMode = new List<TextureWrapMode>();
        public static void Open(Rect r)
        {
            var go = GetWindow<SettingsWindow>(true, "设置");
            go.position = r;
            WindowRect = r;
            go.Show();
            //go.Show();
        }

        private void OnGUI()
        {
            IsUISeting = EditorGUILayout.BeginFoldoutHeaderGroup(IsUISeting, "界面设置");
            if (IsUISeting)
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

            IsPluginSeting = EditorGUILayout.Foldout(IsPluginSeting, "插件");
            if (IsPluginSeting)
            {
                if (!Plugins.Equals(null) && Plugins.Count != 0)
                {
                    for (int i = 0; i < Plugins.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        IsPluginsToggleValue[i] = EditorGUILayout.Foldout(IsPluginsToggleValue[i], i + 1 + ". " + Plugins[i].PluginName);
                        Plugins[i].IsEnlabe = GUILayout.Toggle(Plugins[i].IsEnlabe, "开启");
                        GUILayout.EndHorizontal();
                        if (IsPluginsToggleValue[i])
                        {
                            GUILayout.Label(Plugins[i].PluginTips);
                        }
                    }

                }

                if (GUILayout.Button("重新加载插件"))
                {
                    Plugins.Clear();
                    IsPluginsToggleValue.Clear();
                    var types = TypeCache.GetTypesDerivedFrom<SelectTextureWindowPlugins>();
                    // var types = assembly.GetTypes();
                    for (int j = 0; j < types.Count; j++)
                    {
                        Plugins.Add(Activator.CreateInstance(types[j]) as SelectTextureWindowPlugins);
                        IsPluginsToggleValue.Add(false);
                    }
                    // Debug.Log(Path.GetFullPath(Data.GetDataPath().Replace(@"\Resources",string.Empty)));
                    // var files = Directory.GetFiles(Path.GetFullPath(Data.GetDataPath().Replace(@"\Resources",string.Empty)) , "*.cs");
                    // for (int i = 0; i < files.Length; i++)
                    // {

                    //    // var assembly = Assembly.LoadFile(files[i]);
                    //    // var types = assembly.GetTypes();
                    //     for (int j = 0; j < types.Count; j++)
                    //     {
                    //         if (types[j].GetInterface("ISelectTextureWindowPlug") != null)
                    //         {

                    //             Plugins.Add(types[j]);
                    //         }

                    //     }

                    // }
                }

            }

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