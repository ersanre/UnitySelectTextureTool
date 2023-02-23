using System.Collections.Generic;
using System.IO;
using CopyMaterial.Editor;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class Data
    {
        //  public static  string DataPath ;
        public SelectTextureWindowData SelectTextureWindowData;
        // public Data()
        // {
        //     SelectTextureWindowData =  AssetDatabase.LoadAssetAtPath<SelectTextureWindowData>(DataPath);
        // }


        /// <summary>
        /// 添加 路径和名字数据
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="name">别名</param>
        public void SavePathDataInAsset(string path, string name = "null")
        {
            if (name == "null")//没写名字就读取文件夹的名字
            {
                name = path.Substring(path.LastIndexOf('/') + 1);
            }
            SelectTextureWindowData.Names.Add(name);
            SelectTextureWindowData.Paths.Add(path);
        }

        /// <summary>
        /// 创建默认数据，默认添加文件夹“Assets”名字“Assets”，，data创建在Assets
        /// </summary>
        /// <returns>返回成功创建数据的全路径</returns>
        public static string CreateDefaultData(Material material)
        {
            string dataPath = GetDataPath();
            SelectTextureWindowData data = ScriptableObject.CreateInstance<SelectTextureWindowData>();
            data.Names = new List<string>();
            data.Paths = new List<string>();
            data.SeachString = new List<string>();
            //data.NowMaterial=material;
            data.TextureSize = 85f;
            data.Names.Add("Assets");
            data.Paths.Add("Assets");
            data.WindowBackgroundColor = new Color(0, 0, 0, 0);

            AssetDatabase.CreateAsset(data, dataPath + "/SelectTextureWindowData.asset");
            AssetDatabase.Refresh(); //刷新
            return dataPath;
        }

        public static string GetDataPath()
        {
            var mIcon = Resources.Load<Texture2D>("SelectTextureWindowIcon");
            var dataPath = AssetDatabase.GetAssetPath(mIcon);
            dataPath = dataPath.Substring(0, dataPath.Length - 28);
            return dataPath;
        }

        /// <summary>
        /// 尝试获取数据路径，如果有就返回true并额外返回全路径
        /// </summary>
        /// <param name="path">数据全路径</param>
        /// <returns>是否存在数据</returns>
        // public  static bool TryGetDataPath(out string path)
        // {
        //     var s = GetPath.GetScriptPath("SelectTextureWindow");
        //     if (s != null)
        //     {
        //         if (File.Exists(s + @"/SelectTextureWindowData.asset"))
        //         {
        //             path = s + @"/SelectTextureWindowData.asset";
        //             return true;
        //         }
        //         else if (File.Exists("Assets/SelectTextureWindowData.asset"))
        //         {
        //             path = "Assets/SelectTextureWindowData.asset";
        //             return true;
        //         }
        //     }
        //
        //     path = null;
        //     return false;
        // }

        /// <summary>
        /// 读取数据并返回其中的数据
        /// </summary>
        /// <param name="path">数据全路径</param>
        /// <param name="names">返回的名字数组</param>
        /// <param name="paths">返回的路径数组</param>
        // public  void ReadData(ref List<string> names, ref List<string> paths, ref float pix)
        // {
        //     names.AddRange(SelectTextureWindowData.Names);
        //     paths.AddRange(SelectTextureWindowData.Paths);
        //     pix = SelectTextureWindowData.TextureSize;

        // }

        // public  void ChangeTextureSizeData(float pix)
        // {
        //     SelectTextureWindowData data = AssetDatabase.LoadAssetAtPath<SelectTextureWindowData>(DataPath);

        //     data.TextureSize = pix;

        //     // AssetDatabase.DeleteAsset(dataPath);
        //     AssetDatabase.CreateAsset(data, DataPath);
        //     AssetDatabase.Refresh();
        // }



    }
}