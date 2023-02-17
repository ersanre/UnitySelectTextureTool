using System;
using Assets.EditorFramework.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorFramework.Editor
{
    public class SplitView : GUIBase
    {
        //声明调整大小的回调事件
        public event Action OnBeginResize, OnEndResize;
        
        public event Action<Rect> FirstArea, SecondArea;
        public static float SplitSize = 200;
        public float MinSplitWidth = 100;
        public float padding = 8;
        private bool mResizing;
        public bool Dragging
        {
            get { return mResizing; }
            set
            {
                if (mResizing != value)//外部状态不等于这个状态是进来，代表状态更新
                {
                    mResizing = value;
                    if (value) 
                    {
                        if (OnBeginResize != null)
                        {
                            OnBeginResize();
                        }
                    }
                    else
                    {
                        if (OnEndResize != null)
                        {
                            OnEndResize();
                        }
                    }
                }
            }
        }
        public RectExtension.SplitType mSplitType;

        /// <summary>
        /// 传入分割的类型是垂直分割还是水平分割，
        /// </summary>
        /// <param name="splitType"></param>分割类型
        // public SplitView(RectExtension.SplitType splitType)
        // {
        //     mSplitType = splitType;
        // }

        public override void OnGUI(Rect position)
        {
            Rect[] rects = position.Split(mSplitType, SplitSize, padding);
            
            if (FirstArea != null) //绘制里面的内容，绘制方法外面写
            {
                FirstArea(rects[0]); //直接调用里面的方法（方法写在外面） 后面括号是参数
            }

            if (SecondArea != null)
            {
                SecondArea(rects[1]); //直接调用里面的方法（方法写在外面） 后面括号是参数
            }

            var mid = rects.GetMidTowRect(mSplitType);
            EditorGUI.DrawRect(mid.Zoom(-4f, RectExtension.AnchorType.MiddleCenter), Color.grey);

            Event e = Event.current;

            if (mid.Contains(e.mousePosition)) //鼠标样式
            {
                if (mSplitType == RectExtension.SplitType.Vertical)
                {
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeHorizontal);
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeVertical);
                }
            }

            switch (e.type) //拖动改变x轴
            {
                case EventType.MouseDown:
                    if (mid.Contains(e.mousePosition))
                    {
                        Dragging = true;
                    }

                    break;
                case EventType.MouseDrag:
                    if (Dragging)
                    {
                        SplitSize += e.delta.x
                                     + e.delta.y;

                        //限制窗口最小
                        SplitSize = mSplitType == RectExtension.SplitType.Vertical
                            ? Mathf.Clamp(SplitSize, rects[0].xMin + MinSplitWidth, rects[1].xMax - MinSplitWidth)
                            : Mathf.Clamp(SplitSize, rects[0].yMin + MinSplitWidth, rects[1].yMax - MinSplitWidth);
                        e.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (Dragging)
                    {
                        Dragging = false;
                    }

                    break;
            }
        }

        protected override void OnDispose()
        {
            FirstArea = null;
            SecondArea = null;
            OnBeginResize = null;
            OnEndResize = null;
        }
    }
}