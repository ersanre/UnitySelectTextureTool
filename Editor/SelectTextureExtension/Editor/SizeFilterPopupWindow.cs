using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    //提供一个 按照索引选取，和条件选取的方法
    public class SizeFilterPopupWindow<T> : PopupWindowContent
    {
        private float mRectX;

        public static bool IsAllIsFalse
        {
            set
            {
                if (value)
                {
                    PropetrtSelect = PropetrtSelect.ToDictionary(k => k.Key, v => false);
                }

            }
            get { return PropetrtSelect.ContainsValue(true) ? false : true; }
        }

        public List<T> Property = new List<T>();
        public static Dictionary<T, bool> PropetrtSelect = new Dictionary<T, bool>();

        public override void OnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical();
            for (int i = 0; i < Property.Count; i++)
            {

                PropetrtSelect[Property[i]] = GUILayout.Toggle(PropetrtSelect[Property[i]], Property[i].ToString());

            }

            // SelectTextureWindow.MyData.TextureSizeTypes.Values.All(p => p);

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                SelectTextureWindow.RefreshFilter();//刷新筛选
            }

        }
        /// <summary>
        /// toggle组弹出窗口构造函数
        /// </summary>
        /// <param name="t">显示的内容list表</param>
        /// <param name="windowWidht">窗口的宽度</param>
        public SizeFilterPopupWindow(List<T> t, float windowWidht)
        {
            mRectX = windowWidht;
            Property = t;
            for (int i = 0; i < t.Count; i++)
            {
                //不包含才会加进字典
                if (!PropetrtSelect.ContainsKey(t[i]))
                {
                    PropetrtSelect.Add(t[i], false);
                }
            }

            //list不包含key时将value设为false
            PropetrtSelect = PropetrtSelect.ToDictionary(k => k.Key, v => t.Contains(v.Key) ? v.Value : false);

        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(mRectX, Property.Count * 17 + 5);
        }
    }
}