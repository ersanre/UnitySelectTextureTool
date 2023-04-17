using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using EditorFramework.Editor;
using UnityEditor;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    interface IRectHeight
    {
        List<GUIBase> Content { get; set; }
    }

    public class MainArea : GUIBase
    {
        //往下，从左往右排序， 给个样式
        //Rect 在ongui的时候再给
        // protected override float Height{get;set;}
        protected List<GUIBase> _myContent = new List<GUIBase>();
        public enum Arrangement
        {
            Horizontal,
            Vertical
        }
        private Arrangement _arrangment;
        private GUIStyle gUIStyle;
        //是否自动填充，横向或纵向填充
       // public bool IsAutoFill;



        public MainArea(Arrangement arrangement, GUIStyle gUIStyle)
        {
            _arrangment = arrangement;
            this.gUIStyle = gUIStyle;
        }

        public virtual List<GUIBase> Content { get => _myContent; set => _myContent = value; }
        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            GUI.Box(position, "", gUIStyle);
            if (this.Content.Count == 0)
            {
                return;
            }

            var allHeight = 0f;
            var rList = new List<int>();
            Rect TempRect;
            //垂直排序
            if (_arrangment == Arrangement.Vertical)
            {

                for (int j = 0; j < Content.Count; j++)
                {
                    //没有设置高度 就自动控制高度
                    if (Content[j].Rect.height==0)
                    {
                       // var mainArea = Content[j] as MainArea;
                            rList.Add(j);
                    }
                    else
                    {
                        allHeight += Content[j].Rect.height;
                    }
                }
                var autoHright = (this.mPosition.height - allHeight) / rList.Count;
                var Height = position.y;

                for (int i = 0; i < Content.Count; i++)
                {
                    if (rList.Count != 0 && rList.Contains(i))
                    {
                        TempRect = new Rect(position.x, Height, position.width, autoHright);
                    }
                    else
                    {
                        TempRect = new Rect(position.x, Height, position.width, Content[i].Rect.height);
                    }

                    //var tempRect = new Rect(0, height, position.width, Content[i].Rect.height);


                    // Content[i].Rect=tempRect;
                    Content[i].OnGUI(TempRect);
                    Height += TempRect.height;
                }
            }
            else
            {

                for (int j = 0; j < Content.Count; j++)
                {
                    if (Content[j].Rect.width==0)
                    {
                            rList.Add(j);
                    }
                    else
                    {
                        allHeight += Content[j].Rect.width;
                    }
                }
                var autoHright = (this.mPosition.width - allHeight) / rList.Count;
                var Height = position.x;

                for (int i = 0; i < Content.Count; i++)
                {
                    if (rList.Count != 0 && rList.Contains(i))
                    {
                        TempRect = new Rect(Height, position.y, autoHright, position.height);
                    }
                    else
                    {
                        TempRect = new Rect(Height, position.y, Content[i].Rect.width, position.height);
                    }

                    //var tempRect = new Rect(0, height, position.width, Content[i].Rect.height);


                    // Content[i].Rect=tempRect;
                    Content[i].OnGUI(TempRect);
                    Height += TempRect.width;
                }
            }
        }

        protected override void OnDispose()
        {
            //firstLineArea.OnCstomDraw -= DrawToolbarArea;
        }


    }
}
