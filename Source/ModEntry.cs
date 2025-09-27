// Excluded for stub/reference builds.
#if !REFERENCE_STUBS
using UnityEngine;
using Verse;

namespace ParanoidAndroid
{
    public class ParanoidAndroidMod : Mod
    {
        public static ParanoidAndroidSettings Settings { get; private set; } = null!;

        public ParanoidAndroidMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ParanoidAndroidSettings>();
        }

        public override string SettingsCategory() => "Paranoid Android";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Enable Debug Logging", ref Settings.DebugLogging, "Show verbose diagnostic logs.");
            listing.End();
            Settings.Write();
        }
    }

    public class ParanoidAndroidSettings : ModSettings
    {
        public bool DebugLogging = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref DebugLogging, nameof(DebugLogging), false);
        }
    }
}
#endif
