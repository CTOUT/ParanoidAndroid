// Only include this patch when real Verse/Harmony assemblies are present (HAVE_VERSE defined in csproj when not using stubs)
#if HAVE_VERSE
using HarmonyLib;
using Verse;

namespace ParanoidAndroid.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Tick))]
    internal static class Pawn_Tick_Patch
    {
        static void Postfix(Pawn __instance)
        {
            if (ParanoidAndroidMod.Settings == null || !ParanoidAndroidMod.Settings.DebugLogging) return; // fast path off
            if (__instance is null || !__instance.Spawned || __instance.Destroyed) return;
            if (__instance.Map == null) return; // world gen or off-map

            if (__instance.IsHashIntervalTick(5000))
            {
                Log.Message($"[ParanoidAndroid][Debug] Pawn alive: {__instance.LabelShort} on {__instance.Map.uniqueID}");
            }
        }
    }
}
#endif
