using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YaoZiTools.SelectTextureExtension.Editor
{
    //提供一个 按照索引选取，和条件选取的方法, 单选，多选 与或条件
    public class SizeFilterPopupWindow<T> : PopupWindowContent
    {
        private float mRectX;
        public event Action IsToggleChange;
        public static bool IsAllIsFalse
        {
            set
            {
                if (value)
                {
                    PropetrtySelect = PropetrtySelect.ToDictionary(k => k.Key, v => false);
                    // SelectTextureWindow.RefreshFilter();
                }

            }
            get { return PropetrtySelect.ContainsValue(true) ? false : true; }
        }

        public List<T> Property = new List<T>();

        private static Dictionary<T, bool> _propetrtySelect = new Dictionary<T, bool>();
        public static Dictionary<T, bool> PropetrtySelect
        {
            get { return _propetrtySelect; }
            private set { _propetrtySelect = value; }
        }
        private T TempValue;
        public bool MultipleSelect;

        public override void OnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical();
            for (int i = 0; i < Property.Count; i++)
            {
                PropetrtySelect[Property[i]] = GUILayout.Toggle(PropetrtySelect[Property[i]], Property[i].ToString());
                if (PropetrtySelect[Property[i]] && MultipleSelect)
                {
                    if (!Property[i].Equals(TempValue) && TempValue != null)
                    {

                        PropetrtySelect[TempValue] = false;
                    }
                    TempValue = Property[i];
                }
            }
            // SelectTextureWindow.MyData.TextureSizeTypes.Values.All(p => p);

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                IsToggleChange?.Invoke();
            }

        }
        /// <summary>
        /// toggle组弹出窗口构造函数
        /// </summary>
        /// <param name="t">显示的内容list表</param>
        /// <param name="windowWidht">窗口的宽度</param>
        /// <param name="multipleSelect">是否支持多选</param>
        public SizeFilterPopupWindow(List<T> t, float windowWidht, bool multipleSelect = false)
        {
            // PropetrtySelect = new Dictionary<T, bool>();
            MultipleSelect = multipleSelect;
            mRectX = windowWidht;
            Property = t;
            for (int i = 0; i < t.Count; i++)
            {
                //不包含才会加进字典
                if (!PropetrtySelect.ContainsKey(t[i]))
                {
                    PropetrtySelect.Add(t[i], false);
                }
            }

            //list不包含key时将value设为false
            PropetrtySelect = PropetrtySelect.ToDictionary(k => k.Key, v => t.Contains(v.Key) ? v.Value : false);

        }
        public override Vector2 GetWindowSize()
        {
            return new Vector2(mRectX, Property.Count * 17 + 5);
        }
        // public static void SetPropertySelect(T Key, bool value)
        // {
        //     PropetrtySelect = PropetrtySelect.ToDictionary(k => k.Key, v => v.Key.Equals(Key) ? value : v.Value);
        //     SelectTextureWindow.RefreshFilter();//不要每次都刷
        // }
        /// <summary>
        /// 设置toggle组的选项
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        public void SetPropertySelect(List<T> Key, bool value)
        {
            for (int i = 0; i < Key.Count; i++)
            {
                PropetrtySelect = PropetrtySelect.ToDictionary(k => k.Key, v => v.Key.Equals(Key[i]) ? value : v.Value);
            }
            IsToggleChange?.Invoke();
        }
        public void SetPropertySelect(Func<List<T>, List<T>> func, bool value)
        {
            var keys = func(Property);
            SetPropertySelect(keys, value);
        }

    }
}