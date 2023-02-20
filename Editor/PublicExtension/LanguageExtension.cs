
using UnityEngine;

public static class LanguageExtension
{
    public static bool IsChinese//是繁体中文或者简体中文返回true
    {
        get
        {
            if (IsChineseTW() || IsChineseSimple())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    //当前操作系统是否为简体中文
    public static bool IsChineseSimple()
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN";
    }

    //当前操作系统是否为繁体中文
    public static bool IsChineseTW()
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.Name == "Zh-TW";
    }


}
