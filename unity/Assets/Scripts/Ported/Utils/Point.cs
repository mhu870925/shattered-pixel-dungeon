using System;
using UnityEngine;

namespace ShatteredPixelDungeon.Utils
{
    /// <summary>
    /// Integer 2D point equivalent to com.watabou.utils.Point (Java).
    /// Unity Vector2Int으로 변환/생성하기 쉽도록 최소 연산을 그대로 제공합니다.
    /// </summary>
    [Serializable]
    public class Point
    {
        public int x;
        public int y;

        public Point()
        {
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point p)
        {
            x = p.x;
            y = p.y;
        }

        public Point Set(int x, int y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        public Point Set(Point p)
        {
            x = p.x;
            y = p.y;
            return this;
        }

        public Point Clone()
        {
            return new Point(this);
        }

        public Point Scale(float f)
        {
            x = (int)(x * f);
            y = (int)(y * f);
            return this;
        }

        public Point Offset(int dx, int dy)
        {
            x += dx;
            y += dy;
            return this;
        }

        public Point Offset(Point d)
        {
            x += d.x;
            y += d.y;
            return this;
        }

        public bool IsZero()
        {
            return x == 0 && y == 0;
        }

        public float Length()
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public static float Distance(Point a, Point b)
        {
            var dx = a.x - b.x;
            var dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public Vector2Int ToVector2Int() => new(x, y);

        public static Point FromVector2Int(Vector2Int v) => new(v.x, v.y);

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return p.x == x && p.y == y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }
    }
}
