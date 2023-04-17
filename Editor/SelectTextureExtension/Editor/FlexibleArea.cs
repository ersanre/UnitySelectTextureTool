using System.Collections;
using System.Collections.Generic;
using EditorFramework.Editor;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class FlexibleArea : GUIBase
    {
        public override Rect Rect { get => new Rect(0,0,0,0); set => base.Rect = value; }
        protected override void OnDispose()
        {

        }
    }
}