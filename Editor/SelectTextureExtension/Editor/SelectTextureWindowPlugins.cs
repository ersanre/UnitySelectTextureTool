using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YaoZiTools.SelectTextureExtension.Editor
{
    public abstract class SelectTextureWindowPlugins
    {
        /// <summary>
        /// 工具名称，在设置面板上显示的名字
        /// </summary>
        /// <value></value>
       public abstract string  PluginName{set;get;}
       /// <summary>
       /// 工具提示或简介：设置面板上显示
       /// </summary>
       /// <value></value>
       public abstract string PluginTips{set;get;}
       /// <summary>
       /// 插件启用状态
       /// </summary>
       /// <value></value>
       public abstract bool IsEnlabe{set;get;}
       public abstract void Draw();

    }
}