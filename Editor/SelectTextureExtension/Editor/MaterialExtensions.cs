using System.Collections.Generic;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public static class MaterialExtensions
    {
        /// <summary>
        /// 获取材质球上真实的贴图属性名
        /// </summary>
        /// <param name="material">材质球</param>
        /// <returns>真实的贴图属性名列表</returns>
     public  static List<string> GetTextureProperty(this Material material)
        {
            var textures = material.GetTexturePropertyNames();
            List<string> TexturNames=new List<string>();
            for (int i = 0; i < textures.Length; i++)
            {
                if (material.shader.FindPropertyIndex(textures[i]) != -1) //shader上有的才列出来
                {
                    TexturNames.Add(textures[i]);
                }
            }

            return TexturNames;
        }
    }
}