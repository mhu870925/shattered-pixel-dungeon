using System.Collections.Generic;
using UnityEngine;

namespace ShatteredPixelDungeon.Utils
{
    /// <summary>
    /// Integer rectangle equivalent to com.watabou.utils.Rect.
    /// UnityEngine.RectInt를 쓰기 전에 원본 로직을 그대로 재현합니다.
    /// </summary>
    public class Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public Rect()
        {
            Set(0, 0, 0, 0);
        }

        public Rect(Rect rect)
        {
            Set(rect.left, rect.top, rect.right, rect.bottom);
        }

        public Rect(int left, int top, int right, int bottom)
        {
            Set(left, top, right, bottom);
        }

        public int Width() => right - left;

        public int Height() => bottom - top;

        public int Square() => Width() * Height();

        public Rect Set(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            return this;
        }

        public Rect Set(Rect rect) => Set(rect.left, rect.top, rect.right, rect.bottom);

        public Rect SetPos(int x, int y) => Set(x, y, x + (right - left), y + (bottom - top));

        public Rect Shift(int x, int y) => Set(left + x, top + y, right + x, bottom + y);

        public Rect Resize(int w, int h) => Set(left, top, left + w, top + h);

        public bool IsEmpty() => right <= left || bottom <= top;

        public Rect SetEmpty()
        {
            left = right = top = bottom = 0;
            return this;
        }

        public Rect Intersect(Rect other)
        {
            return new Rect
            {
                left = Mathf.Max(left, other.left),
                right = Mathf.Min(right, other.right),
                top = Mathf.Max(top, other.top),
                bottom = Mathf.Min(bottom, other.bottom)
            };
        }

        public Rect Union(Rect other)
        {
            return new Rect
            {
                left = Mathf.Min(left, other.left),
                right = Mathf.Max(right, other.right),
                top = Mathf.Min(top, other.top),
                bottom = Mathf.Max(bottom, other.bottom)
            };
        }

        public Rect Union(int x, int y)
        {
            if (IsEmpty())
            {
                return Set(x, y, x + 1, y + 1);
            }

            if (x < left)
            {
                left = x;
            }
            else if (x >= right)
            {
                right = x + 1;
            }

            if (y < top)
            {
                top = y;
            }
            else if (y >= bottom)
            {
                bottom = y + 1;
            }

            return this;
        }

        public Rect Union(Point p) => Union(p.x, p.y);

        public bool Inside(Point p)
        {
            return p.x >= left && p.x < right && p.y >= top && p.y < bottom;
        }

        public Point Center()
        {
            return new Point(
                (left + right) / 2 + (((right - left) % 2) == 0 ? Random.Int(2) : 0),
                (top + bottom) / 2 + (((bottom - top) % 2) == 0 ? Random.Int(2) : 0));
        }

        public Rect Shrink(int d) => new(left + d, top + d, right - d, bottom - d);

        public Rect Shrink() => Shrink(1);

        public Rect Scale(int d) => new(left * d, top * d, right * d, bottom * d);

        public List<Point> GetPoints()
        {
            var points = new List<Point>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    points.Add(new Point(i, j));
                }
            }

            return points;
        }
    }
}
