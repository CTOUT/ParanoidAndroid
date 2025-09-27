// Exclude bootstrap unless we have real Verse available
#if HAVE_VERSE
using System.Reflection;
using Verse;
using HarmonyLib;

namespace ParanoidAndroid
{
    internal static class HarmonyBootstrap
    {
        private const string HarmonyId = "ParanoidAndroid.Core";
        static HarmonyBootstrap()
        {
#if HAVE_VERSE
            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll(typeof(HarmonyBootstrap).Assembly);
                int count = 0; foreach (var _ in harmony.GetPatchedMethods()) count++;
                Log.Message($"[ParanoidAndroid] Harmony initialized. Patched methods: {count}");
                if (ParanoidAndroidMod.Settings?.DebugLogging == true)
                {
                    Util.PatchAudit.LogPatchedMethods(harmony);
                }
            }
            catch { }
#endif
        }
    }
}
#endif
