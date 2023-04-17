using System.ComponentModel.Design;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using YaoZiTools.SelectTextureExtension.Editor;
using UnityEngine.Rendering;

public class CustomShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        //复制  UnityEditor.MaterialEditor.PropertiesDefaultGUI的代码， 在贴图类型绘制按钮，其他使用默认渲染

        EditorGUIUtility.fieldWidth = 64f;
        EditorGUIUtility.labelWidth = Screen.width - EditorGUIUtility.fieldWidth - 40f;


        // float textureRectY = 0;
        // for (int i = 0; i < properties.Length; i++)
        // {
        //     var tempPropertyHeight = materialEditor.GetPropertyHeight(properties[i], properties[i].displayName);
        //    // Rect controlRect = EditorGUILayout.GetControlRect(true, tempPropertyHeight, EditorStyles.layerMaskField);
        //     textureRectY += tempPropertyHeight;
            
        //     if (properties[i].type == MaterialProperty.PropType.Texture)
        //     {
        //         Rect ncontrolRect = new Rect(0, textureRectY-tempPropertyHeight, Screen.width, tempPropertyHeight);
                

        //         //controlRect.yMax = EditorGUILayout.GetControlRect(true, tempPropertyHeight, EditorStyles.layerMaskField);
        //         EditorGUI.DrawRect(ncontrolRect, Color.yellow);
        //     }
           

       // }
        for (int i = 0; i < properties.Length; i++)
        {
            if ((properties[i].flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) == 0)
            {
                float propertyHeight = materialEditor.GetPropertyHeight(properties[i], properties[i].displayName);
                Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField);
                if (properties[i].type == MaterialProperty.PropType.Texture)
                {
                    materialEditor.ShaderProperty(controlRect, properties[i], properties[i].displayName);
                    //GUILayout.BeginArea(controlRect);
                    var selectRect = controlRect;
                    selectRect.width = 50;
                    selectRect.height = 15;
                    selectRect.x = controlRect.xMax - 115;
                    // selectRect.y=controlRect.yMax-60;
                    //selectRect.yMax-=20;
                    if (GUI.Button(selectRect, "Select"))
                    {
                        SelectTextureWindow.PropertyName = properties[i].name;
                        SelectTextureWindow.Material = materialEditor.target as Material;
                        SelectTextureWindow.Open();
                    }
                }
                else
                {
                    materialEditor.ShaderProperty(controlRect, properties[i], properties[i].displayName);
                }

            }

        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (SupportedRenderingFeatures.active.editableMaterialRenderQueue)
        {
            materialEditor.RenderQueueField();
        }
        materialEditor.EnableInstancingField();
        materialEditor.DoubleSidedGIField();

    }

}