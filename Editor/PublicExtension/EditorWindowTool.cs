using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace EditorFramework.Editor
{
    public static class EditorWindowTool
    {
        public static Rect LocalPosition(this EditorWindow self)
        {
            return new Rect(0, 0, self.position.width, self.position.height);
        }
    }
}