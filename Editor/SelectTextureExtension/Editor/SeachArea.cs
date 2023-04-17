using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using EditorFramework.Editor;
using UnityEditor;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class SearchArea : GUIBase
    {

        public event Action<List<string>> SearchHistoryIsChange;
        public event Action<string> SearchTextIsChange;
        public static string SearchString;
        public  List<string> SearchHistory = new List<string>();
        //public static bool IsTextFieldChange;
        private Stopwatch sw = new Stopwatch(); //搜索历史的计时器
        private  Rect mRect=new Rect(0,0,600,20);
        public override Rect Rect { get => mRect; set => mRect = value; }

        private Rect SeachRect { get { return new Rect(this.mPosition.x + 5, this.mPosition.x + 2, 200, 20); } }

        private Rect SeachCancelRect { get { return new Rect(SeachRect.xMax + 5, SeachRect.y, 20, 20); } }
        //搜索历史
        private Rect SeachStringsRect { get { return new Rect(SeachCancelRect.x + SeachCancelRect.width, SeachCancelRect.y, 350, 20); } }


        /// <summary>
        /// 绘制搜索框
        /// </summary>
        public override void OnGUI(Rect rect)
        {
            #region 搜索
            EditorGUI.BeginChangeCheck();
            SearchString = EditorGUI.TextField(SeachRect, SearchString, new GUIStyle("ToolbarSeachTextField"));
            if (!string.IsNullOrEmpty(SearchString))
            {
                if (GUI.Button(SeachCancelRect, "", "ToolbarSeachCancelButton"))
                {
                    SearchString = "";
                    SearchTextIsChange?.Invoke(SearchString);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                SearchTextIsChange?.Invoke(SearchString);
                SelectTextureWindow.RefreshFilter();

                if (!string.IsNullOrEmpty(SearchString))
                {
                    sw.Start();
                }
                else
                {
                    sw.Stop();
                    sw.Reset();
                }

            }
            //搜索历史
            if (sw.Elapsed >= TimeSpan.FromSeconds(2.0) && !string.IsNullOrEmpty(SearchString))
            {
                AddSeachList(SearchString);
                SearchHistoryIsChange?.Invoke(SearchHistory);
                sw.Stop();
                sw.Reset();
            }

            GUILayout.BeginArea(SeachStringsRect);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < SearchHistory.Count; i++)
            {
                if (GUILayout.Button(SearchHistory[i], "sv_label_" + i % 7, GUILayout.MaxWidth(40)))
                {
                    SearchString = SearchHistory[i];
                    SearchTextIsChange?.Invoke(SearchString);
                 //   IsTextFieldChange = true;
                    SelectTextureWindow.RefreshFilter();
                }

                if (GUILayout.Button("", "ToolbarSeachCancelButton", GUILayout.Width(10)))
                {
                    SearchHistory.RemoveAt(i);
                    SearchHistoryIsChange?.Invoke(SearchHistory);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();



            #endregion
        }
         void AddSeachList(string SeachStrin)
        {
            if (SearchHistory.Count >= 5) //数量限制
            {
                SearchHistory.RemoveAt(0);
            }
            if (SearchHistory.Contains(SeachStrin)) //有没有一样的
            {
                SearchHistory.RemoveAt(SearchHistory.IndexOf(SeachStrin)); //移除之前一样的
                SearchHistory.Add(SeachStrin); //再加进去 （移到最后
            }
            else
            {
                SearchHistory.Add(SeachStrin);
            }
        }

        protected override void OnDispose()
        {

        }
    }

}