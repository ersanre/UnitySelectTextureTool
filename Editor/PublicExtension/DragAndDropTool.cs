
using UnityEditor;
using UnityEngine;

namespace EditorFramework
{
    public static class DragAndDropTool 
    {
        public class DragInof//为什么要是动态的，避免多次使用状态不对，使用的时候new一个
        {
            public  bool EnterArea;//翻译：输入区域
            public  bool Complete;//翻译：完成
            public  bool Dragging;//翻译：拖拽 
            public string[] Paths => DragAndDrop.paths;
            public object[] ObjectReferences => DragAndDrop.objectReferences;
            public DragAndDropVisualMode VisualMode => DragAndDrop.visualMode;
            public int ActiveControlID => DragAndDrop.activeControlID;
        }

        private static  DragInof mDragInof = new DragInof();
        
        public static DragInof Drag(Rect rect)
        {
            var e = Event.current;//现在的状态
            mDragInof.EnterArea  = rect.Contains(e.mousePosition);//如果 rect包含鼠标的位置,表示已经拖到目标区域了
            if (e.type == EventType.DragUpdated)//拖拽更新
            {
                mDragInof.Complete = false;
                mDragInof.Dragging = true;
                if (mDragInof.EnterArea)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;//鼠标拖拽东西到区域内鼠标样式会变化
                    e.Use();//使用掉事件，就是后面不检测了
                }
            }
            
            else if (e.type==EventType.DragPerform)//拖拽开始 ，开始生效
            {
                mDragInof.Complete = false;
                mDragInof.Dragging = true;
                DragAndDrop.AcceptDrag();//开始接受 拖拽
            }
            else if (e.type==EventType.DragExited)//拖拽退出
            {
                mDragInof.Complete = true;
                mDragInof.Dragging = false;
            }
            else
            {
                mDragInof.Complete = false;
                mDragInof.Dragging = true;
            }
            
            

            return mDragInof;
        }
    }
}