using System.Drawing;
using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    public class TextureTestTool : TextureTools
    {
        private GUIContent myGUIContent = new GUIContent() { image = Resources.Load<Texture2D>("TextureTool_TextureRGBA"), tooltip = "RGBA" };
        protected override GUIContent ToogleContent { get { return myGUIContent; } set { myGUIContent = value; } }

        protected override void OnDispose()
        {
            DrawTextureGroup.IsTextureChange -= SetTextureBox;
            SelectTextureWindow.IsChangeMaterialTexture = true;
        }
        private bool[] ToggleValues = new bool[5] { false, false, false, false, false };
        private Rect[] TextureRects = new Rect[5];
        private Texture2D[] Textures = new Texture2D[5];
        private string[] RGBAButtonText = new string[7] { "R", "G", "B", "A", "灰", "黑", "白" };
        private string[] TipsText = new string[4] { "+", "+", "+", "=" };
        private GUIContent[] ButtonContents = new GUIContent[5]
        {
            new GUIContent(){text ="R"},
            new GUIContent(){text ="G"},
            new GUIContent(){text ="B"},
            new GUIContent(){text ="A"},
            new GUIContent(){text ="Result"},
        };

        private int tempIndex = -1;
        private int[] tempint = new int[] { 0, 0, 0, 0 };
        Texture2D RTexture;
        Texture2D GTexture;
        Texture2D BTexture;
        Texture2D ATexture;
        Texture2D ResultTexture;
        GUIStyle gUIStyle;
        GUIStyle TextGUIStyle;
        private int SizeInt = 1;


        private Material[] DrawMaterial = new Material[5] {
             new Material(Shader.Find("TextureToolRGBAShader")),
              new Material(Shader.Find("TextureToolRGBAShader")),
              new Material(Shader.Find("TextureToolRGBAShader")),
              new Material(Shader.Find("TextureToolRGBAShader")),
               new Material(Shader.Find("TextureToolRGBAShader")) };
        public override void OnGUI(Rect position)
        {
            var textureSize = Mathf.Clamp(position.height, 40, 200) - 25;
            //  GUILayout.BeginArea(position);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < Textures.Length; i++)
            {
                GUILayout.BeginVertical();
                ToggleValues[i] = GUILayout.Toggle(ToggleValues[i], ButtonContents[i], gUIStyle, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
                if (ToggleValues[i] && tempIndex != i)
                {
                    if (tempIndex != -1)
                    {
                        ToggleValues[tempIndex] = false;
                    }

                    tempIndex = i;
                }
                if (Event.current.type == EventType.Repaint)
                {
                    TextureRects[i] = GUILayoutUtility.GetLastRect();
                }
                if (Textures[i] != null)
                {

                    // m.SetTexture("_MainTexture", Textures[i]);
                    // m.SetFloat("_U", UValue);
                    var tempM = 0f;
                    if (Textures[i].width > Textures[i].height)
                    {
                        tempM = (float)Textures[i].height / Textures[i].width;
                        TextureRects[i].y += (1 - tempM) / 2 * TextureRects[i].height;

                        TextureRects[i].height *= tempM;

                    }
                    else
                    {
                        tempM = (float)Textures[i].width / Textures[i].height;
                        TextureRects[i].x += (1 - tempM) / 2 * TextureRects[i].width;
                        TextureRects[i].width *= tempM;

                    }
                    Graphics.DrawTexture(TextureRects[i], Textures[i], new Rect(0, 0, 1, 1), 0, 0, 0, 0, GUI.color, DrawMaterial[i]);
                }
                //选择框
                if (ToggleValues[i])
                {
                    DrawTextureGroup.DarwLine(TextureRects[i], 4, Color.yellow);
                }
                // GUILayout.BeginHorizontal(GUILayout.Width(position.height - 20));
                if (i < 4)
                {
                    EditorGUI.BeginChangeCheck();
                    tempint[i] = GUILayout.Toolbar(tempint[i], RGBAButtonText, GUILayout.Width(textureSize));

                    DrawMaterial[i].SetFloat("_CustomValue", tempint[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        ResetResultTexture(i);
                    }

                }

                GUILayout.EndVertical();
                if (i < 4)
                {
                    GUILayout.Label(TipsText[i], TextGUIStyle, GUILayout.Height(textureSize));
                }
            }
            if (GUILayout.Button("使用"))
            {
                SelectTextureWindow.SetTextureInMaterial(ResultTexture);
            }

            GUILayout.Label("输出尺寸:");
            SizeInt = EditorGUILayout.Popup(SizeInt, new string[] { "128", "256", "512", "1024" });
            GUILayout.Label("输出格式:");
            EditorGUILayout.Popup(0, new string[] { "png", "jpg", "tga" });
            if (GUILayout.Button("Save"))
            {
                var size = 0;
                switch (SizeInt)
                {
                    case 0:
                        size = 128;
                        break;
                    case 1:
                        size = 256;
                        break;
                    case 2:
                        size = 512;
                        break;
                    case 3:
                        size = 1024;
                        break;
                }
                var path = EditorUtility.SaveFilePanel("选择保存路径", "", "CustomRGBATexture", "png");
                if (path != "")
                {
                    ResultTexture = SetTextureRGBA(
                 RTexture != null ? RTexture : Texture2D.blackTexture,
                 GTexture != null ? GTexture : Texture2D.blackTexture,
                 BTexture != null ? BTexture : Texture2D.blackTexture,
                 ATexture != null ? ATexture : Texture2D.blackTexture,
                 size, size);
                    SelectTextureWindow.SaveTextureInAssets(ResultTexture, path);
                }
            }
            GUILayout.EndHorizontal();
        }
        Texture2D textureR;
        Texture2D textureG;
        Texture2D textureB;
        Texture2D textureA;
        Texture GetTexture(Texture texture, Material material)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, tmp, material);

            RenderTexture previous = RenderTexture.active;

            Texture2D myTexture2D = new Texture2D(texture.width, texture.height);

            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();

            RenderTexture.active = previous;

            RenderTexture.ReleaseTemporary(tmp);
            return myTexture2D;

        }
        public override void OnEnable()
        {
            //贴图切换不改变材质球贴图
            SelectTextureWindow.IsChangeMaterialTexture = false;
            DrawTextureGroup.IsTextureChange += SetTextureBox;
            gUIStyle = new GUIStyle("AppToolbarButtonMid") { fontSize = 70, fontStyle = FontStyle.Bold, };
            gUIStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            TextGUIStyle = new GUIStyle() { fontSize = 50, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0) };
            TextGUIStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            //gUIStyle.margin= new RectOffset(0,0,0,0);
            // DrawMaterial.SetColor("_MaskColor", new Color(1, 1, 1, 1));
        }
        private TextureBox textureBox;
        public static Texture2D ScaleTexture(Texture2D texture2D, int targetWidth, int targetHeight)
        {
            var result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);

            for (int i = 0; i < targetHeight; i++)
            {
                for (int j = 0; j < targetWidth; j++)
                {
                    var color = texture2D.GetPixelBilinear((float)j / targetWidth, (float)i / targetHeight);
                    result.SetPixel(j, i, color);
                }
            }

            return result;
        }
        public static Texture2D SetTextureRGBA(Texture2D rTexture, Texture2D gTexture, Texture2D bTexture, Texture2D aTexture, int targetWidth, int targetHeight)
        {
            // gTexture.alphaIsTransparency=true;
            var result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
            Color[] resultColors = new Color[targetWidth * targetHeight];
            var cccc = gTexture.GetPixels();
            for (int i = 0; i < targetHeight; i++)
            {
                for (int j = 0; j < targetWidth; j++)
                {
                    var u = (float)j / (float)targetWidth;
                    var v = (float)i / (float)targetHeight;
                    var rColor = rTexture.GetPixelBilinear(u, v);
                    var gColor = gTexture.GetPixelBilinear(u, v);
                    var bColor = bTexture.GetPixelBilinear(u, v);
                    var aColor = aTexture.GetPixelBilinear(u, v);
                    resultColors[i * targetWidth + j] = new Color(rColor.r, gColor.r, bColor.r, aColor.r);
                }
            }
            result.SetPixels(resultColors);
            //不调用这个函数，set和get不会生效
            result.Apply();

            return result;
        }
        private void SetTextureBox(TextureBox obj)
        {
            textureBox = obj;
            if (tempIndex != -1 && ToggleValues[tempIndex])
            {
                if (tempIndex != 4)
                {
                    Textures[tempIndex] = obj.Texture;
                    DrawMaterial[tempIndex].SetTexture("_MainTexture", obj.Texture);
                    ResetResultTexture(tempIndex);
                }
                else
                {
                    //设置贴图
                    for (int i = 0; i < 4; i++)
                    {
                        Textures[i] = obj.Texture;
                        DrawMaterial[i].SetTexture("_MainTexture", obj.Texture);
                        tempint[i] = i;
                        DrawMaterial[i].SetFloat("_CustomValue", i);
                    }
                    ResetResultTexture();
                }

            }
        }

        private void ResetResultTexture(int tempIndex)
        {
            switch (tempIndex)
            {
                case 0:
                    if (Textures[0] != null)
                    {
                        RTexture = GetTexture(Textures[0], DrawMaterial[0]) as Texture2D;
                    }
                    break;
                case 1:
                    if (Textures[1] != null)
                    {
                        GTexture = GetTexture(Textures[1], DrawMaterial[1]) as Texture2D;
                    }
                    break;
                case 2:
                    if (Textures[2] != null)
                    {
                        BTexture = GetTexture(Textures[2], DrawMaterial[2]) as Texture2D;
                    }
                    break;
                case 3:
                    if (Textures[3] != null)
                    {
                        ATexture = GetTexture(Textures[3], DrawMaterial[3]) as Texture2D;
                    }
                    break;
            }
            ResultTexture = SetTextureRGBA(
                RTexture != null ? RTexture : Texture2D.blackTexture,
                 GTexture != null ? GTexture : Texture2D.blackTexture,
                  BTexture != null ? BTexture : Texture2D.blackTexture,
                  ATexture != null ? ATexture : Texture2D.blackTexture,
                256, 256);

            ButtonContents[4].text = string.Empty;
            ButtonContents[4].image = ResultTexture;
        }
        private void ResetResultTexture()
        {
            if (Textures[0] != null)
            {
                RTexture = GetTexture(Textures[0], DrawMaterial[0]) as Texture2D;
            }
            if (Textures[1] != null)
            {
                GTexture = GetTexture(Textures[1], DrawMaterial[1]) as Texture2D;
            }
            if (Textures[2] != null)
            {
                BTexture = GetTexture(Textures[2], DrawMaterial[2]) as Texture2D;
            }
            if (Textures[3] != null)
            {
                ATexture = GetTexture(Textures[3], DrawMaterial[3]) as Texture2D;
            }


            ResultTexture = SetTextureRGBA(
                RTexture != null ? RTexture : Texture2D.blackTexture,
                 GTexture != null ? GTexture : Texture2D.blackTexture,
                  BTexture != null ? BTexture : Texture2D.blackTexture,
                  ATexture != null ? ATexture : Texture2D.blackTexture,
                256, 256);

            ButtonContents[4].text = string.Empty;
            ButtonContents[4].image = ResultTexture;
        }
    }
}