# DLC / External Feature Tracking (RimWorld 1.6)

This document tracks any features that require DLC or external workshop mods.

## Official DLC Flags

- Royalty: `ParanoidAndroid.Util.ModDependency.Royalty`
- Ideology: `ParanoidAndroid.Util.ModDependency.Ideology`
- Biotech: `ParanoidAndroid.Util.ModDependency.Biotech`
- Anomaly: `ParanoidAndroid.Util.ModDependency.Anomaly`
- Odyssey: `ParanoidAndroid.Util.ModDependency.Odyssey`

## Guidelines

1. Always guard DLC-specific logic: `if (ModDependency.Biotech) { ... }`
2. Provide fallback or skip silently when DLC absent.
3. Avoid hard references to DLC-only defs unless wrapped.
4. Add entries below when introducing new DLC-dependent features.

## Features Table

| Feature | Requires | Code Entry Point | Fallback Behavior | Notes |
| ------- | -------- | ---------------- | ----------------- | ----- |

<!-- AUTO-FEATURE-ROWS:BEGIN -->

| (example) Psychic Focus Tuning | Royalty | TBD | Skip patch | Pending design |

<!-- AUTO-FEATURE-ROWS:END -->

## Workshop Soft Dependencies

Add rows here for optional workshop mod integrations (by packageId + workshop ID):

| Integration | Workshop ID | PackageId    | Detection Mechanism | Purpose               | Notes               |
| ----------- | ----------- | ------------ | ------------------- | --------------------- | ------------------- |
| ExampleModX | 1234567890  | example.modx | RunningModsList     | Enhances stat synergy | Not implemented yet |

Update this file whenever adding new DLC or mod conditionals.
