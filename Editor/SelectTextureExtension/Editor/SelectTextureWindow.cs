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



        // private static Rect SeachRect = new Rect(5, 2, 200, 20); //搜索
        // private static Rect SeachCancelRect = new Rect(SeachRect.width+5, 2, 20, 20); //搜索关闭

        // private static Rect TextureSizeRect=new Rect(WindowRect.width-30,0,300, 20); //贴图缩放
        private static Rect DirectoryRect; //文件夹

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
        public static string TexturePathInfo;
        public static GUISkin skin;


        public static void Open()
        {
            GetWindow<SelectTextureWindow>().Show();
        }

        private void OnEnable()
        {
            //  kuang = AssetDatabase.LoadAssetAtPath<Texture>("Assets/YaoZiTools/SelectTextureExtension/kuang.png");
            skin = Resources.Load<GUISkin>("mySkin");
            LoadData(); //加载配置
            TexturPropertyNames = MyData.NowMaterial.GetTextureProperty();
            IsSelectTexture = TexturPropertyNames.ToDictionary(a => a, b => false);
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

                for (int i = 0; i < MyData.Names.Count; i++)
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

            TextureSizeTool.IsSDButtonValue=true;
            TextureSizeTool.TempInt=0;

            // DrawTextures[selectedGroup].Load();



            //aaa.Invoke(MyData.NowMaterial,BindingFlags.Static,null,null,null);
            // for (int i = 0; i < aaa.Length; i++)
            // {
            //   //  Debug.Log(aaa[i].Name.ToString());
            //      Debug.Log(aaa[i].GetValue(mEditorWindow[0]));
            // }
            // isSelect.Clear();
        }

        private void OnReSeletcData()
        {
            MyData.NowTextruePropertyName = PropertyName;
            MyData.NowMaterial = Material;
        }

        private bool selection;

        private static int selectedGroup = 0;

        private int debug1;

        private bool AddDirectoryToggleValue;

        // object ggg;

        private void OnGUI()
        {
            //  GUI.skin =skin;
            //   var editorType = typeof(EditorWindow);
            // var m_Parent = editorType.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
            //  var mEditorWindow = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(editorType)).Where(type => type.ToString().Contains("ObjectSelector")).ToList();
            //  GetWindow(mEditorWindow[0]).Show();
            // //ccc.Close();
            // var aaa =mEditorWindow[0].GetField("m_PreviewSize");
            // //var bbb = aaa.
            // Debug.Log(aaa.GetValue(m_Parent));

            //Debug.Log(EditorGUIUtility.GetObjectPickerControlID());

            // EditorGUIUtility.ShowObjectPicker<Texture2D>(MyData.NowMaterial,true,"22",0);
            // EditorGUIUtility.GetObjectPickerObject();

            // Debug.Log(Event.current.commandName);




            var TextureSizeRect = new Rect(5, position.height - 18, 70, 20);
            MyData.TextureSize = (int)GUI.HorizontalSlider(TextureSizeRect, MyData.TextureSize, 50f, 200f);//图片大小滑动条
                                                                                                           //Debug.Log(GUIUtility.GetControlID(FocusType.Passive)) ;
            GUI.Box(new Rect(0, 0, position.width, 20), "", "ColorPickerBox");

            #region 搜索

            var SeachRect = new Rect(5, 2, 200, 20); //搜索
            var SeachCancelRect = new Rect(SeachRect.xMax + 5, SeachRect.y, 20, 20); //搜索关闭
            var seachStringsRect = new Rect(SeachCancelRect.x + SeachCancelRect.width, SeachCancelRect.y, 350, 20); //搜索历史

            EditorGUI.BeginChangeCheck();
            //var sRect = GUILayoutUtility.GetRect(300, 20,GUILayout.ExpandWidth(false));
            //var sRect =new Rect(0,0,300, 20);
            SeachString = EditorGUI.TextField(SeachRect, SeachString, new GUIStyle("ToolbarSeachTextField"));
            if (!string.IsNullOrEmpty(SeachString))
            {
                if (GUI.Button(SeachCancelRect, "", "ToolbarSeachCancelButton"))
                {
                    SeachString = "";
                }
            }

            // SeachString = GUILayout.TextField(SeachString, "ToolbarSeachTextField");
            IsTextFieldChange = false;
            if (EditorGUI.EndChangeCheck())
            {
                IsTextFieldChange = true;
                RefreshFilter();

                if (!string.IsNullOrEmpty(SeachString))
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
            if (sw.Elapsed >= TimeSpan.FromSeconds(2.0) && !string.IsNullOrEmpty(SeachString))
            {
                AddSeachList(SeachString);
                //MyData.SeachString.Add(SeachString);
                sw.Stop();
                sw.Reset();
            }
            GUILayout.BeginArea(seachStringsRect);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < MyData.SeachString.Count; i++)
            {
                if (GUILayout.Button(MyData.SeachString[i], "sv_label_" + i % 7, GUILayout.MaxWidth(40)))
                {
                    SeachString = MyData.SeachString[i];
                    IsTextFieldChange = true;
                    RefreshFilter();
                }

                if (GUILayout.Button("", "ToolbarSeachCancelButton", GUILayout.Width(10)))
                {
                    MyData.SeachString.RemoveAt(i);
                }
            }

            // IsSort = GUILayout.Toggle(IsSort, SortIcon, "IconButton", GUILayout.Width(20));
            // if (IsSort)
            // {
            //     ReadFileInfo(DrawTextures[selectedGroup]);
            // }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            #endregion


            var dRecta = new Rect(5, 20, position.width - 50, 20);
            // dRecta.MinWidth(30*MyData.Names.Count);
            var e = Event.current;

            #region 文件夹栏
            GUILayout.BeginArea(dRecta);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUI.enabled = IsToolbarEnabled;
            selectedGroup = GUILayout.Toolbar(selectedGroup, MyData.Names.ToArray(), EditorStyles.toolbarButton,
                GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100 * MyData.Names.Count),
                GUILayout.MinWidth(40 * MyData.Names.Count));
            if (Event.current.type == EventType.Repaint)
            {
                ToolbarRect = GUILayoutUtility.GetLastRect();
            }

            GUI.enabled = true;

            //  ToolbarRects=  ToolbarRect.Split(Names.Count,RectExtension.SplitType.Horizontal); //不好


            AddDirectoryToggleValue = GUILayout.Toggle(AddDirectoryToggleValue,
                EditorGUIUtility.IconContent("d_CreateAddNew"), EditorStyles.toolbarButton, GUILayout.Width(20));

            if (foldeField.IsGetPath)
            {
                if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                {
                    AddDirectoryToggleValue = false;
                    foldeField.IsGetPath = false;
                    SaveDataInAsset(foldeField.Path);
                }
            }

            if (AddDirectoryToggleValue)
            {
                foldeField.OnGUI(AddDirectoryRect);
            }

            var addRect = GUILayoutUtility.GetRect(20, 200, 20, 20, GUILayout.ExpandWidth(false));

            if (Event.current.type == EventType.Repaint)
            {
                AddDirectoryRect = addRect;
            }
            if (EditorGUI.EndChangeCheck())//增加删除，或者切换 都刷新筛选
            {
                RefreshFilter();
                //DrawTextures[selectedGroup].Load();//继续加载没加载的
            }
            //EditorGUI.BeginChangeCheck();

            //获取删除文件夹按钮的位置
            ToolbarRects = ToolbarRect.Split(MyData.Names.Count, RectExtension.SplitType.Horizontal);
            ToolbarClosRects = new Rect[ToolbarRects.Length];
            for (int i = 0; i < ToolbarRects.Length; i++)
            {
                ToolbarClosRects[i] = ToolbarRects[i];
                ToolbarClosRects[i].x = ToolbarRects[i].xMax - 20;
                ToolbarClosRects[i].height = 20;
                ToolbarClosRects[i].width = 20;
            }


            for (int i = 0; i < MyData.Names.Count; i++)
            {
                Undo.RecordObject(MyData, "改名");
                if ((ToolbarRect.Contains(e.mousePosition) && e.clickCount == 2) || !IsToolbarEnabled) //双击出现
                {
                    GUILayout.BeginArea(ToolbarRects[i]);
                    GUILayout.BeginHorizontal();
                    MyData.Names[i] = EditorGUILayout.DelayedTextField(MyData.Names[i]);

                    //MyData.Names[i] = Names[i];
                    // Debug.Log("hh");
                    IsToolbarEnabled = false;
                    if (GUILayout.Button("", //删除文件夹按钮
                            "WinBtnClose"))
                    {
                        //Undo.RecordObject(MyData,"数据");
                        RemoveDataInIndex(i);
                        if (selectedGroup != 0) //不是删除第一个，选中就-1
                        {
                            selectedGroup--;
                        }
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }
            }

            if (!(ToolbarRect.Contains(e.mousePosition)) && e.clickCount == 1)
            {
                IsToolbarEnabled = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            #endregion





            #region 材质属性

            // var MaterialPropertyRect =
            //     new Rect(tRecta.width + 5, tRecta.y + 5, position.width - tRecta.width, tRecta.height);
            // GUILayout.BeginArea(MaterialPropertyRect);


            // GUILayout.BeginVertical();

            // EditorGUILayout.ObjectField(MyData.NowMaterial, typeof(Material));
            // IsSelectTexture[MyData.NowTextruePropertyName] = true; //now 永远都是选中状态
            // for (int i = 0; i < TexturPropertyNames.Count; i++)
            // {
            //     IsSelectTexture[TexturPropertyNames[i]] = GUILayout.Toggle(IsSelectTexture[TexturPropertyNames[i]],
            //         MyData.NowMaterial.GetTexture(TexturPropertyNames[i]), PropertyButtonStyle);
            //     if (IsSelectTexture[TexturPropertyNames[i]])
            //     {
            //         IsSelectTexture[MyData.NowTextruePropertyName] = false; //先将之前的选中取消
            //         MyData.NowTextruePropertyName = TexturPropertyNames[i]; //将选中的名字赋值给now
            //     }
            // }

            // GUILayout.EndVertical();
            // GUILayout.EndArea();

            #endregion

            #region 筛选栏

            var shaixuanRect = new Rect(position.width - 190, 0, 190, 20);
            //var shaixuanRect = GUILayoutUtility.GetRect(position.width, 20);
            var refreshButtonRect = new Rect(position.width - 30, 0, 30, 20);
            // ButtonRect.x = shaixuanRect.xMax - 28;
            // ButtonRect.width = 30;
            if (GUI.Button(refreshButtonRect, EditorGUIUtility.IconContent("d_Refresh"), "AppToolbarButtonLeft"))//重置筛选条件
            {
                SetSizeSelectIsFasle();
                for (int i = 0; i < MyData.TextureWrapModes.Count; i++)
                {
                    MyData.TextureWrapModes[MyData.TextureWrapModes.Keys.ToArray()[i]] = false;
                }

                //SizeFilterPopupWindow.IsAllIsFalse = true;
               // ModeFilterPopupWindow.IsAllIsFalse = true;
                RefreshFilter();


            }

            var SizeToggleRect = refreshButtonRect;
            SizeToggleRect.width = 80;
            SizeToggleRect.x = refreshButtonRect.x - SizeToggleRect.width - 2;

            var wrapModeToggleRect = SizeToggleRect;
            wrapModeToggleRect.width = 80;
            wrapModeToggleRect.x = SizeToggleRect.x - wrapModeToggleRect.width - 2;


            SizeToggleValue = GUI.Toggle(SizeToggleRect, false, "MaxSize", "ToolbarDropDownToggle");
            if (SizeToggleValue)
            {
                PopupWindow.Show(SizeToggleRect, new SizeFilterPopupWindow<int>(DrawTextures[selectedGroup].getTextureList.TextureSize,SizeToggleRect.width));
            }

            wrapModeToggleValue = GUI.Toggle(wrapModeToggleRect, false, "Mode", "ToolbarDropDownToggle");
            if (wrapModeToggleValue)
            {
                // PopupWindow.Show(wrapModeToggleRect, new ModeFilterPopupWindow()
                // {
                //     mRectX = wrapModeToggleRect.width,
                //     TextureWrapMode = DrawTextures[selectedGroup].getTextureList.TextureWrapMode,
                // });
                 PopupWindow.Show(wrapModeToggleRect, new SizeFilterPopupWindow<TextureWrapMode>(DrawTextures[selectedGroup].getTextureList.TextureWrapMode,wrapModeToggleRect.width));
            }

            // EditorGUI.DropdownButton(SizeToggleRect, new GUIContent("啥啊"), FocusType.Keyboard);
            // EditorGUI.DrawRect(SizeToggleRect,Color.cyan);

            #endregion

            #region 排序

            var sortRect = new Rect(position.width - 30, 22, 30, 20);
            var sortTipsRect = new Rect(position.width - 110, 20, 80, 20);
            //  if (GUI.Button(sortRect,SortIcon,"TimeScrubberButton"))
            //  {
            //     // TextureSort(DrawTextures[selectedGroup]);
            //      RefreshFilter();
            //  }
            EditorGUI.BeginChangeCheck();
            IsSort = GUI.Toggle(sortRect, IsSort, SortIcon, "RL FooterButton");

            if (EditorGUI.EndChangeCheck())
            {
                RefreshFilter();
            }
            if (IsSort)
            {
                GUI.Label(sortTipsRect, "修改时间", "MeTimeLabel");
            }
            else
            {
                GUI.Label(sortTipsRect, "默认排序", "MeTimeLabel");
            }

            #endregion

            #region 三个按钮
            // var toolRect = new Rect(5, 40, position.width, 20);
            // TextureSizeTool.Draw(toolRect);
            #endregion
            #region 贴图区域

            var tRecta = new Rect(2, 40, position.width - 2, position.height - 60);

            // tRecta.MinHeight(MyData.TextureSize);
            // tRecta.MinWidth(MyData.TextureSize);
            // var texRect = GUILayoutUtility.GetRect(MyData.TextureSize, position.width, MyData.TextureSize,
            //         position.height); //贴图区域
            GUILayout.BeginArea(tRecta, "", "FrameBox");
            //  DrawTextures[selectedGroup].getTextureList.textrueArrayLength

            if ((DrawTextures.Count != 0 && !DrawTextures[selectedGroup].IsLoad))
            {
                DrawTextures[selectedGroup].Load();
            }
            else if (DrawTextures.Count != 0)
            {
                // Debug.Log( DrawTextures[selectedGroup].NowTextureBoxs.Count+":"+DrawTextures[selectedGroup].getTextureList.textrueArrayLength) ;
                DrawTextures[selectedGroup].Draw(new Rect(0, 0, tRecta.width, tRecta.height));
            }

            // DrawTextures[selectedGroup].Draw(tRecta);    


            // if (DrawTextures[selectedGroup].Equals(null))
            // {
            //   DrawTextures.Insert(selectedGroup,new DrawTextureGroup(MyData.Paths[selectedGroup])
            //         {
            //             SelectIsSizType = mTextureGroupBox.SelectIsTextureSizeType,
            //             SelectIsTextureWrapMode = mTextureGroupBox.SelectIsTextureWrapMode
            //         });
            // }
            // else
            // {
            //    DrawTextures[selectedGroup].Draw(tRecta); 
            // }



            GUILayout.EndArea();

            #endregion
            #region 底部信息栏

            //var InfoArea = GUILayoutUtility.GetRect(position.width, 20);
            var InfoRect = new Rect(TextureSizeRect.xMax + 20, position.height - 18, position.width, 20);
            var textureNumRect = new Rect(position.width - 52, position.height - 18, 60, 20);//贴图数量
            var MaterailRect = new Rect(position.width - 192, position.height - 18, 140, 20);//材质球
            // if (Event.current.type == EventType.Repaint)
            // {
            //     //infoAreaRect = InfoArea;
            // }

            GUILayout.BeginArea(InfoRect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(TexturePathInfo, "PR DisabledLabel");
                    if (!string.IsNullOrEmpty(TexturePathInfo))
                    {
                        if (GUILayout.Button("ping", "AssetLabel Partial"))
                        {
                            EditorGUIUtility.PingObject(MyData.NowMaterial.GetTexture(MyData.NowTextruePropertyName));
                        }

                        if (GUILayout.Button("open", "AssetLabel Partial"))
                        {
                            //EditorGUIUtility.PingObject(MyData.NowMaterial.GetTexture(MyData.NowTextruePropertyName));
                            AssetDatabase.OpenAsset(MyData.NowMaterial.GetTexture(MyData.NowTextruePropertyName));
                        }
                    }


                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            if (DrawTextures != null && DrawTextures[selectedGroup].NowTextureBoxs != null)
            {
                GUI.Label(textureNumRect, DrawTextures[selectedGroup].NowTextureBoxs.Count.ToString(), "MeTimeLabel");//显示图片数量
            }
            //当前材质球
            EditorGUI.ObjectField(MaterailRect, MyData.NowMaterial, typeof(Material), true);


            #endregion

            var textureInfo = new Rect(tRecta.x, tRecta.yMax - 180, tRecta.width, 180);

            // GUILayout.BeginArea(textureInfo,new GUIStyle("box"));
            // {
            //     EditorGUI.DrawRect(textureInfo,new Color(1,1,1,1));
            //     GUILayout.Button("测试按钮");
            // }GUILayout.EndArea();

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
            //  Names.Clear();
            // TexturePaths.Clear();
            if (!File.Exists(Data.GetDataPath() + "/SelectTextureWindowData.asset"))
            {
                Data.CreateDefaultData(Material);
            }

            MyData = AssetDatabase.LoadAssetAtPath<SelectTextureWindowData>(Data.GetDataPath() + "/SelectTextureWindowData.asset");
            //把外面进来的材质球加载数据的时候保存到历史记录
            // if ( MyData.NowMaterial==null)
            // {
            //    Debug.Log("null");
            // }
            if (Material != null)
            {
                // AddMaterialData(Material); //添加历史记录
                MyData.NowMaterial = Material;
            }
            // Material = MyData.Materials[MyData.Materials.Count - 1];
            // Material = MyData.NowMaterial;

            if (PropertyName != null)
            {
                MyData.NowTextruePropertyName = PropertyName;
            }

           ModeFilterPopupWindow.IsAllIsFalse = true;
            SizeFilterPopupWindow<int>.IsAllIsFalse = true;
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
            MyData.Paths.RemoveAt(index);
            MyData.Names.RemoveAt(index);
            EditorUtility.SetDirty(MyData);//设置为脏 ctrl+s就会保存
            DrawTextures.RemoveAt(index);
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

            SelectTextureCustomMaterialInspector.ButtomEvent -= OnReSeletcData;

        }

        public static void RefreshFilter()
        {
            DrawTextures[selectedGroup].NowTextureBoxs = DrawTextures[selectedGroup].getTextureList.TextureBoxs;
            TextureSizeFilter(DrawTextures[selectedGroup]);
            TextureWrapModesFilter(DrawTextures[selectedGroup]);
            TexturesousuoFilter(DrawTextures[selectedGroup]);
            if (IsSort)
            {
                TextureSort(DrawTextures[selectedGroup]);
            }
        }
        public static void RefreshFilter(DrawTextureGroup drawTextureGroup)
        {
            drawTextureGroup.NowTextureBoxs = drawTextureGroup.getTextureList.TextureBoxs;
            TextureSizeFilter(drawTextureGroup);
            TextureWrapModesFilter(drawTextureGroup);
            TexturesousuoFilter(drawTextureGroup);
        }
        public static void TextureSizeFilter(DrawTextureGroup drawTextureGroup)
        {

            drawTextureGroup.NowTextureBoxs = SizeFilterPopupWindow<int>.IsAllIsFalse
                ? drawTextureGroup.NowTextureBoxs
                : drawTextureGroup.NowTextureBoxs.Where((boxs =>
                {
                    return SizeFilterPopupWindow<int>.PropetrtSelect[
                        boxs.Texture.height > boxs.Texture.width ? boxs.Texture.height : boxs.Texture.width];
                })).ToList();
        }
        public static void TextureWrapModesFilter(DrawTextureGroup drawTextureGroup)
        {
            drawTextureGroup.NowTextureBoxs = (ModeFilterPopupWindow.IsAllIsFalse
                ? drawTextureGroup.NowTextureBoxs
                : drawTextureGroup.NowTextureBoxs.Where((boxs =>
                {
                    return SizeFilterPopupWindow<TextureWrapMode>.PropetrtSelect[boxs.Texture.wrapMode];
                }))).ToList();
        }
        public static void TexturesousuoFilter(DrawTextureGroup drawTextureGroup)
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
            for (int i = 0; i < MyData.TextureSizeTypes.Count; i++)
            {
                MyData.TextureSizeTypes[MyData.TextureSizeTypes.Keys.ToArray()[i]] = false;
            }
        }
    }
}