using System.Net;
using System.Collections;
using System.Collections.Generic;
using EditorFramework;
using EditorFramework.Editor;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class ToolbarArea : GUIBase
    {
        public override Rect Rect { get => new Rect(0, 0, 1000, 20); set => base.Rect = value; }
        private bool IsToolbarEnabled = true;
        private bool AddDirectoryToggleValue;
        public int selectedIndex;
        private List<string> NameList;
        private List<string> PathList;
        private Rect ToolbarRect;
        private static FoldeField foldeField;
        private Rect AddDirectoryRect;
        private static Rect[] ToolbarRects; //toolbar单个区域
        private static Rect[] ToolbarClosRects;
        public event Action IsListChange;
        public event Action IsListAdd;
        public event Action IsSelectChange;

        public event Action<int> IsListRemoveIndex;
        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            var e = Event.current;
            GUILayout.BeginArea(position);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUI.enabled = IsToolbarEnabled;
            selectedIndex = GUILayout.Toolbar(selectedIndex, NameList.ToArray(), EditorStyles.toolbarButton,
                GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100 * NameList.Count),
                GUILayout.MinWidth(40 * NameList.Count));
            if (Event.current.type == EventType.Repaint)
            {
                ToolbarRect = GUILayoutUtility.GetLastRect();
            }

            GUI.enabled = true;

            //  ToolbarRects=  ToolbarRect.Split(Names.Count,RectExtension.SplitType.Horizontal); //不好

            // +号
            AddDirectoryToggleValue = GUILayout.Toggle(AddDirectoryToggleValue,
                EditorGUIUtility.IconContent("d_CreateAddNew"), EditorStyles.toolbarButton, GUILayout.Width(20));

            if (foldeField.IsGetPath)
            {
                if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                {
                    AddDirectoryToggleValue = false;
                    foldeField.IsGetPath = false;
                    //截取文件夹名字
                    var name = Path.GetFileName(foldeField.Path);
                    NameList.Add(name);
                    PathList.Add(foldeField.Path);
                    IsListAdd?.Invoke();
                    IsListChange?.Invoke();
                    // SelectTextureWindow.SaveDataInAsset(foldeField.Path);
                }
            }

            if (AddDirectoryToggleValue)
            {
                foldeField.OnGUI(AddDirectoryRect);
            }

            var addRect = GUILayoutUtility.GetRect(20, 200, 20, 20, GUILayout.ExpandWidth(false));

            if (Event.current.type == EventType.Repaint)
            {
                AddDirectoryRect = addRect;
            }
            if (EditorGUI.EndChangeCheck())//增加删除，或者切换 都刷新筛选
            {
                IsSelectChange?.Invoke();
              //  SelectTextureWindow.RefreshFilter();
                //DrawTextures[selectedGroup].Load();//继续加载没加载的
            }
            //EditorGUI.BeginChangeCheck();

            //获取删除文件夹按钮的位置
            ToolbarRects = ToolbarRect.Split(NameList.Count, RectExtension.SplitType.Horizontal);
            ToolbarClosRects = new Rect[ToolbarRects.Length];
            for (int i = 0; i < ToolbarRects.Length; i++)
            {
                ToolbarClosRects[i] = ToolbarRects[i];
                ToolbarClosRects[i].x = ToolbarRects[i].xMax - 20;
                ToolbarClosRects[i].height = 20;
                ToolbarClosRects[i].width = 20;
            }

            if ((ToolbarRect.Contains(e.mousePosition) && e.clickCount == 2) || !IsToolbarEnabled) //双击出现
            {
                for (int i = 0; i < NameList.Count; i++)
                {

                    GUILayout.BeginArea(ToolbarRects[i]);
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    //ChangeName
                    NameList[i] = EditorGUILayout.DelayedTextField(NameList[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        IsListChange?.Invoke();
                    }
                    IsToolbarEnabled = false;
                    //删除文件夹按钮
                    if (GUILayout.Button("", "WinBtnClose"))
                    {
                       // NameList.RemoveAt(i);
                       // PathList.RemoveAt(i);
                        IsListRemoveIndex?.Invoke(i);
                        IsListChange?.Invoke();
                        //Undo.RecordObject(MyData,"数据");
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }
            }

            if (!(ToolbarRect.Contains(e.mousePosition)) && e.clickCount == 1)
            {
                IsToolbarEnabled = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        protected override void OnDispose()
        {

        }
        public ToolbarArea(List<string> names, List<string> paths, int IndexOf)
        {
            foldeField = new FoldeField();
            NameList = names;
            PathList = paths;
            selectedIndex =IndexOf;
        }
    }
}