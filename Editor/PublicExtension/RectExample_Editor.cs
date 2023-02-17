

using UnityEditor;
using UnityEngine;

namespace EditorFramework
{


    public static class RectExample_Editor
    {
        public static Rect DrawOutLine(this Rect self, Color color, float pixel = 2f)
        {
            var mcolor = Handles.color;
            Handles.color = color;
            Handles.DrawAAPolyLine(pixel,
                new Vector3(self.xMin,self.yMin),
                            new Vector3(self.xMax,self.yMin),
                             new Vector3(self.xMax,self.yMax),
                                new Vector3(self.xMin,self.yMax),
                                 new Vector3(self.xMin,self.yMin));
            Handles.color = mcolor;
            return self;
        }
    }
}