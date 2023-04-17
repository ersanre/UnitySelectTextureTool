using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorFramework.Editor;
using UnityEditor;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    public abstract class TextureTools : GUIBase
    {
        public bool ToggleValue { set; get; }
        protected abstract GUIContent ToogleContent { set; get; }
        public Texture MyTexture;
        GUIStyle myGUIStyle = new GUIStyle("AppToolbarButtonLeft");
        protected bool IsToggleChange { set; get; }
        public void ShowToggle()
        {
            EditorGUI.BeginChangeCheck();
            ToggleValue = GUILayout.Toggle(ToggleValue, ToogleContent, myGUIStyle, GUILayout.Height(20), GUILayout.Width(24));
            IsToggleChange = EditorGUI.EndChangeCheck();
            if (IsToggleChange)
            {
                if (ToggleValue)
                {
                    //初始化
                    OnEnable();
                }
                else
                {
                    OnDispose();
                }
            }

        }

        protected override void OnDispose()
        {
        }
        public virtual void OnEnable()
        {
           // myGUIStyle =
        }

    }
}