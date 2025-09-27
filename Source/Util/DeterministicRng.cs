using System;
#if HAVE_VERSE
using Verse;
#endif

namespace ParanoidAndroid.Util
{
    // Wrapper for deterministic MP-friendly random flows if extended later.
    internal static class DeterministicRng
    {
        private static readonly Random _fallback = new(123456);
        public static int RangeInclusive(int min, int max)
        {
#if HAVE_VERSE
            try { return Verse.Rand.RangeInclusive(min, max); } catch { }
#endif
            return _fallback.Next(min, max + 1);
        }
        public static float Value
        {
            get
            {
#if HAVE_VERSE
                try { return Verse.Rand.Value; } catch { }
#endif
                return (float)_fallback.NextDouble();
            }
        }
    }
}
