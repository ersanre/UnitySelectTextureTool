using System;
using System.Collections;
using System.Collections.Generic;
using EditorFramework.Editor;
using UnityEditor;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{

    public class Filter<T> : GUIBase
    {
        private Rect mRect = new Rect(0, 0, 80, 20);
        public string Label;
        private string TempLabel;
        public SizeFilterPopupWindow<T> MyPopupWindowContent;
        public List<T> ToggleTepyList = new List<T>();
        public override Rect Rect { get => mRect; set => mRect = value; }
        public event Action IsButtonClick;
        public event Action IsToggleChange;
        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);

            if (GUI.Button(position, TempLabel, "ToolbarDropDownToggle"))
            {

                MyPopupWindowContent = new SizeFilterPopupWindow<T>(ToggleTepyList, position.width);
                IsButtonClick?.Invoke();
                MyPopupWindowContent.IsToggleChange -= ChangeLabel;
                MyPopupWindowContent.IsToggleChange += ChangeLabel;
                MyPopupWindowContent.IsToggleChange += SendEvent;
                PopupWindow.Show(position, MyPopupWindowContent);
            }

        }

        private void SendEvent()
        {
            IsToggleChange?.Invoke();
        }

        public Filter(string label, List<T> toggleTepyList)
        {
            this.ToggleTepyList = toggleTepyList;
            this.Label = label;
            TempLabel = label;

        }
         public Filter()
        {
        }


        private void ChangeLabel()
        {
            var i = 0;
            foreach (var item in SizeFilterPopupWindow<T>.PropetrtySelect)
            {

                if (item.Value)
                {
                    TempLabel = item.Key.ToString();
                    i++;
                }
                if (i == 2)
                {
                    TempLabel += "...";
                    return;
                }
            }
            if (i == 0)
            {
                TempLabel = Label;
            }
        }

        protected override void OnDispose()
        {
            // MyPopupWindowContent.IsToggleChange -= IsToggleChange;
            MyPopupWindowContent.IsToggleChange -= ChangeLabel;
        }
    }
}