using System;
#if !(REFERENCE_STUBS || STUB_VERSE)
using Verse;
#endif

namespace ParanoidAndroid.Util
{
    // Example of how to migrate old saved data. Replace with real component or data classes.
    internal class SaveMigrationExample
    {
        // Old field kept for migration; will be collapsed into NewValue during load if present.
        private int? _legacyValue; // existed in versions <= 0.1.0
        private int _newValue;

        public void ExposeData()
        {
#if !(REFERENCE_STUBS || STUB_VERSE)
            Scribe_Values.Look(ref _legacyValue, "legacyValue", null);
            Scribe_Values.Look(ref _newValue, "newValue", 0);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && _legacyValue.HasValue)
            { _newValue = _newValue == 0 ? _legacyValue.Value : _newValue; _legacyValue = null; }
#endif
        }
    }
}
