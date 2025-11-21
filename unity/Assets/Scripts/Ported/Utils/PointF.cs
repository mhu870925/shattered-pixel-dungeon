using System;
using UnityEngine;

namespace ShatteredPixelDungeon.Utils
{
    /// <summary>
    /// Float 2D point equivalent to com.watabou.utils.PointF.
    /// Mathf 기반 삼각함수/루트 연산을 그대로 제공합니다.
    /// </summary>
    [Serializable]
    public class PointF
    {
        public const float PI = 3.1415926f;
        public const float PI2 = PI * 2f;
        public const float G2R = PI / 180f;

        public float x;
        public float y;

        public PointF()
        {
        }

        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public PointF(PointF p)
        {
            x = p.x;
            y = p.y;
        }

        public PointF(Point p)
        {
            x = p.x;
            y = p.y;
        }

        public PointF Clone()
        {
            return new PointF(this);
        }

        public PointF Scale(float f)
        {
            x *= f;
            y *= f;
            return this;
        }

        public PointF InvScale(float f)
        {
            x /= f;
            y /= f;
            return this;
        }

        public PointF Set(float x, float y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        public PointF Set(PointF p)
        {
            x = p.x;
            y = p.y;
            return this;
        }

        public PointF Set(float v)
        {
            x = v;
            y = v;
            return this;
        }

        public PointF Polar(float a, float l)
        {
            x = l * Mathf.Cos(a);
            y = l * Mathf.Sin(a);
            return this;
        }

        public PointF Offset(float dx, float dy)
        {
            x += dx;
            y += dy;
            return this;
        }

        public PointF Offset(PointF p)
        {
            x += p.x;
            y += p.y;
            return this;
        }

        public PointF Negate()
        {
            x = -x;
            y = -y;
            return this;
        }

        public PointF Normalize()
        {
            var l = Length();
            if (l != 0f)
            {
                x /= l;
                y /= l;
            }
            return this;
        }

        public Point Floor()
        {
            return new Point((int)x, (int)y);
        }

        public bool IsZero()
        {
            return Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f);
        }

        public float Length()
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public static PointF Sum(PointF a, PointF b)
        {
            return new PointF(a.x + b.x, a.y + b.y);
        }

        public static PointF Diff(PointF a, PointF b)
        {
            return new PointF(a.x - b.x, a.y - b.y);
        }

        public static PointF Inter(PointF a, PointF b, float d)
        {
            return new PointF(a.x + (b.x - a.x) * d, a.y + (b.y - a.y) * d);
        }

        public static float Distance(PointF a, PointF b)
        {
            var dx = a.x - b.x;
            var dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static float Angle(float x, float y)
        {
            return Mathf.Atan2(y, x);
        }

        public static float Angle(PointF start, PointF end)
        {
            return Mathf.Atan2(end.y - start.y, end.x - start.x);
        }

        public Vector2 ToVector2() => new(x, y);

        public static PointF FromVector2(Vector2 v) => new(v.x, v.y);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return obj is PointF p && Mathf.Approximately(p.x, x) && Mathf.Approximately(p.y, y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }
    }
}
