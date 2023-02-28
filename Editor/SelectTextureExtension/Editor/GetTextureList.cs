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
    public struct TextureBoxs //贴图单个数据
    {
        public Texture Texture;

        public DateTime TimeInfo;

    }
}

public class GetTextureList
{
    //public static List<Texture> GetAssetAllTexture()
    // public List<Texture> alltextures = new List<Texture>();
    public List<bool> isSelect = new List<bool>();
    public List<TextureWrapMode> TextureWrapMode = new List<TextureWrapMode>(); //模式表不添加重复模式
    public List<int> TextureSize = new List<int>();
    public List<TextureBoxs> TextureBoxs = new List<TextureBoxs>();
    // public List<TextureBoxs> NowTextureBoxs = new List<TextureBoxs>();
    //   public DrawTextureGroup DrawTextureGroup;
    private int _textrueArrayLength;
    public int TextrueArrayLength
    {
        get{return _textrueArrayLength;}
    }
    public bool IsGetOver;
    private int LodIndex;


    public IEnumerator GetAssetTextureInPath(string path, bool isGetDirectory = false)
    {
        // List<Texture> textures = new List<Texture>();

        var sw = Stopwatch.StartNew();
        var guid = AssetDatabase.FindAssets("t:Texture", new string[] { path });
        _textrueArrayLength = guid.Length; //确定长度
        if (TextureBoxs.Count == TextrueArrayLength)
        {
            yield return 0;
        }

        Debug.Log("获取全部Texture的GUID耗时:" + sw.Elapsed); //很快
        sw.Restart();
        //alltextures.Clear();
        //isSelect.Clear();
        // TextureWrapMode.Clear();
        for (int i = LodIndex; i < guid.Length; i++)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid[i]));
            var maxSize = texture.width > texture.height ? texture.width : texture.height;
            if (!TextureWrapMode.Contains(texture.wrapMode))
            {
                TextureWrapMode.Add(texture.wrapMode); //自己的贴图组的模式表
            }

            if (!TextureSize.Contains(maxSize)) //自己的贴图组的Size表
            {
                // TextureSize.Add(maxSize);
                var b = false;

                for (int j = 0; j < TextureSize.Count; j++) //排序小到大
                {
                    if (maxSize < TextureSize[j])
                    {
                        TextureSize.Insert(j, maxSize);
                        b = true;
                        break;
                    }
                }

                if (!b)
                {
                    TextureSize.Add(maxSize);
                }
            }

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

            TextureBoxs.Add(new TextureBoxs()
            {
                Texture = texture, //贴图
                TimeInfo = new FileInfo(AssetDatabase.GUIDToAssetPath(guid[i])).LastWriteTime
            });
            LodIndex++;

            isSelect.Add(false);
            if (i % 60 == 0 && i != 0) //加载300个每帧
            {
                //Debug.Log(path+" : " + i);
                SelectTextureWindow.RefreshFilter();//刷新筛选数据，跑一下筛选 
                yield return null;
            }



        }
        IsGetOver = true;
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

}