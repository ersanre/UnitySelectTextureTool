using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomMaterialPropertyDrawer : MaterialPropertyDrawer
{
    float height;
    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
         if (prop.type == MaterialProperty.PropType.Texture)
            {
				//绘制UI
              //  editor.TexturePropertySingleLine(label, prop);
              GUILayout.BeginHorizontal();
                //editor.TextureProperty(prop,"ss");

                //editor.DefaultPreviewGUI()
                GUILayout.Button("dd");
                GUILayout.EndHorizontal();
				//如果没有贴图，不显示Tiling Offset，节省位置
                if (prop.textureValue != null)
                {
                    EditorGUI.indentLevel += 2;
                    editor.TextureScaleOffsetProperty(prop);
                    EditorGUI.indentLevel -= 2;
                }
            }

    }
    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
              if (prop.textureValue != null)
            {
                height = base.GetPropertyHeight(prop, label, editor);
            }
            else
            {
                height = 20;
            }
            return height;

    }
   
}
