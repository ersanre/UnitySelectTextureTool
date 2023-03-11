using System.Dynamic;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class SelectTextureWindowData : ScriptableObject
    {
        public float TextureSize;
        public Material NowMaterial;
        public string NowTextruePropertyName;
        public List<string> Paths;
        public List<string> Names;
        public List<Material> Materials;
        //public List<int> TextureSizeTypes;
        public Dictionary<int, bool> TextureSizeTypes;
        public Dictionary<TextureWrapMode, bool> TextureWrapModes;
        public List<string> SeachString;
        public Color WindowBackgroundColor = Color.black;
        public Texture WindowBackgroundTexture;
        public Color SelectColor = Color.yellow;
        public float SplitSize;

    }

}