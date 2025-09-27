using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if HAVE_VERSE
using Verse;
#endif

namespace ParanoidAndroid.Util
{
    /// <summary>
    /// Helpers for soft/optional dependencies (workshop mods) without taking hard compile-time references.
    /// Provides: quick presence check by packageId, optional assembly load, and cached reflection lookups.
    /// </summary>
    internal static class SoftDependency
    {
        private static readonly Dictionary<string, bool> _presenceCache = new();
        private static readonly Dictionary<string, Assembly?> _assemblyCache = new();

        /// <summary>
        /// True if a running mod with the given (lowercase) packageId is active.
        /// </summary>
        public static bool Present(string packageIdLower)
        {
            if (_presenceCache.TryGetValue(packageIdLower, out var val)) return val;
#if HAVE_VERSE
            val = LoadedModManager.RunningModsListForReading.Any(m => m?.PackageIdLowerCase == packageIdLower);
#else
            val = false; // cannot determine in stub build
#endif
            _presenceCache[packageIdLower] = val;
            return val;
        }

        /// <summary>
        /// Attempt to find and load an external assembly (.dll) residing inside an active mod's folder.
        /// Only loads once per key; returns null if not found or load fails. Does NOT throw.
        /// </summary>
        /// <param name="packageIdLower">Target mod package id (lowercase).</param>
        /// <param name="relativeDllPath">Relative path inside the mod root (e.g. "1.6/Assemblies/SomeModCore.dll").</param>
        public static Assembly? TryLoadModAssembly(string packageIdLower, string relativeDllPath)
        {
            var cacheKey = packageIdLower + "::" + relativeDllPath;
            if (_assemblyCache.TryGetValue(cacheKey, out var asm)) return asm;
            if (!Present(packageIdLower)) { _assemblyCache[cacheKey] = null; return null; }
            try
            {
#if HAVE_VERSE
                var mod = LoadedModManager.RunningModsListForReading.FirstOrDefault(m => m?.PackageIdLowerCase == packageIdLower);
                if (mod == null) { _assemblyCache[cacheKey] = null; return null; }
                var fullProp = mod.GetType().GetProperty("RootDir", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                var rootDir = fullProp?.GetValue(mod) as string;
                var full = rootDir == null ? null : Path.Combine(rootDir, relativeDllPath);
                if (full == null) { _assemblyCache[cacheKey] = null; return null; }
#else
                string? full = null;
#endif
                if (!File.Exists(full)) { _assemblyCache[cacheKey] = null; return null; }
                asm = Assembly.Load(File.ReadAllBytes(full));
                _assemblyCache[cacheKey] = asm;
                LogUtil.Debug("SoftDependency", $"Loaded external assembly '{relativeDllPath}' from '{packageIdLower}'.");
                return asm;
            }
            catch (Exception ex)
            {
                LogUtil.Warn("SoftDependency", $"Failed to load assembly for {packageIdLower}::{relativeDllPath} -> {ex.Message}");
                _assemblyCache[cacheKey] = null;
                return null;
            }
        }

        /// <summary>
        /// Cached type resolution inside a previously loaded optional assembly.
        /// </summary>
        public static Type? GetTypeCached(Assembly? asm, string fullName, Dictionary<string, Type?> cache)
        {
            if (asm == null) return null;
            if (cache.TryGetValue(fullName, out var t)) return t;
            t = asm.GetType(fullName, throwOnError: false, ignoreCase: false);
            cache[fullName] = t;
            return t;
        }
    }
}
