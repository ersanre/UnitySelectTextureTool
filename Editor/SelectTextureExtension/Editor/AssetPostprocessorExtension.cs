using System.IO;
using UnityEditor;
using UnityEngine;
using YaoZiTools.SelectTextureExtension.Editor;

public class AssetPostprocessorExtension : AssetPostprocessor
{
/// <summary>
/// 有贴图文件导入时，将路径包含贴图路径的文件夹的lod状态置成false//删除时也会调用这个
/// </summary>
/// <param name="texture"></param>
    private void OnPostprocessTexture(Texture2D texture)
    {
        if (SelectTextureWindow.MyData==null)
        {
            return;
        }
        for (int i = 0; i < SelectTextureWindow.MyData.Paths.Count; i++)
        {
            if (assetPath.Contains(SelectTextureWindow.MyData.Paths[i])&&SelectTextureWindow.DrawTextures[i]!=null)
            {
              SelectTextureWindow.DrawTextures[i].IsLoad=false;
            }
        }

    }
}