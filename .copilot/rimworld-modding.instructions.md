---
description: "RimWorld modding instruction set: Harmony patches, Def XML, performance, safety, save compatibility."
applyTo: "**/*.cs, **/*.xml, **/*.patch, **/*.txt, **/About.xml"
---

# RimWorld Modding Instruction Set

When assisting on this repository:

Priorities (in order):

1. Maintain save compatibility.
2. Minimize patch invasiveness.
3. Avoid performance regressions in hot paths.
4. Ensure clear diagnostics and optional debug logging.
5. Prevent conflicts with other mods where feasible.

## C# Guidelines

- Namespace prefix: ParanoidAndroid.\*
- Harmony instance id: "ParanoidAndroid.Core"
- Patch class naming: `<TargetType>_<Method>_Patch`
- Separate patch registration into a `HarmonyBootstrap` static initializer
- Use `internal static class` for patch containers unless externally consumed
- Cache expensive lookups (Defs, MethodInfo) in static readonly fields after def load

## XML Def Guidelines

- Prefix all new defNames with `ParAndroid_`
- Use PatchOperationSequence for multi-step modifications
- Document purpose in XML comments at top of file
- Avoid overwriting vanilla values unless necessary—prefer additive patches

## Serialization

- Use `Scribe_Values.Look(ref field, "fieldName", defaultValue);`
- For complex types: `Scribe_Deep.Look(ref obj, "objField");`
- For lists: specify `LookMode` explicitly if not primitive
- If removing a field: keep stub with `[Unsaved(false)]` or migration shim to avoid load errors

## Performance

- Avoid allocations in `Tick()`, `PostDraw()`, pathfinding loops
- No LINQ / closures in high-frequency code
- Reuse static buffers: `static readonly List<Thing> tmpThings = new();`
- Prefer `for` loops over `foreach` when iterating large collections repeatedly

## Logging & Debugging

- Wrap optional logs: `if (ParanoidSettings.Debug) Log.Message(...);`
- Use consistent tag: `[ParanoidAndroid]`
- Catch exceptions at external integration boundaries and downgrade to warning unless critical

## Safety

- Check `Spawned`, `Map != null`, `Destroyed` before accessing map state
- Avoid operating on `null` pawns or maps during world gen
- Validate reflection targets once; provide fallback path if absent

## Patch Selection Heuristics

- XML first (Defs) → simple Harmony postfix → prefix (with `return false`) → transpiler (last resort)

## Multiplayer

- Avoid use of non-deterministic APIs (System.Random local instances) inside synced contexts; prefer Verse.Rand or deterministically seeded RNG when needed

## Delivery Format for Patch Requests

When the user requests a patch, return: Target, Rationale, Risks, Edge Cases, Code, Test Idea.

Apply these rules automatically unless the user explicitly overrides with reason.
