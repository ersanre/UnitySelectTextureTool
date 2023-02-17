using UnityEngine;

namespace EditorFramework
{
    public static class RectExtension
    {
        public enum AnchorType
        {
            UpperLeft = 0,
            UpperCenter = 1,
            UpperRight = 2,
            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,
            LowerLeft = 6,
            LowerCenter = 7,
            LowerRight = 8
        }

        public enum SplitType
        {
            Vertical,
            Horizontal
        }

        /// <summary>
        /// 以某个点为中心缩放rect块
        /// </summary>
        /// <param name="self"></param>一个rect
        /// <param name="pixels"></param>要缩放的像素
        /// <returns></returns>缩放后的rect
        public static Rect Zoom(this Rect self, float pixel, AnchorType anchorType)
        {
            Rect r = self;
            switch (anchorType)
            {
                case AnchorType.MiddleCenter:
                    r = self.CutLeft(-pixel * 0.5f)
                        .CutRigth(-pixel * 0.5f)
                        .CutTop(-pixel * 0.5f)
                        .CutBottom(-pixel * 0.5f);

                    break;
            }

            return r;
        }

        /// <summary>
        /// 求两个rect之间的区域
        /// </summary>
        /// <param name="rects"></param>rect数组，最多两个rect
        /// <returns></returns>两个rect中间的rect
        public static Rect GetMidTowRect(this Rect[] rects, SplitType splitType)
        {
            if (splitType == SplitType.Vertical)
            {
                return new Rect(rects[0].xMax, rects[0].yMin, rects[1].xMin - rects[0].xMax, rects[0].height);
            }
            else
            {
                return new Rect(rects[0].xMin, rects[0].yMax, rects[0].width, rects[1].yMin - rects[0].yMax);
            }
        }

        /// <summary>
        /// 总分割
        /// </summary>
        /// <param name="self"></param>
        /// <param name="splitType"></param>分割方式
        /// <param name="size"></param>以这个尺寸分割
        /// <param name="padding"></param>两个块之间的间隙
        /// <param name="justMid"></param>居中
        /// <returns></returns>两个rect块
        public static Rect[] Split(this Rect self, SplitType splitType, float size, float padding = 0,
            bool justMid = true)
        {
            if (splitType == SplitType.Vertical)
            {
                return VerticalSplit(self, size, padding, justMid);
            }
            else
            {
                return HorizontalSplit(self, size, padding, justMid);
            }
        }

        public static Rect[] Split(this Rect self, int count, SplitType splitType, float padding = 0,
            bool justMid = true)
        {
            Rect[] newRects = new Rect[count];

            if (splitType == SplitType.Vertical)
            {
                var rect = new Rect(0, 0, self.width, self.height / count);
                var newHeight = self.height / count;
                for (int i = 0; i < count; i++)
                {

                    newRects[i] = rect;
                    rect.y += newHeight;
                }
            }
            else
            {
                var rect = new Rect(self.x, self.y, self.width / count, self.height);
                var newWidth = self.width / count;
                for (int i = 0; i < count; i++)
                {

                    newRects[i] = rect;
                    rect.x += newWidth;
                }
            }

            return newRects;
        }

        /// <summary>
        /// 左右分割
        /// </summary>
        /// <param name="self"></param>一个rect块
        /// <param name="size"></param>以这个宽带分割
        /// <param name="padding"></param>两个块之间的间隙
        /// <param name="justMid"></param>是否居中
        /// <returns></returns>返回两个块
        public static Rect[] VerticalSplit(this Rect self, float size, float padding = 0, bool justMid = true)
        {
            if (justMid)
            {
                return new Rect[2]
                {
                    //??
                    //self.CutRigth(self.width-width).CutRigth(padding).CutRigth(Mathf.CeilToInt(padding/2f)),
                    // self.CutLeft(width).CutLeft(padding).CutLeft(-Mathf.CeilToInt(padding/2f))
                    self.CutRigth(self.width - size + padding * 0.5f),
                    self.CutLeft(size + padding * 0.5f),
                    //new Rect(width-padding*0.5f,self.y,padding,self.height)
                };
            }

            return new Rect[2]
            {
                new Rect(),
                new Rect()
            };
        }

        /// <summary>
        /// 上下分割
        /// </summary>
        /// <param name="self"></param>一个Rect块
        /// <param name="size"></param>以这个宽度裁切
        /// <param name="padding"></param>两个块之间的间隙
        /// <param name="justMid"></param>是否居中
        /// <returns></returns>返回两个块
        public static Rect[] HorizontalSplit(this Rect self, float size, float padding = 0, bool justMid = true)
        {
            if (justMid)
            {
                return new Rect[2]
                {
                    //??
                    //self.CutRigth(self.width-width).CutRigth(padding).CutRigth(Mathf.CeilToInt(padding/2f)),
                    // self.CutLeft(width).CutLeft(padding).CutLeft(-Mathf.CeilToInt(padding/2f))
                    self.CutBottom(self.height - size + padding * 0.5f),
                    self.CutTop(size + padding * 0.5f),
                    //new Rect(width-padding*0.5f,self.y,padding,self.height)
                };
            }

            return new Rect[2]
            {
                new Rect(),
                new Rect()
            };
        }

        //裁切某个部分
        public static Rect CutRigth(this Rect self, float pixels)
        {
            self.xMax -= pixels;
            return self;
        }

        public static Rect CutLeft(this Rect self, float pixels)
        {
            self.xMin += pixels;
            return self;
        }

        public static Rect CutTop(this Rect self, float pixels)
        {
            self.yMin += pixels; //?
            return self;
        }

        public static Rect CutBottom(this Rect self, float pixels)
        {
            self.yMax -= pixels;
            return self;
        }
    }
}