using System.Globalization;
using System.Runtime.CompilerServices;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EditorFramework;
using UnityEditor;
using UnityEngine;
using EditorFramework.Editor;
using Debug = UnityEngine.Debug;
using static EditorFramework.RectExtension;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public struct TextureGroupBox //贴图的筛选状态
    {
        private Dictionary<int, int> SizeTypeLength;
        public Dictionary<int, bool> _selectIsTextureSizeType;

        public Dictionary<TextureWrapMode, bool> SelectIsTextureWrapMode;

        public Dictionary<int, bool> SelectIsTextureSizeType
        {
            set { value = _selectIsTextureSizeType; }

            get { return _selectIsTextureSizeType; }
        }
    }

    public class SelectTextureWindow : EditorWindow
    {
        public static List<DrawTextureGroup> DrawTextures = new List<DrawTextureGroup>(); //渲染图片的组

        private static int time;

        GUIContent SortIcon;
        private static bool IsSort;

        private Stopwatch sw = new Stopwatch(); //搜索历史的计时器

        private static Texture kuang; //自己的选中框

        private static FoldeField foldeField; //文件拖放识别窗口


        private Rect textrureRects = new Rect(1, 1, 5000, 5000);
        private Rect AddDirectoryRect;
        private static Rect ToolbarRect; //整块toolbar
        private static Rect[] ToolbarRects; //toolbar单个区域
        private static Rect[] ToolbarClosRects; //toolbar 的删除按钮位置


        public static Material Material;
        public static string PropertyName;

        public static SelectTextureWindowData MyData;

        private bool IsToolbarEnabled;
        private GUIStyle CloseButtonStyle;
        private GUIStyle PropertyButtonStyle;

        private bool SizeToggleValue;
        private bool wrapModeToggleValue;

        public TextureGroupBox mTextureGroupBox;

        private bool DebugToggle;

        public static List<string> TexturPropertyNames = new List<string>();
        public static Dictionary<string, bool> IsSelectTexture = new Dictionary<string, bool>();

        public static string SeachString;
        public static bool IsTextFieldChange;
        private bool IsSelectproperty;
        /// <summary>
        /// 贴图路径信息
        /// </summary>
        public static string TexturePathInfo;
        public static GUISkin skin;
        //搜索
        public Rect SeachRect = new Rect(5, 2, 200, 20);
        //贴图size 滑动条
        public Rect TextureSizeRect { get { return new Rect(position.width - 75, position.height - 18, 70, 20); } }
        //搜索关闭x按钮
        public Rect SeachCancelRect { get { return new Rect(SeachRect.xMax + 5, SeachRect.y, 20, 20); } }
        //搜索历史
        public Rect SeachStringsRect { get { return new Rect(SeachCancelRect.x + SeachCancelRect.width, SeachCancelRect.y, 350, 20); } }
        //文件夹栏 toolbar
        public Rect DirectoryRect { get { return new Rect(5, SeachRect.height, position.width, 20); } }
        //排序按钮
        public Rect sortRect { get { return new Rect(position.width - 220, SeachRect.y, 30, 20); } }
        //排序文字提示
        public Rect sortTipsRect { get { return new Rect(sortRect.x - 80, sortRect.y, 80, 20); } }
        public Rect tRecta { get { return new Rect(2, DirectoryRect.yMax, DirectoryRect.width, position.height - 60); } }
        public Rect textureInfo { get { return new Rect(0, position.height - 20, tRecta.width, 20); } }
        public Rect InfoRect { get { return new Rect(0, position.height - 20, position.width, 20); } }
        //材质球数量
        public Rect textureNumRect { get { return new Rect(position.width - 52, position.height - 18, 60, 20); } }
        //当前材质球
        public Rect MaterailRect { get { return new Rect(position.width - 192, position.height - 18, 140, 20); } }

        private static SplitView _splitView;

        public static SearchArea MySearchArea;
        public static MainArea mainArea;
        public static MainArea FirstLineArea;
        public static MainArea TextureArea;
        private static Filter<int> SizeFilter;
        private static ToolbarArea MyToolbarArea;
        private static Filter<TextureWrapMode> WrapModeFilter;
        private static List<TextureTools> TextureToolsList = new List<TextureTools>();
        public static bool IsChangeMaterialTexture = true;

        public static void Open()
        {
            LoadData();
           var go= GetWindow<SelectTextureWindow>();
           go.minSize=new Vector2(500,500);
           go.Show();
        }

        private void OnEnable()
        {
            //  LoadData();
            //   Debug.Log("加载");

            skin = Resources.Load<GUISkin>("mySkin");
            //  kuang = AssetDatabase.LoadAssetAtPath<Texture>("Assets/YaoZiTools/SelectTextureExtension/kuang.png");
            //  LoadData(); //加载配置
            //  TexturPropertyNames = MyData.NowMaterial.GetTextureProperty();
            // IsSelectTexture = TexturPropertyNames.ToDictionary(a => a, b => false);
            if (MyData.NowMaterial == null)
            {
                MyData.NowMaterial = MyData.Materials[MyData.Materials.Count - 1];
            }

            this.minSize = new Vector2(MyData.TextureSize * 3, MyData.TextureSize * 2);
            this.titleContent =
                new GUIContent("Select Texture", Resources.Load<Texture2D>("SelectTextureWindowIcon"));
            // LoadRect();
            if (MyData.Names.Count != 0 && MyData.Names.Count != DrawTextures.Count)
            {
                DrawTextures.Clear();
                ToolbarRects = new Rect[MyData.Names.Count]; //?

                for (int i = 0; i < MyData.Paths.Count; i++)
                {
                    DrawTextures.Add(new DrawTextureGroup(MyData.Paths[i])
                    {
                        SelectIsSizType = mTextureGroupBox.SelectIsTextureSizeType,
                        SelectIsTextureWrapMode = mTextureGroupBox.SelectIsTextureWrapMode
                    });

                }
            }

            mTextureGroupBox = new TextureGroupBox();
            // mTextureGroupBox.SizeTypeLength = new Dictionary<int, int>();
            mTextureGroupBox.SelectIsTextureSizeType = new Dictionary<int, bool>();
            mTextureGroupBox.SelectIsTextureWrapMode = new Dictionary<TextureWrapMode, bool>();

            SeachString = "";

            foldeField = new FoldeField();

            IsToolbarEnabled = true;

            SelectTextureCustomMaterialInspector.ButtomEvent += OnReSeletcData;

            CloseButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            CloseButtonStyle.padding.bottom = 0;
            CloseButtonStyle.padding.right = 0;
            CloseButtonStyle.padding.left = 0;
            CloseButtonStyle.padding.top = 0;
            CloseButtonStyle.clipping = TextClipping.Overflow;
            CloseButtonStyle.stretchHeight = false;
            CloseButtonStyle.stretchWidth = false;

            PropertyButtonStyle = new GUIStyle("CommandMid");
            PropertyButtonStyle.fixedWidth = 50;
            PropertyButtonStyle.fixedHeight = 50;

            SortIcon = EditorGUIUtility.IconContent("AlphabeticalSorting");//排序图标
            SortIcon.tooltip = "排序";

            TextureSizeTool.IsSDButtonValue = true;
            TextureSizeTool.TempInt = 0;

            DrawTextureGroup.IsTextureChange += SetTextureInMaterial;


            _splitView.FirstArea += FirstArea;
            _splitView.SecondArea += SecondArea;

            _splitView.DrawDragAndDrropRect += DrawDragAndDrropRect;

            mainArea = new MainArea(MainArea.Arrangement.Vertical, "box");
            FirstLineArea = new MainArea(MainArea.Arrangement.Horizontal, "box") { Rect = new Rect(0, 0, this.position.width, 20) };
            TextureArea = new MainArea(MainArea.Arrangement.Vertical, "box");
            FirstLineArea.Rect = new Rect(0, 0, 0, 20);

            MySearchArea = new SearchArea();
            MySearchArea.SearchHistory = MyData.SeachString;
            MySearchArea.SearchTextIsChange += (s) => { SeachString = s; RefreshFilter(); };
            FirstLineArea.Content.Add(MySearchArea);
            SizeFilter = new Filter<int>();
            SizeFilter.Label = "Size";
            SizeFilter.ToggleTepyList = DrawTextures[selectedGroup].getTextureList.TextureSizeList;
            WrapModeFilter = new Filter<TextureWrapMode>("WrapMode", DrawTextures[selectedGroup].getTextureList.TextureWrapMode);
            SizeFilter.IsToggleChange += RefreshFilter;
            WrapModeFilter.IsToggleChange += RefreshFilter;
            // SizeFilter.MyPopupWindowContent.IsToggleChange += SelectTextureWindow.RefreshFilter;
            WrapModeFilter.IsToggleChange += RefreshFilter;




            FirstLineArea.Content.Add(new FlexibleArea());
            FirstLineArea.Content.Add(SizeFilter);
            FirstLineArea.Content.Add(WrapModeFilter);


            MyToolbarArea = new ToolbarArea(MyData.Names, MyData.Paths, SelectTextureWindow.selectedGroup);
            MyToolbarArea.IsListChange += SetDirtyInData;
            MyToolbarArea.IsListAdd += AddNewDirector;
            MyToolbarArea.IsListRemoveIndex += SelectTextureWindow.RemoveDataInIndex;
            MyToolbarArea.IsSelectChange += SelectTextureWindow.RefreshFilter;
            MyToolbarArea.IsSelectChange += SelectTextureWindow.SetToogleList;
            TextureArea.Content.Add(MyToolbarArea);
            TextureArea.Content.Add(_splitView);
            //  FirstLine.Content.Add(new Filter<int>("Mode",DrawTextures[selectedGroup].getTextureList.TextureSizeList));
            mainArea.Content.Add(FirstLineArea);
            mainArea.Content.Add(TextureArea);


            // var r =new FirstLineArea(40);
            //  var r2 =new FirstLineArea(40);
            //  r2.Content.Add(new SearchArea(MyData.SeachString));
            //  r.Content.Add(new Filter(new SizeFilterPopupWindow<int>(DrawTextures[selectedGroup].getTextureList.TextureSizeList,50f),"Size") );
            // mainArea.Content.Add(r);
            // mainArea.Content.Add(r2);
            // DrawTextures[selectedGroup].Load();



            //aaa.Invoke(MyData.NowMaterial,BindingFlags.Static,null,null,null);
            // for (int i = 0; i < aaa.Length; i++)
            // {
            //   //  Debug.Log(aaa[i].Name.ToString());
            //      Debug.Log(aaa[i].GetValue(mEditorWindow[0]));
            // }
            // isSelect.Clear();
        }

        private static void SetToogleList()
        {
            SizeFilter.ToggleTepyList = DrawTextures[MyToolbarArea.selectedIndex].getTextureList.TextureSizeList;
            WrapModeFilter.ToggleTepyList = DrawTextures[MyToolbarArea.selectedIndex].getTextureList.TextureWrapMode;
        }

        private void SetDirtyInData()
        {
            EditorUtility.SetDirty(MyData);
        }

        private void DrawDragAndDrropRect(Rect obj)
        {
            GUILayout.BeginArea(obj);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("", "RL DragHandle", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        /// <summary>
        /// 贴图单张预览
        /// </summary>
        public static Texture MaterialTexture;

        private void SecondArea(Rect obj)
        {
            var textRectSize = obj.height - 20;
            textRectSize = Mathf.Clamp(textRectSize, 50, 200);
            var textureButtonRect = new Rect(obj.x + 3, obj.y, textRectSize, textRectSize);
            var textureToolsRect = new Rect(textureButtonRect.xMax + 3, obj.y, 26, obj.height - 20);
            var toolsMainRect = new Rect(textureToolsRect.xMax - 2, obj.y, obj.width - textureToolsRect.xMax - 50, obj.height - 20);

            if (GUI.Button(textureButtonRect, MaterialTexture, SelectTextureWindow.skin.customStyles[1]))
            {
                EditorGUIUtility.PingObject(MaterialTexture);
            }
            // GUILayout.EndArea();

            int tempValue = -1;
            GUILayout.BeginArea(textureToolsRect);

            for (int i = 0; i < TextureToolsList.Count; i++)
            {
                TextureToolsList[i].MyTexture = MaterialTexture;
                TextureToolsList[i].Rect = toolsMainRect;
                TextureToolsList[i].ShowToggle();
                if (TextureToolsList[i].ToggleValue)
                {
                    if (i != tempValue && tempValue != -1)
                    {
                        TextureToolsList[tempValue].ToggleValue = false;
                    }
                    tempValue = i;

                }
            };
            GUILayout.EndArea();

            if (tempValue != -1)
            {
                GUILayout.BeginArea(toolsMainRect, "", "LODBlackBox");
                TextureToolsList[tempValue].OnGUI(toolsMainRect);
                GUILayout.EndArea();

            }


            // for (int i = 0; i < TextureToolsList.Count; i++)
            // {
            //     if (TextureToolsList[i].ToggleValue)
            //     {
            //         GUILayout.BeginArea(toolsMainRect, "", "LODBlackBox");

            //         TextureToolsList[0].OnGUI(toolsMainRect);
            //         if (i != tempValue && tempValue != -1)
            //         {
            //             TextureToolsList[tempValue].ToggleValue = false;
            //         }
            //         tempValue = i;
            //         GUILayout.EndArea();
            //     }
            // }




            GUILayout.BeginArea(InfoRect);
            {

                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    // GUILayout.Label("","CN EntryInfoIconSmall");
                    GUILayout.Label(TexturePathInfo, "PR DisabledLabel");
                    if (!string.IsNullOrEmpty(TexturePathInfo))
                    {


                        if (GUILayout.Button("open", "AssetLabel Partial"))
                        {
                            //EditorGUIUtility.PingObject(MyData.NowMaterial.GetTexture(MyData.NowTextruePropertyName));
                            AssetDatabase.OpenAsset(MaterialTexture);
                        }
                        if (GUILayout.Button("Save", "AssetLabel Partial"))
                        {
                            //EditorGUIUtility.PingObject(MyData.NowMaterial.GetTexture(MyData.NowTextruePropertyName));
                            SaveTextureInAssets(DrawTextures[selectedGroup].NowTextureBoxs[DrawTextures[selectedGroup].SelectIndex]);
                        }
                    }
                    GUILayout.FlexibleSpace();

                    if (DrawTextures != null && DrawTextures[selectedGroup].NowTextureBoxs != null)
                    {
                        GUILayout.Label(DrawTextures[selectedGroup].NowTextureBoxs.Count.ToString(), "MeTimeLabel");//显示图片数量
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            //当前材质球
            //  EditorGUI.ObjectField(MaterailRect, MyData.NowMaterial, typeof(Material), true);

        }

        private void FirstArea(Rect obj)
        {
            GUILayout.BeginArea(obj, "", "FrameBox");

            if ((DrawTextures.Count != 0 && !DrawTextures[MyToolbarArea.selectedIndex].IsLoad))
            {
                DrawTextures[MyToolbarArea.selectedIndex].Load();
            }
            else if (DrawTextures.Count != 0)
            {
                // Debug.Log( DrawTextures[selectedGroup].NowTextureBoxs.Count+":"+DrawTextures[selectedGroup].getTextureList.textrueArrayLength) ;
                DrawTextures[MyToolbarArea.selectedIndex].Draw(new Rect(0, 0, obj.width, obj.height));
            }
            GUILayout.EndArea();
        }

        private void SetTextureInMaterial(TextureBox obj)
        {
            TexturePathInfo = obj.TexturePath;
            //设置材质球贴图
            if (IsChangeMaterialTexture)
            {
                SetTextureInMaterial(obj.Texture);
            }

            //贴图单张预览
            MaterialTexture = obj.Texture;
        }
        public static void SetTextureInMaterial(Texture2D texture2D)
        {
            Undo.RecordObject(SelectTextureWindow.MyData.NowMaterial, "修改材质贴图");//注册可撤销
            SelectTextureWindow.MyData.NowMaterial.SetTexture(SelectTextureWindow.MyData.NowTextruePropertyName, texture2D);
        }

        private void OnReSeletcData()
        {
            MyData.NowTextruePropertyName = PropertyName;
            MyData.NowMaterial = Material;
        }

        private bool selection;

        private static int selectedGroup;

        private int debug1;

        private bool AddDirectoryToggleValue;

        private void OnGUI()
        {
            //图片大小滑动条
            if (TextureSizeRect != null)
            {
                MyData.TextureSize = (int)GUI.HorizontalSlider(TextureSizeRect, MyData.TextureSize, 50f, 200f);
            }

            //第一行
            FirstLineArea.OnGUI(new Rect(0, 0, position.width, 20));
            //贴图区域 包含Toolbar
            TextureArea.OnGUI(new Rect(0, 20, position.width, position.height - 20));
            Repaint();
        }

        void ShowButton(Rect position)
        {
            // if (GUI.Button(position, "Debug"))
            // {
            //     DebugToggle = !DebugToggle;
            // }

            // Rect shezhiRect = new Rect(position);
            // shezhiRect.x -= 20;
            // shezhiRect.width=20;
            // shezhiRect.height=15;
            position.width += 5;
            position.x -= 5;
            if (GUI.Button(position, EditorGUIUtility.IconContent("_Popup"), "RL FooterButton"))
            {

                SettingsWindow.Open(new Rect(this.position.xMax - 300, this.position.y + 20, 300, 200));
            };
        }

        static void AddNewDirector()
        {
            // LoadData();
            //只new新加的
            DrawTextures.Add(new DrawTextureGroup(MyData.Paths[MyData.Paths.Count - 1]));
        }

        static void LoadData()
        {

            if (!File.Exists(Data.GetDataPath() + "/SelectTextureWindowData.asset"))
            {
                Data.CreateDefaultData(Material);
            }
            if (MyData == null)
            {
                MyData = AssetDatabase.LoadAssetAtPath<SelectTextureWindowData>(Data.GetDataPath() + "/SelectTextureWindowData.asset");
            }
            if (_splitView == null)
            {
                _splitView = new SplitView(SplitType.Vertical, AutoFillRect.SecondRect);
            }
            if (Material != null)
            {
                MyData.NowMaterial = Material;
            }
            if (PropertyName != null)
            {
                MyData.NowTextruePropertyName = PropertyName;
            }
            SplitView.SplitSize = MyData.SplitSize;

            MaterialTexture = MyData.NowMaterial.GetTexture(MyData.NowTextruePropertyName);
            if (MaterialTexture == null)
            {
                MaterialTexture = Texture2D.whiteTexture;
            }
            if (MaterialTexture != null)
            {
                TexturePathInfo = AssetDatabase.GetAssetPath(MaterialTexture);
            }
            //加载插件
            var types = TypeCache.GetTypesDerivedFrom<TextureTools>();
            if (types.Count != TextureToolsList.Count)
            {
                TextureToolsList.Clear();
                for (int i = 0; i < types.Count; i++)
                {
                    TextureToolsList.Add(Activator.CreateInstance(types[i]) as TextureTools);

                }
            }

        }

        // void EditorWindow(int ID)
        // {
        //     GUILayout.Button(EditorGUIUtility.IconContent("ProjectBrowserGridLabel"));
        // }


        static void AddMaterialData(Material material)
        {
            if (MyData.Materials.Count >= 5) //数量限制
            {
                MyData.Materials.RemoveAt(0);
            }

            // MyData.Materials.Add(material);
            if (MyData.Materials.Contains(material)) //有没有一样的
            {
                MyData.Materials.RemoveAt(MyData.Materials.IndexOf(material)); //移除之前一样的
                MyData.Materials.Add(material); //再加进去 （移到最后
            }
            else
            {
                MyData.Materials.Add(material);
            }
        }

        public static void SaveDataInAsset(string path, string name = "null")
        {
            if (name == "null")
            {
                name = path.Substring(path.LastIndexOf('/') + 1);
            }

            EditorUtility.SetDirty(MyData);//设置为脏 ctrl+s就会保存

            MyData.Names.Add(name);
            MyData.Paths.Add(path);
            DrawTextures.Add(new DrawTextureGroup(path));
        }

        public static void RemoveDataInIndex(int index)
        {
            //限制最少有一个
            if (MyData.Names.Count == 1)
            {
                return;
            }
            MyData.Paths.RemoveAt(index);
            MyData.Names.RemoveAt(index);
            EditorUtility.SetDirty(MyData);//设置为脏 ctrl+s就会保存
            DrawTextures.RemoveAt(index);
            if (MyToolbarArea.selectedIndex != 0) //不是删除第一个，选中就-1
            {
                MyToolbarArea.selectedIndex--;
            }
        }
        public static void SaveData()
        {
            EditorUtility.SetDirty(MyData);
            AssetDatabase.SaveAssets();

        }

        static void AddSeachList(string SeachStrin)
        {
            if (MyData.SeachString.Count >= 5) //数量限制
            {
                MyData.SeachString.RemoveAt(0);
            }
            if (MyData.SeachString.Contains(SeachStrin)) //有没有一样的
            {
                MyData.SeachString.RemoveAt(MyData.SeachString.IndexOf(SeachStrin)); //移除之前一样的
                MyData.SeachString.Add(SeachStrin); //再加进去 （移到最后
            }
            else
            {
                MyData.SeachString.Add(SeachStrin);
            }
            EditorUtility.SetDirty(MyData);
        }
        private void OnDisable()
        {
            //DrawTextures.Clear();
            DrawTextureGroup.IsTextureChange -= SetTextureInMaterial;

            SelectTextureCustomMaterialInspector.ButtomEvent -= OnReSeletcData;
            _splitView.FirstArea -= FirstArea;
            _splitView.SecondArea -= SecondArea;
            _splitView.DrawDragAndDrropRect -= DrawDragAndDrropRect;
            SizeFilter.IsToggleChange -= RefreshFilter;
            WrapModeFilter.IsToggleChange -= RefreshFilter;
            MyToolbarArea.IsListAdd -= AddNewDirector;

            MyToolbarArea.IsListChange -= SetDirtyInData;
            MyToolbarArea.IsListRemoveIndex -= SelectTextureWindow.RemoveDataInIndex;
            MyToolbarArea.IsSelectChange -= SelectTextureWindow.RefreshFilter;

            WrapModeFilter.IsToggleChange -= RefreshFilter;

            MyData.SplitSize = SplitView.SplitSize;
            EditorUtility.SetDirty(MyData);
        }

        public static void RefreshFilter()
        {
            // if (DrawTextures==null)
            // {
            //     return;
            // }
            DrawTextures[MyToolbarArea.selectedIndex].NowTextureBoxs = DrawTextures[MyToolbarArea.selectedIndex].getTextureList.TextureBoxs;
            TextureSizeFilter(DrawTextures[MyToolbarArea.selectedIndex]);
            TextureWrapModesFilter(DrawTextures[MyToolbarArea.selectedIndex]);
            TextureSearchFilter(DrawTextures[MyToolbarArea.selectedIndex]);
            if (IsSort)
            {
                TextureSort(DrawTextures[MyToolbarArea.selectedIndex]);
            }
        }
        public static void RefreshFilter(ref List<TextureBox> textureBoxs)
        {
            TextureSizeFilter(ref textureBoxs);
        }
        public static void TextureSizeFilter(ref List<TextureBox> textureBoxs)
        {

            textureBoxs.Where((boxs =>
                       {
                           return SizeFilterPopupWindow<int>.PropetrtySelect[
                               boxs.Texture.height > boxs.Texture.width ? boxs.Texture.height : boxs.Texture.width];
                       })).ToList();
        }
        public static void RefreshFilter(DrawTextureGroup drawTextureGroup)
        {
            drawTextureGroup.NowTextureBoxs = drawTextureGroup.getTextureList.TextureBoxs;
            TextureSizeFilter(drawTextureGroup);
            TextureWrapModesFilter(drawTextureGroup);
            TextureSearchFilter(drawTextureGroup);
        }
        public static void TextureSizeFilter(DrawTextureGroup drawTextureGroup)
        {

            drawTextureGroup.NowTextureBoxs = SizeFilterPopupWindow<int>.IsAllIsFalse
                ? drawTextureGroup.NowTextureBoxs
                : drawTextureGroup.NowTextureBoxs.Where((boxs =>
                {
                    return SizeFilterPopupWindow<int>.PropetrtySelect[
                        boxs.MaxSize];
                })).ToList();
        }
        public static void TextureWrapModesFilter(DrawTextureGroup drawTextureGroup)
        {
            drawTextureGroup.NowTextureBoxs = (SizeFilterPopupWindow<TextureWrapMode>.IsAllIsFalse
                ? drawTextureGroup.NowTextureBoxs
                : drawTextureGroup.NowTextureBoxs.Where((boxs =>
                {
                    return SizeFilterPopupWindow<TextureWrapMode>.PropetrtySelect[boxs.Texture.wrapMode];
                }))).ToList();
        }
        public static void TextureSearchFilter(DrawTextureGroup drawTextureGroup)
        {
            drawTextureGroup.NowTextureBoxs = string.IsNullOrEmpty(SeachString) ? drawTextureGroup.NowTextureBoxs : drawTextureGroup.NowTextureBoxs.Where((boxs =>
            {
                return boxs.Texture.name.ToUpper().Contains(SelectTextureWindow.SeachString.ToUpper());
            })).ToList();
        }

        public static void TextureSort(DrawTextureGroup drawTextureGroup)
        {

            drawTextureGroup.NowTextureBoxs.Sort((x, y) =>
            {
                return -x.TimeInfo.CompareTo(y.TimeInfo);
            });

        }
        public static void RefreshFileInfo()
        {
            for (int i = 0; i < DrawTextures[selectedGroup].NowTextureBoxs.Count; i++)
            {
                var fi = new FileInfo(AssetDatabase.GetAssetPath(DrawTextures[selectedGroup].NowTextureBoxs[i].Texture));
                var temp = DrawTextures[selectedGroup];

            }

        }
        public static void SetSizeSelectIsFasle()
        {
            // for (int i = 0; i < MyData.TextureSizeTypes.Count; i++)
            SizeFilterPopupWindow<int>.IsAllIsFalse = true;
            SizeFilterPopupWindow<TextureWrapMode>.IsAllIsFalse = true;
            // {
            //     MyData.TextureSizeTypes[MyData.TextureSizeTypes.Keys.ToArray()[i]] = false;
            // }
        }
        public static void SaveTextureInAssets(TextureBox textureBoxs)
        {

            var texture = textureBoxs.Texture.EncodeToPNG();
            var p = textureBoxs.TexturePath.Replace("\\", "/");
            p = Path.GetFileName(p);
            p = AssetDatabase.GenerateUniqueAssetPath("Assets/TempTexture/" + p);
            var fullPatn = Path.GetFullPath(p);

            SaveTextureInAssets(textureBoxs.Texture, fullPatn);

            SelectTextureWindow.MyData.NowMaterial.SetTexture(MyData.NowTextruePropertyName, AssetDatabase.LoadAssetAtPath<Texture2D>(p));
        }
        public static void SaveTextureInAssets(Texture2D texture2D, string path)
        {
            var texture = texture2D.EncodeToPNG();
            File.WriteAllBytes(path, texture);
            AssetDatabase.Refresh();
        }
    }
}