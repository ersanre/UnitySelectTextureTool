using System.Security.Principal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YaoZiTools.SelectTextureExtension.Editor;
using Debug = UnityEngine.Debug;
using Directory = System.IO.Directory;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class TextureBox //贴图单个数据
    {
        public Texture2D Texture;
        public DateTime TimeInfo;
        public string TexturePath;
        public bool IsSelect;
        public int MaxSize
        {
            get { return Texture.width > Texture.height ? Texture.width : Texture.height; }
        }
        public TextureBox(Texture2D texture2D, DateTime dateTime, String texturePath, bool isSelect = false)
        {
            this.Texture = texture2D;
            this.TimeInfo = dateTime;
            this.TexturePath = texturePath;
            this.IsSelect = isSelect;
        }

    }
}

public class GetTextureList
{
    //public static List<Texture> GetAssetAllTexture()
    // public List<Texture> alltextures = new List<Texture>();
    public List<TextureWrapMode> TextureWrapMode = new List<TextureWrapMode>(); //模式表不添加重复模式
    public List<int> TextureSizeList = new List<int>();
    public List<TextureBox> TextureBoxs = new List<TextureBox>();
    // public List<TextureBoxs> NowTextureBoxs = new List<TextureBoxs>();
    //   public DrawTextureGroup DrawTextureGroup;
    private int _textrueArrayLength;
    public int TextrueArrayLength
    {
        get { return _textrueArrayLength; }
    }
    // public bool IsGetOver;

    private int LodIndex;


    public IEnumerator GetAssetTextureInPath(string path, bool isGetDirectory = false)
    {
        LodIndex = 0;
        var sw = Stopwatch.StartNew();
        string[] guidOrPath;
        bool isAssets = path.StartsWith("Assets");//判断是否是工程内的路径
        // List<Texture> textures = new List<Texture>();
        if (isAssets)
        {
            guidOrPath = AssetDatabase.FindAssets("t:Texture", new string[] { path });
        }
        else
        {
            //较快 ，排序有点问题
            // string[] filters = new[] { "*.png", "*.tga", "*.jpg" };
            // guidOrPath = filters.SelectMany(f => Directory.GetFiles(path, f, SearchOption.AllDirectories)).ToArray();

            // string[] filePaths = filters.SelectMany(f => Directory.GetFiles(basePath, f)).ToArray();
            //耗时久
            //guidOrPath = Directory.GetFiles(path, "*.png||*.jpg",SearchOption.AllDirectories);
            guidOrPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".tga")).ToArray();
        }



        _textrueArrayLength = guidOrPath.Length; //确定长度
        Debug.Log("获取全部Texture的GUID耗时:" + sw.Elapsed); //很快
        sw.Restart();
        Texture2D texture;
        var tempPath = "";
        for (int i = 0; i < guidOrPath.Length; i++)
        {


            if (isAssets)
            {
                tempPath = AssetDatabase.GUIDToAssetPath(guidOrPath[i]);
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>(tempPath);
            }
            else
            {
                tempPath = guidOrPath[i];

                using (FileStream fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
                {
                    var length = fileStream.Length;
                    LodIndex = (int)length;
                    byte[] b = new byte[length];
                    fileStream.Read(b, 0, (int)length);
                    texture = new Texture2D(512, 512);
                    texture.LoadImage(b);
                }
            }
            var tempTextureBox =new TextureBox(texture, new FileInfo(tempPath).LastWriteTime, tempPath);

            TextureBoxs.Add(tempTextureBox);

           // var maxSize = texture.width > texture.height ? texture.width : texture.height;
            if (!TextureWrapMode.Contains(texture.wrapMode))
            {
                TextureWrapMode.Add(texture.wrapMode); //收集贴图组的模式表
            }

            if (!TextureSizeList.Contains(tempTextureBox.MaxSize)) //收集的贴图组的Size表
            {
                // TextureSize.Add(maxSize);
                var b = false;

                for (int j = 0; j < TextureSizeList.Count; j++) //排序小到大
                {
                    if (tempTextureBox.MaxSize < TextureSizeList[j])
                    {
                        TextureSizeList.Insert(j, tempTextureBox.MaxSize);
                        b = true;
                        break;
                    }
                }

                if (!b)
                {
                    TextureSizeList.Add(tempTextureBox.MaxSize);
                }
            }

            /* 
             if (SelectTextureWindow.MyData.TextureSizeTypes == null)
             {
                 SelectTextureWindow.MyData.TextureSizeTypes = new Dictionary<int, bool>();
             }

             if (SelectTextureWindow.MyData.TextureWrapModes == null)
             {
                 SelectTextureWindow.MyData.TextureWrapModes = new Dictionary<TextureWrapMode, bool>();
             }

             if (!SelectTextureWindow.MyData.TextureSizeTypes.ContainsKey(maxSize))
             {
                 SelectTextureWindow.MyData.TextureSizeTypes.Add(maxSize, false);
                 //SelectTextureWindow.MyData.TextureSizeTypes.OrderBy((p => p.Key));
             }

             if (!SelectTextureWindow.MyData.TextureWrapModes.ContainsKey(texture.wrapMode))
             {
                 SelectTextureWindow.MyData.TextureWrapModes.Add(texture.wrapMode, false);
             }
                 */

            // LodIndex++;
            if ((i % 100 == 0) || LodIndex >= 800000 && i != 0) //加载100个每帧
            {
                //Debug.Log(path+" : " + i);
                SelectTextureWindow.RefreshFilter();//刷新筛选数据，跑一下筛选 
                yield return null;
            }
        }
        //  IsGetOver = true;
        SelectTextureWindow.RefreshFilter();
        Debug.Log("加载图片耗时：" + sw.Elapsed); //耗时12
    }


    public static List<GUIContent> GetAssetAllTextureContent()
    {
        List<GUIContent> textures = new List<GUIContent>();
        // List<GUIContent> texturesContents = new List<GUIContent>();
        var guid = AssetDatabase.FindAssets("t:Texture", new string[] { "Assets" });
        for (int i = 0; i < guid.Length; i++)
        {
            textures.Add(new GUIContent(i.ToString(),
                AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid[i]))));
            //texturesContents.Add( new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid[i]))));
        }

        return textures;
    }


    public static List<Texture> GetTextures(string[] paths)
    {
        List<Texture> textures = new List<Texture>();
        for (int i = 0; i < paths.Length; i++)
        {
            textures.Add(AssetDatabase.LoadAssetAtPath<Texture>(paths[i]));
            //texturesContents.Add( new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid[i]))));
        }

        return textures;
    }

    /// <summary>
    /// 将全路径转换成Assets下路径
    /// </summary>
    /// <param name="fullPatn">全路径</param>
    /// <returns>Assets路径</returns>
    public static string[] ConvertAssetPaths(string[] fullPatn)
    {
        string[] newPahts = new string[fullPatn.Length];
        for (int i = 0; i < fullPatn.Length; i++)
        {
            newPahts[i] = fullPatn[i].Substring(31);
        }

        return newPahts;
    }
    public void GetTextureFilePath(string path)
    {
        var texturePath = Directory.GetFiles(path, "*.png");
        for (int i = 0; i < texturePath.Length; i++)
        {
            using (FileStream fileStream = new FileStream(texturePath[i], FileMode.Open, FileAccess.Read))
            {
                byte[] b = new byte[fileStream.Length];
                fileStream.Read(b, 0, (int)fileStream.Length);
                var texture = new Texture2D(512, 512);
                texture.LoadImage(b);
                // TextureBoxs.Add(new TextureBoxs()
                // {
                //     Texture = texture, //贴图

                // });
            }
        }


    }

}