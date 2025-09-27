using System;
using System.Reflection;
// Verse logging only available in real game builds.
#if !(REFERENCE_STUBS || STUB_VERSE)
using Verse;
#endif

namespace ParanoidAndroid.Util
{
    internal static class LogUtil
    {
        public static void Debug(string component, string message)
        {
#if !(REFERENCE_STUBS || STUB_VERSE)
            if (ParanoidAndroidMod.Settings != null && !ParanoidAndroidMod.Settings.DebugLogging) return;
            Log.Message($"[ParanoidAndroid][{component}] {message}");
#endif
        }

        public static void Warn(string component, string message)
        {
#if !(REFERENCE_STUBS || STUB_VERSE)
            Log.Warning($"[ParanoidAndroid][{component}] {message}");
#endif
        }

        public static void Error(string component, string message, Exception? ex = null)
        {
#if !(REFERENCE_STUBS || STUB_VERSE)
            Log.Error($"[ParanoidAndroid][{component}] {message}{(ex != null ? (" :: " + ex) : string.Empty)}");
#endif
        }
    }

    internal static class Reflect
    {
        // Cached retrieval example for external optional methods.
        public static MethodInfo? GetMethodCached(Type type, string name, BindingFlags flags, ref MethodInfo? cache)
        {
            if (cache != null) return cache;
            cache = type.GetMethod(name, flags);
            return cache;
        }
    }
}
