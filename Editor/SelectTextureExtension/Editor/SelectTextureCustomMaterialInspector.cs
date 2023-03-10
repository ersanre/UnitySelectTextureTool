using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YaoZiTools.SelectTextureExtension.Editor
{


    [CustomEditor(typeof(Material))]
    public class SelectTextureCustomMaterialInspector : MaterialEditor
    {
        Object texture;
        //  private List<Texture> Texture2Ds = new List<Texture>();
        private List<string> TexturNames = new List<string>();
        private List<string> TexturDescription = new List<string>();
        private Rect ButtonReact;
        private Material Material;
        public static event Action ButtomEvent;
        private ShaderGUI ShaderGUI = Activator.CreateInstance(typeof(CustomShaderGUI)) as ShaderGUI;
        public static FieldInfo MyCustomShaderGUI;
        [InitializeOnLoadMethod]
        private static void GetShaderGUI()
        {
            var o = typeof(MaterialEditor);
           SelectTextureCustomMaterialInspector.MyCustomShaderGUI = o.GetField("m_CustomShaderGUI", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }
        public override void OnEnable()
        {
            base.OnEnable();

            Material = target as Material;
            // base.PropertiesDefaultGUI(new MaterialProperty[]{new MaterialProperty(){textureValue = SelectTextureWindow.MyData.NowMaterial.GetTexture(SelectTextureWindow.MyData.NowTextruePropertyName)}});
            //Material = go;

            LoadTextrueProperty();

            //Debug.Log(s.va);
            //  var shaderg= new ShaderGUI();
            // var shaderU= typeof(ShaderUtil);
            // var create = shaderU.GetMethod("CreateShaderGUI",BindingFlags.Static|BindingFlags.NonPublic);
            //shaderGUI1 = Activator.CreateInstance(typeof(CustomShaderGUI)) as ShaderGUI;
            // Debug.Log(ShaderGUI);
            // var oo = create.Invoke(null,new object[]{"CustomShaderGUI"});
            // ShaderUtil.CreateShaderGUI()
             SelectTextureCustomMaterialInspector.MyCustomShaderGUI.SetValue(this, ShaderGUI);

            //     this.ShaderProperty(Material.GetColor("gg"),"ff")
            //    var propertyID=  ShaderUtil.GetPropertyCount(Material.shader);
            //    for (int i = 0; i < propertyID; i++)
            //    {
            //      ShaderUtil.GetPropertyName(Material.shader,propertyID);
            //    }
            //     materialProperty=new MaterialProperty();
            //     materialProperty.textureValue =  Material.GetTexture(TexturNames[0]);
            //     DrawTextureGroup.tc += LoadTextrueProperty;

        }


        public override void OnInspectorGUI()
        {
            //this.TextureProperty()
            // base.TextureProperty(materialProperty,"sgg");
            base.OnInspectorGUI();
            //Material = go;
            if (TexturNames != null)
            {

                GUILayout.BeginVertical();
                for (int i = 0; i < TexturNames.Count; i++)
                {
                    // Texture2Ds[i] = EditorGUILayout.ObjectField(TexturNames[i] ,Texture2Ds[i], typeof(Texture2D),false) as Texture2D;
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(TexturDescription[i] + ":");
                    if (GUILayout.Button(Material.GetTexture(TexturNames[i]), "Wizard Box", GUILayout.Width(70), GUILayout.Height(70)))
                    {

                        SelectTextureWindow.PropertyName = TexturNames[i];
                        // SelectTextureWindow.TexturPropertyNames.Clear();
                        //  SelectTextureWindow.TexturPropertyNames.AddRange(TexturNames);
                        SelectTextureWindow.Material = Material;
                      //  ButtomEvent?.Invoke();
                        //var MyData = AssetDatabase.LoadAssetAtPath<SelectTextureWindowData>(Data.DataPath);
                        SelectTextureWindow.Open();
                    }


                    GUILayout.EndHorizontal();
                    //Texture2Ds[i] = EditorGUI.DrawPreviewTexture(new Rect(0,0);
                }

                GUILayout.EndVertical();


            }

            //  EditorGUI.DrawRect(base.GetTexturePropertyCustomArea(new Rect(0, 0, 5, 50)), Color.green);
            //  base.TextureProperty(new MaterialProperty() { textureValue = SelectTextureWindow.MyData.NowMaterial.GetTexture(SelectTextureWindow.MyData.NowTextruePropertyName) }, "sss");

        }

        protected override void OnShaderChanged()
        {
            // Debug.Log("shader change");
            LoadTextrueProperty();
            // SelectTextureWindow.TexturPropertyNames.Clear();
            //  SelectTextureWindow.TexturPropertyNames.AddRange(TexturNames); ;
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
                if (ShaderUtil.GetPropertyType(Material.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {

                    TexturNames.Add(ShaderUtil.GetPropertyName(Material.shader, i));
                    TexturDescription.Add(ShaderUtil.GetPropertyDescription(Material.shader, i));
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