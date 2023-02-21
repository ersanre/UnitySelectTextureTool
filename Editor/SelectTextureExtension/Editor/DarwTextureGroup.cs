
using System.Security.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace YaoZiTools.SelectTextureExtension.Editor
{
    // public delegate void TextrueChangeEventHandler(DrawTextureGroup drawTextureGroup,
    //     TextureChangeEventArgs textureChangeEventArgs);
    public class DrawTextureGroup
    {
        public Vector2 ScrollViewValue = new Vector2(0f, 0f);
        public float pix;
        private int textindex;
        public int start;
        public int end;
        public int cunstomWidth = 10;
        public int cunstomHeight = 10;
        public int darwHeight;

        private int LastSelectIndex;
        public Rect lowRect;
        public Rect topRect;
        private Rect mouseRect;
        private Rect selectRect;
        private bool IsSelectionChange;
        public List<Texture> textures = new List<Texture>();
        private int textrueArrayLength;

        private static GUIStyle mEditorStyles = new GUIStyle("CommandMid");

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

        public List<TextureBoxs> NowTextureBoxs;
        //private TextureBoxs[] NowTextureBoxs;
        //public event TextrueChangeEventHandler TextrueChange; 

        public Dictionary<int, bool> SelectIsSizType = new Dictionary<int, bool>();
        public Dictionary<TextureWrapMode, bool> SelectIsTextureWrapMode = new Dictionary<TextureWrapMode, bool>();

        private Texture[] TempTexture;
        private int tempInt;

        public DrawTextureGroup(string path)
        {
            Path = path;
            //Load();


            getTextureList = new GetTextureList();
            //getTextureList.DrawTextureGroup = this;

            mEditorStyles = new GUIStyle("TimeScrubberButton");
            SelectTextureWindow.skin.customStyles[0].fixedWidth = SelectTextureWindow.MyData.TextureSize;
            SelectTextureWindow.skin.customStyles[0].fixedHeight = SelectTextureWindow.MyData.TextureSize;
            mEditorStyles.margin = new RectOffset(2, 2, 2, 2);
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
            var nowLength = NowTextureBoxs.Count == getTextureList.textrueArrayLength ? NowTextureBoxs.Count : getTextureList.textrueArrayLength;

            darwHeight = Mathf.CeilToInt(((float)nowLength) / cunstomWidth); //真实绘制行数


            cunstomWidth = Mathf.FloorToInt((TexturesRect.width - 15) /
                                           SelectTextureWindow.MyData
                                               .TextureSize); //一行多少个 -1 可以让左右的滑动条不出现，不会出现多渲染的情况  取整有可能是0
            SelectTextureWindow.skin.customStyles[0].margin.left = (int)(((TexturesRect.width - 15) % SelectTextureWindow.MyData.TextureSize) / cunstomWidth);
            // SelectTextureWindow.skin.customStyles[0].fixedWidth=SelectTextureWindow.skin.customStyles[0].fixedHeight;

            cunstomHeight = Mathf.CeilToInt(TexturesRect.height / SelectTextureWindow.MyData.TextureSize) >
                           nowLength / cunstomWidth
                ? Mathf.CeilToInt((float)nowLength / cunstomWidth)
                : Mathf.CeilToInt(TexturesRect.height / SelectTextureWindow.MyData.TextureSize); //有多少行  多渲染一行,不能铺满屏幕时，按少的来

            var tempPixe = Mathf.FloorToInt(((TexturesRect.width - cunstomWidth * SelectTextureWindow.MyData.TextureSize - mEditorStyles.margin.right - 15) / cunstomWidth));
            mEditorStyles.margin.left = tempPixe;

            var BigRect = new Rect(TexturesRect.x, TexturesRect.y, TexturesRect.width - 20,
                (darwHeight + 1) * SelectTextureWindow.MyData.TextureSize);
            ScrollViewValue = GUI.BeginScrollView(TexturesRect, ScrollViewValue, BigRect);

            // var tRect = GUILayoutUtility.GetRect(1, ScrollViewValue.y); //空的
            var tRect = GUILayoutUtility.GetRect(1, ((int)(ScrollViewValue.y / SelectTextureWindow.MyData.TextureSize) * SelectTextureWindow.MyData.TextureSize)); //空的

            start = Mathf.FloorToInt(ScrollViewValue.y / SelectTextureWindow.MyData.TextureSize) *
                    cunstomWidth; //开始渲染索引,确保是整数*每行个数

            end = start + cunstomWidth * cunstomHeight;
            end = Mathf.Clamp(end, 0, nowLength); //为什么限制


            //Debug.Log(EditorWindow.focusedWindow== );
            if (start + end != tempInt)
            {
                TempTexture = new Texture[cunstomWidth * cunstomHeight];

            }



            GUILayout.BeginVertical();

            for (int i = start; i < end; i += 0)
            {
                GUILayout.BeginHorizontal();

                for (int j = 0; j < cunstomWidth; j++) //渲染横向图片
                {
                    if (i >= nowLength)
                    {
                        break;
                        // GUI.Label(
                        //     GUILayoutUtility.GetRect(SelectTextureWindow.MyData.TextureSize,
                        //         SelectTextureWindow.MyData.TextureSize), "");
                        // i++;
                        // continue;
                    }

                    // var r = GUILayoutUtility.GetRect(SelectTextureWindow.MyData.TextureSize,
                    //     SelectTextureWindow.MyData.TextureSize);

                    // IsSelectionChange = getTextureList.isSelect[i];
                    if (start + end != tempInt)
                    {
                        TempTexture[i - start] = (AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(getTextureList.GUIDString[i])));
                    }




                    getTextureList.isSelect[i] = GUILayout.Toggle(getTextureList.isSelect[i], TempTexture[i - start], SelectTextureWindow.skin.customStyles[0]);

                    var lRect = GUILayoutUtility.GetLastRect();

                    if (lRect.Contains(e.mousePosition))
                    {
                        mouseRect = lRect;

                    }
                    if (getTextureList.isSelect[i])
                    {
                        selectRect = lRect;
                    }
                    // EditorGUI.DrawRect(lRect,new Color(1f,0f,0f,1f));

                    if (EditorWindow.focusedWindow != null && e.alt && EditorWindow.focusedWindow.ToString().Contains("SelectTextureWindow"))
                    {

                        lRect.x += 2;
                        lRect.y = lRect.y + lRect.height - 17;
                        lRect.width = 20;
                        lRect.height = 15;
                        var sizeRect = new Rect(lRect);
                        sizeRect.x += 22;
                        sizeRect.width = 36;

                        // GUI.Label(lRect,NowTextureBoxs[i].Texture.wrapMode.ToString().ToUpper()[0].ToString(),"AssetLabel Partial") ;

                        if (NowTextureBoxs[i].Texture.wrapMode.ToString() == "Clamp")
                        {
                            GUI.Label(lRect, "C", "AssetLabel Partial");
                        }
                        else
                        {
                            GUI.Label(lRect, "R", "AssetLabel Partial");
                        }

                        //贴图maxsize
                        GUI.Label(sizeRect, NowTextureBoxs[i].Texture.width > NowTextureBoxs[i].Texture.height ? NowTextureBoxs[i].Texture.width.ToString() : NowTextureBoxs[i].Texture.width.ToString().ToString(), "AssetLabel Partial");



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

                    if (getTextureList.isSelect[i] != IsSelectionChange) //单选 
                    {
                        Undo.RecordObject(SelectTextureWindow.MyData.NowMaterial, "修改材质贴图");//注册可撤销
                        SelectTextureWindow.MyData.NowMaterial.SetTexture(
                            SelectTextureWindow.MyData.NowTextruePropertyName,
                            NowTextureBoxs[i].Texture); //设置贴图

                        SelectTextureWindow.TexturePathInfo = AssetDatabase.GetAssetPath(NowTextureBoxs[i].Texture);
                        //Selection.activeObject = o[i].Texture;
                        // tc?.Invoke(); //触发事件
                        getTextureList.isSelect[LastSelectIndex] = false;
                        LastSelectIndex = i;
                        getTextureList.isSelect[i] = true;
                    }


                    i++;
                }

                GUILayout.EndHorizontal();
            }
            tempInt = start + end;
            GUILayout.EndVertical();
            //框
            if (TexturesRect.Contains(e.mousePosition - ScrollViewValue) && EditorWindow.focusedWindow.ToString().Contains("SelectTextureWindow"))
            {
                DarwLine(mouseRect, 4f, new Color(0, 0.6f, 1, 0.7f));
            }

            DarwLine(selectRect, 6, SelectTextureWindow.MyData.SelectColor);
            // DarwLine(new Vector3(moseRect.x,moseRect.y),new Vector3(moseRect.xMax,moseRect.y),new Vector3(moseRect.xMax,moseRect.yMax),new Vector3(moseRect.x,moseRect.yMax));

            SelectTextureWindow.skin.customStyles[0].fixedWidth = SelectTextureWindow.MyData.TextureSize;
            SelectTextureWindow.skin.customStyles[0].fixedHeight = SelectTextureWindow.MyData.TextureSize;
            GUI.EndScrollView();
        }

        public void Load()
        {
            // getTextureList = new GetTextureList();
            //getTextureList.DrawTextureGroup = this;
            if (getTextureList.textrueArrayLength != this.NowTextureBoxs.Count || getTextureList.textrueArrayLength == 0)
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