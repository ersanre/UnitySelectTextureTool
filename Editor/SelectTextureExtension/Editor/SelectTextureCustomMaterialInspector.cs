using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YaoZiTools.SelectTextureExtension.Editor
{


    [CustomEditor(typeof(Material))]
    public class SelectTextureCustomMaterialInspector : CopyMaterial.Editor.CustomMaterialInspector
    {
        Object texture;
      //  private List<Texture> Texture2Ds = new List<Texture>();
        private List<string> TexturNames = new List<string>();
        private List<string> TexturDescription = new List<string>();
        private Rect ButtonReact;
        private Material Material;
        public static event Action ButtomEvent;
        MaterialProperty materialProperty;


        public override void OnEnable()
        {
            base.OnEnable();

           Material = target as Material;
            //Material = go;
            LoadTextrueProperty();

       //     DrawTextureGroup.tc += LoadTextrueProperty;

        }

        public override void OnInspectorGUI()
        {
            //this.TextureProperty()
            base.OnInspectorGUI();
            if (TexturNames != null)
            {

                GUILayout.BeginVertical();
                for (int i = 0; i < TexturNames.Count; i++)
                {
                    // Texture2Ds[i] = EditorGUILayout.ObjectField(TexturNames[i] ,Texture2Ds[i], typeof(Texture2D),false) as Texture2D;
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(TexturDescription[i]+":");
                    if (GUILayout.Button(Material.GetTexture(TexturNames[i]),"Wizard Box", GUILayout.Width(70), GUILayout.Height(70)))
                    {

                        SelectTextureWindow.PropertyName = TexturNames[i];
                        SelectTextureWindow.TexturPropertyNames.Clear();
                        SelectTextureWindow.TexturPropertyNames.AddRange(TexturNames);
                        SelectTextureWindow.Material = Material;
                        ButtomEvent?.Invoke();
                        //var MyData = AssetDatabase.LoadAssetAtPath<SelectTextureWindowData>(Data.DataPath);
                        SelectTextureWindow.Open();                       
                    }

                    GUILayout.EndHorizontal();
                    //Texture2Ds[i] = EditorGUI.DrawPreviewTexture(new Rect(0,0);
                }

                GUILayout.EndVertical();


            }

        }

        protected override void OnShaderChanged()
        {
           // Debug.Log("shader change");
            LoadTextrueProperty();
            SelectTextureWindow.TexturPropertyNames.Clear();
            SelectTextureWindow.TexturPropertyNames.AddRange(TexturNames); ;
            SelectTextureWindow.IsSelectTexture = TexturNames.ToDictionary(a => a, b => false);
            OnInspectorGUI();
        }

        void LoadTextrueProperty()
        {
           // Texture2Ds.Clear();
            TexturNames.Clear();
            TexturDescription.Clear();
            var textures = Material.GetTexturePropertyNames();
         //  Debug.Log(ShaderUtil.GetPropertyCount(Material.shader)) ;

           for (int i = 0; i < ShaderUtil.GetPropertyCount(Material.shader); i++)
           {
              if (ShaderUtil.GetPropertyType(Material.shader,i)==ShaderUtil.ShaderPropertyType.TexEnv)
              {
                
                TexturNames.Add(ShaderUtil.GetPropertyName(Material.shader,i));
                TexturDescription.Add(ShaderUtil.GetPropertyDescription(Material.shader,i));
              }
           }
           // Material.shader.GetPropertyTextureDimension();
            // for (int i = 0; i < textures.Length; i++)
            // {
                
            //     //Debug.Log(textures[i]+":"+ Material.shader.FindPropertyIndex(textures[i]));
            //    var id = Material.shader.FindPropertyIndex(textures[i]);

            //     if (id != -1) //shader上有的才列出来
            //     {
            //       // Debug.Log( Material.shader.GetPropertyName(id));
            //      //  Debug.Log(Material.shader.GetPropertyDescription(id)) ;
            //         //Texture2Ds.Add(Material.GetTexture(textures[i]));
            //         TexturNames.Add(textures[i]);
            //     }
            // }

        }

        private void OnDestroy()
        {
     //       DrawTextureGroup.tc -= LoadTextrueProperty;
        }
    }
}