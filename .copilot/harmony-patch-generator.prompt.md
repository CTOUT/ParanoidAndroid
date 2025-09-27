---
description: "Generate a safe, minimal Harmony patch for a RimWorld method with conflict analysis and performance considerations."
mode: agent
---

# Harmony Patch Generator Prompt

Given a target method in RimWorld/Verse, generate a Harmony patch with:

Required Sections:

1. Target Identification
   - Type full name
   - Method signature
   - Game version assumptions (if any)
2. Patch Strategy
   - Chosen patch type (Prefix/Postfix/Transpiler/Finalizer)
   - Justification & why simpler forms not used
3. Conflict Surface
   - Common mods likely patching same method
   - Risk rating: Low / Medium / High
4. Edge Cases
   - Null/despawned objects
   - World generation
   - Multiplayer determinism
5. Performance Notes
   - Hot path? (Yes/No)
   - Avoided allocations (list them)
6. Code Output
   - Fully qualified patch class with namespace
   - Static config flag usage if relevant
7. Fallback / Graceful Degradation
   - Behavior if external mod symbols absent

Constraints:

- No LINQ in Tick / Draw / per-frame contexts
- No string interpolation inside loops without guard
- Use cached static references for Defs / MethodInfos
- Guard against repeated reflection lookup

Return Format:

```
## Target
...
## Strategy
...
## Conflict
...
## Edge Cases
...
## Performance
...
## Code
```
