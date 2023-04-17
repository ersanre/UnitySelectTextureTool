using System.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using EditorFramework;
using static EditorFramework.RectExtension;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    // public delegate void TextrueChangeEventHandler(DrawTextureGroup drawTextureGroup,
    //     TextureChangeEventArgs textureChangeEventArgs);
    public class DrawTextureGroup
    {
        public Vector2 ScrollViewValue = new Vector2(0f, 0f);
        public float pix;
        private int textindex;
        private int start;
        private int end;
        private int cunstomWidth = 10;
        private int cunstomHeight = 10;
        private int darwHeight;

        public int SelectIndex;
        private Rect lowRect;
        private Rect topRect;
        private Rect mouseRect;
        private Rect selectRect;
        private bool IsSelectionChange;
        public List<Texture> textures = new List<Texture>();
        private int textrueArrayLength;
        private bool IsHover = true;
        private Rect LastRect;
        private Rect DragRect;
        private int HoverControlID;
        private int HoverIndex;
        //   private static GUIStyle mEditorStyles = new GUIStyle("CommandMid");

        public GetTextureList getTextureList;

        private string Path;

        public bool IsLoad;


        // private bool[] isSelection;
        private List<bool> isSelection;
        // public  Material Material;
        // public string PropertyName;
        private Texture Bg;

        // public static event Action tc;

        public int Length = 100;

        // private int zhenLength;
        // public int TexturesLength;

        public List<TextureBox> NowTextureBoxs;
        //private TextureBoxs[] NowTextureBoxs;
        public static event Action<TextureBox> IsTextureChange;

        public Dictionary<int, bool> SelectIsSizType = new Dictionary<int, bool>();
        public Dictionary<TextureWrapMode, bool> SelectIsTextureWrapMode = new Dictionary<TextureWrapMode, bool>();

        public bool IsDraging { get; private set; }
        private Vector2 Offset;

        public DrawTextureGroup(string path)
        {
            Path = path;
            //Load();


            getTextureList = new GetTextureList();
            //getTextureList.DrawTextureGroup = this;

            //  mEditorStyles = new GUIStyle("TimeScrubberButton");
            SelectTextureWindow.skin.customStyles[0].fixedWidth = SelectTextureWindow.MyData.TextureSize;
            SelectTextureWindow.skin.customStyles[0].fixedHeight = SelectTextureWindow.MyData.TextureSize;
            //  mEditorStyles.margin = new RectOffset(2, 2, 2, 2);
            // mEditorStyles.normal.background=  AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/YaoZiTools/SelectTextureExtension/C_FX_T_23_skin03_06.png");

            NowTextureBoxs = getTextureList.TextureBoxs;
            //  Bg = SelectTextureWindow.MyData.WindowBackgroundTexture;
            // IsLoad = true;

        }


        public void Draw(Rect TexturesRect)
        {
            // TexturesRect.y -= 40; //??????????????
            // TexturesRect.x -= 2; //??????????????
            // GUI.color =SelectTextureWindow.MyData.WindowBackgroundColor;
            var e = Event.current;
            if (SelectTextureWindow.MyData.WindowBackgroundTexture != null)
            {
                GUI.DrawTexture(TexturesRect, SelectTextureWindow.MyData.WindowBackgroundTexture, ScaleMode.ScaleAndCrop);//背景图
            }
            EditorGUI.DrawRect(TexturesRect, SelectTextureWindow.MyData.WindowBackgroundColor);

            //没筛选时用最开始获得的长度，筛选时用实时长度， 这样刚打开滚动条不会 随着图片加载缩短
            var nowLength = NowTextureBoxs.Count == getTextureList.TextrueArrayLength ? getTextureList.TextrueArrayLength : NowTextureBoxs.Count;
            if (nowLength == 0)
            {
                return;
            }

            darwHeight = Mathf.CeilToInt(((float)nowLength) / cunstomWidth); //真实绘制行数


            cunstomWidth = Mathf.FloorToInt((TexturesRect.width - 15) /
                                           SelectTextureWindow.MyData
                                               .TextureSize); //一行多少个 -1 可以让左右的滑动条不出现，不会出现多渲染的情况  取整有可能是0
            SelectTextureWindow.skin.customStyles[0].margin.left = (int)(((TexturesRect.width - 15) % SelectTextureWindow.MyData.TextureSize) / cunstomWidth);
            // SelectTextureWindow.skin.customStyles[0].fixedWidth=SelectTextureWindow.skin.customStyles[0].fixedHeight;

            cunstomHeight = Mathf.CeilToInt(TexturesRect.height / SelectTextureWindow.MyData.TextureSize) >
                           nowLength / cunstomWidth
                ? Mathf.CeilToInt((float)nowLength / cunstomWidth) + 1
                : Mathf.CeilToInt(TexturesRect.height / SelectTextureWindow.MyData.TextureSize) + 1; //有多少行  多渲染一行,不能铺满屏幕时，按少的来

            //   var tempPixe = Mathf.FloorToInt(((TexturesRect.width - cunstomWidth * SelectTextureWindow.MyData.TextureSize - mEditorStyles.margin.right - 15) / cunstomWidth));
            //  mEditorStyles.margin.left = tempPixe;

            var BigRect = new Rect(TexturesRect.x, TexturesRect.y, TexturesRect.width - 20,
                (darwHeight + 1) * SelectTextureWindow.MyData.TextureSize);
            ScrollViewValue = GUI.BeginScrollView(TexturesRect, ScrollViewValue, BigRect);
            //var tRect = GUILayoutUtility.GetRect(1, ((int)(ScrollViewValue.y / SelectTextureWindow.MyData.TextureSize) * SelectTextureWindow.MyData.TextureSize)); //空的
            GUILayout.Space(((int)(ScrollViewValue.y / SelectTextureWindow.MyData.TextureSize) * SelectTextureWindow.MyData.TextureSize));
            start = Mathf.FloorToInt(ScrollViewValue.y / SelectTextureWindow.MyData.TextureSize) *
                    cunstomWidth; //开始渲染索引,确保是整数*每行个数

            end = start + cunstomWidth * cunstomHeight;
            end = Mathf.Clamp(end, 0, nowLength); //为什么限制


            //Debug.Log(EditorWindow.focusedWindow== );

            GUILayout.BeginVertical();

            for (int i = start; i < end; i += 0)
            {
                if (i >= nowLength && !(Event.current.type == EventType.Layout))
                {
                    break;
                }
                GUILayout.BeginHorizontal();
                for (int j = 0; j < cunstomWidth; j++) //渲染横向图片
                {

                    if (i >= nowLength)
                    {
                        if (i >= nowLength && !(Event.current.type == EventType.Layout))
                        {
                            break;
                        }
                        break;
                    }
                    IsSelectionChange = getTextureList.TextureBoxs[i].IsSelect;
                    // var controlID = GUIUtility.GetControlID(FocusType.Passive);
                    var textureRect = GUILayoutUtility.GetRect(SelectTextureWindow.MyData.TextureSize, SelectTextureWindow.MyData.TextureSize, SelectTextureWindow.skin.customStyles[0]);
                    HoverControlID = GUIUtility.GetControlID(FocusType.Passive);
                    if (textureRect.Contains(e.mousePosition))
                    {
                        mouseRect = textureRect;
                        //  Debug.Log( i);
                    }

                    //图片拖放功能
                    /*
                    switch (e.GetTypeForControl(HoverControlID))
                    {
                        case EventType.MouseDown:
                            break;

                        case EventType.MouseDrag:
                            if (textureRect.Contains(e.mousePosition)&&TexturesRect.Contains(e.mousePosition))
                            {
                                DragAndDrop.PrepareStartDrag();
                                DragAndDrop.objectReferences = new Texture[] { NowTextureBoxs[i].Texture };
                                // DragAndDrop.paths = new String[] {"Assets/YaoZiTools/Editor/SelectTextureExtension/Common/Special/FHD/Special_FHD_lj_00018.png"};
                                // DragAndDrop.SetGenericData(nameof(Texture), NowTextureBoxs[i].Texture);
                                DragAndDrop.StartDrag("Texture");
                                IsDraging = true;
                                DragRect = textureRect;
                                Offset = textureRect.position - e.mousePosition;
                                e.Use();
                            }

                            break;
                        case EventType.DragUpdated:
                            GUI.Label(new Rect(e.mousePosition.x, e.mousePosition.y, SelectTextureWindow.MyData.TextureSize, SelectTextureWindow.MyData.TextureSize), " NowTextureBoxs[i].Texture");
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                            DragRect.position = Offset + e.mousePosition;
                            e.Use();
                            break;
                        case EventType.DragPerform:
                            DragAndDrop.AcceptDrag();
                            IsDraging = false;
                            e.Use();
                            break;
                        case EventType.DragExited:
                            GUIUtility.hotControl = 0;
                            IsDraging = false;
                            e.Use();
                            break;

                    }
*/

                    // if (textureRect.Contains(e.mousePosition) && Event.current.type == EventType.MouseDrag)
                    // {

                    //     Debug.Log("MouseDown" + i);
                    //     // Debug.Log(HoverIndex);
                    //     //  Debug.Log(e.delta);
                    //     DragAndDrop.PrepareStartDrag();
                    //     DragAndDrop.objectReferences = new UnityEngine.Object[] { NowTextureBoxs[HoverIndex].Texture };
                    //     DragAndDrop.paths = new String[] { NowTextureBoxs[HoverIndex].TexturePath };
                    //     DragAndDrop.SetGenericData(nameof(Texture2D), NowTextureBoxs[HoverIndex].Texture);
                    //     DragAndDrop.StartDrag("Texture");

                    //     IsHover = true;
                    //     Debug.Log("MouseDown" + i);
                    // }
                    //  getTextureList.TextureBoxs[i].IsSelect = GUILayout.Toggle(getTextureList.TextureBoxs[i].IsSelect, NowTextureBoxs[i].Texture, SelectTextureWindow.skin.customStyles[0]);
                    getTextureList.TextureBoxs[i].IsSelect = GUI.Toggle(textureRect, getTextureList.TextureBoxs[i].IsSelect, NowTextureBoxs[i].Texture, SelectTextureWindow.skin.customStyles[0]);

                    // if (Event.current.type == EventType.Repaint)
                    // {

                    //     LastRect = GUILayoutUtility.GetLastRect();
                    // }

                    //悬浮状态位置


                    //选中状态位置
                    if (getTextureList.TextureBoxs[i].IsSelect)
                    {
                        selectRect = LastRect;
                    }

                    if (EditorWindow.focusedWindow != null && e.alt && EditorWindow.focusedWindow.ToString().Contains("SelectTextureWindow"))
                    {

                        LastRect.x += 2;
                        LastRect.y = LastRect.y + LastRect.height - 17;
                        LastRect.width = 20;
                        LastRect.height = 15;
                        var sizeRect = new Rect(LastRect);
                        sizeRect.x += 22;
                        sizeRect.width = 36;

                        // GUI.Label(lRect,NowTextureBoxs[i].Texture.wrapMode.ToString().ToUpper()[0].ToString(),"AssetLabel Partial") ;

                        if (NowTextureBoxs[i].Texture.wrapMode.ToString() == "Clamp")
                        {
                            GUI.Label(LastRect, "C", "AssetLabel Partial");
                        }
                        else
                        {
                            GUI.Label(LastRect, "R", "AssetLabel Partial");
                        }

                        //贴图maxsize
                        GUI.Label(sizeRect, (NowTextureBoxs[i].Texture.width > NowTextureBoxs[i].Texture.height ? NowTextureBoxs[i].Texture.width : NowTextureBoxs[i].Texture.height).ToString(), "AssetLabel Partial");



                    }




                    // getTextureList.isSelect[i] = GUI.Toggle(r, getTextureList.isSelect[i],
                    //     NowTextureBoxs[i].Texture, mEditorStyles); //渲染图片Toggle

                    // GUI.Label(r,
                    //     (sizeTextureBoxs[i].Texture.height > sizeTextureBoxs[i].Texture.width
                    //         ? sizeTextureBoxs[i].Texture.height
                    //         : sizeTextureBoxs[i].Texture.width).ToString());
                    // GUI.DrawTexture(r,textures[i]);

                    // if (getTextureList.isSelect[i])
                    // {
                    //     GUILayout.Label("", "IN ThumbnailSelection");
                    // }

                    if (getTextureList.TextureBoxs[i].IsSelect != IsSelectionChange) //单选
                    {

                        IsTextureChange?.Invoke(NowTextureBoxs[i]);

                        getTextureList.TextureBoxs[SelectIndex].IsSelect = false;
                        SelectIndex = i;
                        getTextureList.TextureBoxs[i].IsSelect = true;
                        // getTextureList.isSelect[i] = true;
                    }


                    i++;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            //框
            if (TexturesRect.Contains(e.mousePosition - ScrollViewValue) && EditorWindow.focusedWindow.ToString().Contains("SelectTextureWindow"))
            {
                DarwLine(mouseRect, 4f, new Color(0, 0.6f, 1, 0.7f));
            }

            DarwLine(selectRect, 6, SelectTextureWindow.MyData.SelectColor);
            if (IsDraging)
            {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, 2000, 2000), MouseCursor.Zoom);

                GUI.DrawTexture(DragRect, (DragAndDrop.objectReferences[0]) as Texture);
            }

            // DarwLine(new Vector3(moseRect.x,moseRect.y),new Vector3(moseRect.xMax,moseRect.y),new Vector3(moseRect.xMax,moseRect.yMax),new Vector3(moseRect.x,moseRect.yMax));
            //Debug.Log("ID:" + LastControlID + "Type:" + e.GetTypeForControl(LastControlID));
            //  Debug.Log("id: " + GUIUtility.hotControl + "上次id" + LastControlID);
            SelectTextureWindow.skin.customStyles[0].fixedWidth = SelectTextureWindow.MyData.TextureSize;
            SelectTextureWindow.skin.customStyles[0].fixedHeight = SelectTextureWindow.MyData.TextureSize;
            GUI.EndScrollView();
        }

        public void Load()
        {
            // getTextureList = new GetTextureList();
            //getTextureList.DrawTextureGroup = this;
            if (getTextureList.TextrueArrayLength != this.NowTextureBoxs.Count || getTextureList.TextrueArrayLength == 0)
            {
                EditorCoroutineRunner.StartEditorCoroutine(getTextureList.GetAssetTextureInPath(Path));
            }

            // mEditorStyles = new GUIStyle("TimeScrubberButton");
            // SelectTextureWindow.skin.customStyles[0].fixedWidth = SelectTextureWindow.MyData.TextureSize;
            // SelectTextureWindow.skin.customStyles[0].fixedHeight = SelectTextureWindow.MyData.TextureSize;
            // mEditorStyles.margin = new RectOffset(2, 2, 2, 2);
            // // mEditorStyles.normal.background=  AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/YaoZiTools/SelectTextureExtension/C_FX_T_23_skin03_06.png");

            // NowTextureBoxs = getTextureList.TextureBoxs;
            // //  Bg = SelectTextureWindow.MyData.WindowBackgroundTexture;

            IsLoad = true;
        }

        public static void DarwLine(Rect rect, float width, Color color)
        {

            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawAAPolyLine(width, new Vector3(rect.x, rect.y), new Vector3(rect.xMax, rect.y));
            Handles.DrawAAPolyLine(width, new Vector3(rect.xMax, rect.y), new Vector3(rect.xMax, rect.yMax));
            Handles.DrawAAPolyLine(width, new Vector3(rect.xMax, rect.yMax), new Vector3(rect.x, rect.yMax));
            Handles.DrawAAPolyLine(width, new Vector3(rect.x, rect.yMax), new Vector3(rect.x, rect.y));
            Handles.EndGUI();
        }
    }
}