using System;
using UnityEditor;
using UnityEngine;
using static EditorFramework.RectExtension;

namespace EditorFramework.Editor
{
    public class SplitView : GUIBase
    {
        //声明调整大小的回调事件
        public event Action OnBeginResize, OnEndResize;
        public event Action<Rect> DrawDragAndDrropRect;

        public event Action<Rect> FirstArea, SecondArea;
        public static float SplitSize;
        public float MinSplitWidth = 100;
        public float padding = 8;
        private bool mResizing;
        private AutoFillRect autoFillRect;
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
        public SplitView(RectExtension.SplitType splitType, AutoFillRect autoFill = AutoFillRect.firstRect)
        {
            mSplitType = splitType;
            autoFillRect = autoFill;
        }

        public override void OnGUI(Rect position)
        {
            // if (position.height<=SplitSize+MinSplitWidth)
            // {
            //  SplitSize= SplitSize-(SplitSize+MinSplitWidth- position.height);
            // }
            var temp = position.height - SplitSize;
            Rect[] rects = position.Split(mSplitType, SplitSize, padding, true, autoFillRect);

            if (FirstArea != null) //绘制里面的内容，绘制方法外面写
            {
                FirstArea(rects[0]); //直接调用里面的方法（方法写在外面） 后面括号是参数
            }

            if (SecondArea != null)
            {
                SecondArea(rects[1]); //直接调用里面的方法（方法写在外面） 后面括号是参数
            }

            var mid = rects.GetMidTowRect(mSplitType);
            if (DrawDragAndDrropRect != null)
            {
                DrawDragAndDrropRect(mid);
            }
            // EditorGUI.DrawRect(mid.Zoom(-4, AnchorType.MiddleCenter), Color.green);

            Event e = Event.current;

            if (mid.Contains(e.mousePosition)) //鼠标样式
            {
                if (mSplitType == RectExtension.SplitType.Vertical)
                {

                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeVertical);
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeHorizontal);
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
                        if (mSplitType == RectExtension.SplitType.Vertical)
                        {
                            SplitSize = autoFillRect == AutoFillRect.firstRect ? SplitSize + e.delta.y : SplitSize - e.delta.y;

                           // SplitSize += e.delta.y;
                            //限制窗口最小

                            SplitSize = Mathf.Clamp(SplitSize, 0 + MinSplitWidth,position.height - MinSplitWidth);
                        }
                        else
                        {
                             SplitSize = autoFillRect == AutoFillRect.firstRect ? SplitSize + e.delta.x : SplitSize - e.delta.x;
                           // SplitSize += e.delta.x;
                            //限制窗口最小
                            SplitSize = Mathf.Clamp(SplitSize, rects[0].xMin + MinSplitWidth, rects[1].xMax - MinSplitWidth);
                        }


                        //限制窗口最小

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