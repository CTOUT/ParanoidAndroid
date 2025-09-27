# ParanoidAndroid RimWorld Mod

## Overview

Foundation scaffolding for the ParanoidAndroid mod. Key components:

- Harmony bootstrap & patch audit (conflict heuristic)
- Mod settings UI (debug logging toggle)
- Build + packaging tasks & CI workflow
- Dynamic game path + workshop root resolution
- DLC detection utility (`ModDependency`)
- Soft dependency loader (`SoftDependency`) for optional workshop mods
- Deterministic RNG wrapper & build metadata stamping
- Test, benchmark, and playground projects
- Save migration example pattern

## Folder Structure

```text
ParanoidAndroid/
  About/              (Create About.xml here)
  Assemblies/         (Built DLL copied here for RimWorld to load)
  Source/             (C# source files)
  .copilot/           (AI assistance configuration)
  .vscode/            (Editor tasks, settings)
  ParanoidAndroid.csproj
  ParanoidAndroid.Playground.csproj
  ParanoidAndroid.sln
```

## Building

From VS Code:

1. Run task: Build Mod (Debug)
1. Run task: Publish DLL to Assemblies
1. Launch RimWorld and enable the mod

## Adding Harmony Patches

1. Create a file in `Source/Patches/` (create folder) named `<Type>_<Method>_Patch.cs`
1. Add:

```csharp
[HarmonyPatch(typeof(TargetType), nameof(TargetType.Method))]
internal static class TargetType_Method_Patch {
    static void Postfix(TargetType __instance) {
        if (!ParanoidAndroidMod.Settings.DebugLogging) return;
        // ... logic
    }
}
```

1. (Optional) Call `harmony.PatchAll()` already present in bootstrap.

## Settings & Debug Logging

Use `ParanoidAndroidMod.Settings.DebugLogging` to guard verbose logs.

## RimWorld Assemblies Reference

Adjust `<HintPath>` entries in `ParanoidAndroid.csproj` if your directory layout differs. Typically you want references pointing to:

```text
RimWorldWin64_Data/Managed/Assembly-CSharp.dll
RimWorldWin64_Data/Managed/Verse.dll
RimWorldWin64_Data/Managed/UnityEngine.CoreModule.dll
```

## Playground Project

`ParanoidAndroid.Playground` targets .NET 8 for experimenting with logic in isolation (no RimWorld runtime). Put pure algorithm / data structure tests here.

## Advanced Utilities

### Patch Audit & Conflict Heuristic

Enable Debug Logging in settings to output a list of patched methods with owners and a section of potential conflicts (multiple owners plus transpiler or multiple prefixes). Use this as a starting point for deeper manual review.

### DLC & Soft Dependencies

Check DLC: `if (ModDependency.Biotech) { ... }`
Optional workshop mod detection: `if (SoftDependency.Present("some.modid")) { /* integrate */ }`
Optional assembly loading:

```csharp
var extAsm = SoftDependency.TryLoadModAssembly("some.modid", "1.6/Assemblies/SomeMod.dll");
if (extAsm != null) { /* reflect types */ }
```

### Build Metadata & About Stamping

Set `StampAboutXml=true` (e.g. in a Directory.Build.props override or passing `/p:StampAboutXml=true`) and include tokens `__BUILD_TIME__` and `__GIT__` inside `About/About.xml` to have them replaced during build (only when opted in).

### Save Migration Pattern

See `SaveMigrationExample.cs` for handling legacy fields and collapsing them into new schema on load.

### Performance Benchmarks

Use the Benchmarks project (`ParanoidAndroid.Benchmarks`) and run the VS Code task "Run Benchmarks" to assess micro-changes before integrating into game patches.

### Deterministic RNG

Use `DeterministicRng.Next()` wrappers or gate standard RNG usages to improve reproducibility when debugging.

### Feature Documentation Generation

Annotate classes or methods with:

```csharp
[ParanoidAndroid.Util.FeatureTag(
  name: "Raid Scaling Core",
  requires: "Royalty|Odyssey", // pipe = any of
  entryPoint: "RaidScaler.Apply",
  fallback: "Skip",
  notes: "Balances cluster threat")]
```

Run the task: Generate Feature Table to refresh the auto-managed block in `DLC_DEPENDENCIES.md`. Only the section between markers is rewritten.

### CI Validation

The CI workflow enforces:

1. Feature table freshness (fails if `DLC_DEPENDENCIES.md` not regenerated after adding/removing `[FeatureTag]`).
2. Heuristic DLC guard check: if a feature tag mentions DLC keywords but the same file lacks any `ModDependency.<DLC>` usage, the build fails with a warning pointing to the file.

To fix a failure

- Run the VS Code task: Generate Feature Table.
- Add guard condition(s) around DLC-specific logic, e.g. `if (!ModDependency.Biotech) return;`.

## Roadmap / Next Ideas

- Populate `DLC_DEPENDENCIES.md` as new gated features land
- Extend conflict detection with patch order diffing
- Add more granular allocation profiling hooks
- Implement feature toggles stored in settings for experimental logic

## License

Add a LICENSE file if distributing.
