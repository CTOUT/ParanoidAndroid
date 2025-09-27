using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ParanoidAndroid.Util
{
    internal static class ModDependency
    {
        private static readonly Dictionary<string, bool> _cache = new();
        private static bool _initialized;
        private static IEnumerable<object>? _runningMods; // Holds Verse.ModMetaData objects when available
        private static PropertyInfo? _packageIdLowerProp;

        // Official DLC packageIds (RimWorld 1.6) - update if Ludeon adds more.
        public const string RoyaltyId = "ludeon.rimworld.royalty";
        public const string IdeologyId = "ludeon.rimworld.ideology";
        public const string BiotechId = "ludeon.rimworld.biotech";
        public const string AnomalyId = "ludeon.rimworld.anomaly";
        public const string OdysseyId = "ludeon.rimworld.odyssey"; // RimWorld 1.6 Odyssey DLC (verify ID if Ludeon changes naming)

        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            try
            {
                // Look for LoadedModManager type via reflection (Verse assembly) without creating a hard compile-time dependency.
                var loadedModMgr = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a =>
                    {
                        try { return a.GetType("Verse.LoadedModManager", throwOnError: false); } catch { return null; }
                    })
                    .FirstOrDefault(t => t != null);
                if (loadedModMgr == null) return;
                var prop = loadedModMgr.GetProperty("RunningModsListForReading", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var value = prop?.GetValue(null) as IEnumerable<object>;
                if (value == null) return;
                _runningMods = value;
                // Capture property for package id
                _packageIdLowerProp = _runningMods.FirstOrDefault()?.GetType().GetProperty("PackageIdLowerCase", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch
            {
                // Swallow; absence simply means all Has() return false.
            }
        }

        public static bool Has(string packageId)
        {
            if (_cache.TryGetValue(packageId, out var cached)) return cached;
            Init();
            if (_runningMods == null || _packageIdLowerProp == null)
            {
                _cache[packageId] = false; return false;
            }
            foreach (var mod in _runningMods)
            {
                try
                {
                    var pid = _packageIdLowerProp.GetValue(mod) as string;
                    if (pid == packageId) { _cache[packageId] = true; return true; }
                }
                catch { }
            }
            _cache[packageId] = false;
            return false;
        }

        public static bool Royalty => Has(RoyaltyId);
        public static bool Ideology => Has(IdeologyId);
        public static bool Biotech => Has(BiotechId);
        public static bool Anomaly => Has(AnomalyId);
        public static bool Odyssey => Has(OdysseyId);

        public static string ActiveDlcSummary()
        {
            var list = new List<string>();
            if (Royalty) list.Add("Royalty");
            if (Ideology) list.Add("Ideology");
            if (Biotech) list.Add("Biotech");
            if (Anomaly) list.Add("Anomaly");
            if (Odyssey) list.Add("Odyssey");
            return list.Count == 0 ? "(No DLC)" : string.Join(", ", list);
        }

        // Test-only helper to clear caches and re-scan.
        internal static void InternalResetForTests()
        {
            _cache.Clear();
            _initialized = false;
            _runningMods = null;
            _packageIdLowerProp = null;
        }
    }
}
