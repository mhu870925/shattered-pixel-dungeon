using UnityEngine;

namespace ShatteredPixelDungeon.Utils
{
    /// <summary>
    /// Float rectangle equivalent to com.watabou.utils.RectF.
    /// </summary>
    public class RectF
    {
        public float left;
        public float top;
        public float right;
        public float bottom;

        public RectF()
        {
            Set(0, 0, 0, 0);
        }

        public RectF(RectF rect)
        {
            Set(rect.left, rect.top, rect.right, rect.bottom);
        }

        public RectF(Rect rect)
        {
            Set(rect.left, rect.top, rect.right, rect.bottom);
        }

        public RectF(float left, float top, float right, float bottom)
        {
            Set(left, top, right, bottom);
        }

        public float Width() => right - left;

        public float Height() => bottom - top;

        public float Square() => Width() * Height();

        public RectF Set(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            return this;
        }

        public RectF Set(Rect rect) => Set(rect.left, rect.top, rect.right, rect.bottom);

        public RectF SetPos(float x, float y) => Set(x, y, x + (right - left), y + (bottom - top));

        public RectF Shift(float x, float y) => Set(left + x, top + y, right + x, bottom + y);

        public RectF Resize(float w, float h) => Set(left, top, left + w, top + h);

        public bool IsEmpty() => right <= left || bottom <= top;

        public RectF SetEmpty()
        {
            left = right = top = bottom = 0;
            return this;
        }

        public RectF Intersect(RectF other)
        {
            return new RectF
            {
                left = Mathf.Max(left, other.left),
                right = Mathf.Min(right, other.right),
                top = Mathf.Max(top, other.top),
                bottom = Mathf.Min(bottom, other.bottom)
            };
        }

        public RectF Union(RectF other)
        {
            return new RectF
            {
                left = Mathf.Min(left, other.left),
                right = Mathf.Max(right, other.right),
                top = Mathf.Min(top, other.top),
                bottom = Mathf.Max(bottom, other.bottom)
            };
        }

        public RectF Union(float x, float y)
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
                right = x + 1f;
            }

            if (y < top)
            {
                top = y;
            }
            else if (y >= bottom)
            {
                bottom = y + 1f;
            }

            return this;
        }

        public RectF Union(Point p) => Union(p.x, p.y);

        public bool Inside(Point p) => p.x >= left && p.x < right && p.y >= top && p.y < bottom;

        public RectF Shrink(float d) => new(left + d, top + d, right - d, bottom - d);

        public RectF Shrink() => Shrink(1f);

        public RectF Scale(float d) => new(left * d, top * d, right * d, bottom * d);
    }
}
