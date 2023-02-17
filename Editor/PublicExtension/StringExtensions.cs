
using System.IO;
using UnityEditor.Experimental.Networking.PlayerConnection;
using UnityEngine;

namespace EditorFramework
{


    public static class StringExtensions
    {
       public static string ToAssetPath(this  string self)
        {
            if (self.Contains(Application.dataPath))
            {
                return "Assets" + self.Substring(Application.dataPath.Length, self.Length - Application.dataPath.Length);
            }
            
            return "请选择工程内的文件";
        }
       public static string TryToAssetPath(this  string self,out bool Result)
       {
           if (self.Contains(Application.dataPath))
           {
               Result = true;
               return "Assets" + self.Substring(Application.dataPath.Length, self.Length - Application.dataPath.Length);
           }
           Result = false;
           return "请选择工程内的文件";
       }

       public static bool IsDirectory(this string self)
       {
           FileInfo fileInfo = new FileInfo(self);
           if ((fileInfo.Attributes & FileAttributes.Directory) !=0)
           {
               return true;
           }
           return false;
           
       }
    }
}