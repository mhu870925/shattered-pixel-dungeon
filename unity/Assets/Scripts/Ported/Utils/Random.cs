using System;
using System.Collections.Generic;
using UnityEngine;
using RandomGenerator = System.Random;

namespace ShatteredPixelDungeon.Utils
{
    /// <summary>
    /// Stackable RNG 헬퍼. 원본 com.watabou.utils.Random의 API를 C#으로 1:1 포팅했습니다.
    /// </summary>
    public static class Random
    {
        private static readonly Stack<RandomGenerator> Generators = new();
        private static readonly object SyncRoot = new();

        static Random()
        {
            ResetGenerators();
        }

        public static void ResetGenerators()
        {
            lock (SyncRoot)
            {
                Generators.Clear();
                Generators.Push(new RandomGenerator());
            }
        }

        public static void PushGenerator()
        {
            lock (SyncRoot)
            {
                Generators.Push(new RandomGenerator());
            }
        }

        public static void PushGenerator(long seed)
        {
            lock (SyncRoot)
            {
                Generators.Push(new RandomGenerator(unchecked((int)ScrambleSeed(seed))));
            }
        }

        private static long ScrambleSeed(long seed)
        {
            unchecked
            {
                seed ^= seed >> 32;
                seed *= 0xbea225f9eb34556dL;
                seed ^= seed >> 29;
                seed *= 0xbea225f9eb34556dL;
                seed ^= seed >> 32;
                seed *= 0xbea225f9eb34556dL;
                seed ^= seed >> 29;
                return seed;
            }
        }

        public static void PopGenerator()
        {
            lock (SyncRoot)
            {
                if (Generators.Count == 1)
                {
                    Debug.LogError("Tried to pop the last random number generator!");
                    return;
                }

                Generators.Pop();
            }
        }

        private static RandomGenerator GetTop(bool useGeneratorStack)
        {
            lock (SyncRoot)
            {
                return useGeneratorStack ? Generators.Peek() : GetBottom();
            }
        }

        private static RandomGenerator GetBottom()
        {
            // Stack에서는 마지막 요소가 베이스. 배열로 복사해 맨 끝 사용.
            var array = Generators.ToArray();
            return array[array.Length - 1];
        }

        public static float Float() => Float(true);

        public static float Float(bool useGeneratorStack)
        {
            var rng = GetTop(useGeneratorStack);
            return (float)rng.NextDouble();
        }

        public static float Float(float max) => Float() * max;

        public static float Float(float min, float max) => min + Float(max - min);

        public static float NormalFloat(float min, float max)
        {
            return min + ((Float(max - min) + Float(max - min)) / 2f);
        }

        public static int Int() => Int(true);

        public static int Int(bool useGeneratorStack)
        {
            var rng = GetTop(useGeneratorStack);
            return rng.Next();
        }

        public static int Int(int max) => Int(max, true);

        public static int Int(int max, bool useGeneratorStack)
        {
            if (max <= 0) return 0;
            var rng = GetTop(useGeneratorStack);
            return rng.Next(max);
        }

        public static int Int(int min, int max)
        {
            return min + Int(max - min);
        }

        public static int IntRange(int min, int max)
        {
            return min + Int(max - min + 1);
        }

        public static int NormalIntRange(int min, int max)
        {
            return min + (int)((Float() + Float()) * (max - min + 1) / 2f);
        }

        public static int InvNormalIntRange(int min, int max)
        {
            var roll1 = Float();
            var roll2 = Float();
            if (Mathf.Abs(roll1 - 0.5f) >= Mathf.Abs(roll2 - 0.5f))
            {
                return min + (int)(roll1 * (max - min + 1));
            }

            return min + (int)(roll2 * (max - min + 1));
        }

        public static long Long() => Long(true);

        public static long Long(bool useGeneratorStack)
        {
            var rng = GetTop(useGeneratorStack);
            return NextLong(rng);
        }

        public static long Long(long max)
        {
            var result = Long();
            if (result < 0) result += long.MaxValue;
            return max == 0 ? 0 : result % max;
        }

        public static int Chances(float[] chances)
        {
            var length = chances.Length;
            float sum = 0;
            for (var i = 0; i < length; i++)
            {
                sum += Mathf.Max(0, chances[i]);
            }

            if (sum <= 0)
            {
                return -1;
            }

            var value = Float(sum);
            sum = 0;
            for (var i = 0; i < length; i++)
            {
                sum += Mathf.Max(0, chances[i]);
                if (value < sum)
                {
                    return i;
                }
            }

            return -1;
        }

        public static K Chances<K>(Dictionary<K, float> chances)
        {
            var values = new List<K>(chances.Keys);
            var probs = new List<float>(values.Count);
            float sum = 0;
            for (var i = 0; i < values.Count; i++)
            {
                var p = chances[values[i]];
                probs.Add(p);
                sum += p;
            }

            if (sum <= 0)
            {
                return default;
            }

            var value = Float(sum);
            sum = probs[0];
            for (var i = 0; i < values.Count; i++)
            {
                if (value < sum)
                {
                    return values[i];
                }

                if (i + 1 < probs.Count)
                {
                    sum += probs[i + 1];
                }
            }

            return default;
        }

        public static int Index<T>(ICollection<T> collection) => Int(collection.Count);

        public static T OneOf<T>(params T[] array) => array[Int(array.Length)];

        public static T Element<T>(T[] array) => Element(array, array.Length);

        public static T Element<T>(T[] array, int max) => array[Int(max)];

        public static T Element<T>(ICollection<T> collection)
        {
            var size = collection.Count;
            if (size == 0) return default;
            var index = Int(size);
            var i = 0;
            foreach (var item in collection)
            {
                if (i == index) return item;
                i++;
            }

            return default;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            var rng = GetTop(true);
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void Shuffle<T>(T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int j = Int(i, array.Length);
                if (j != i)
                {
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
        }

        public static void Shuffle<U, V>(U[] u, V[] v)
        {
            for (int i = 0; i < u.Length - 1; i++)
            {
                int j = Int(i, u.Length);
                if (j != i)
                {
                    (u[i], u[j]) = (u[j], u[i]);
                    (v[i], v[j]) = (v[j], v[i]);
                }
            }
        }

        private static long NextLong(RandomGenerator rng)
        {
            var buffer = new byte[8];
            rng.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
