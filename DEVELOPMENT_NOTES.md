# Paradroid Mod - Development Notes

## Design Philosophy

This mod aims to capture the strategic essence of the original Paradroid game while fitting naturally into RimWorld's gameplay systems.

## Core Mechanics Design

### Hacking System
- **Neural Interface Required**: Players must have the neural interface implant
- **Skill-Based Success**: Intelligence and tech skills affect success rate
- **Risk vs. Reward**: Higher-tier robots harder to hack but more valuable
- **Temporary vs. Permanent**: Initially temporary control, upgrades enable permanent

### Robot Classification System
Following Paradroid's numbering system, adapted for RimWorld:

**100-199: Basic Utility Bots**
- Cleaning bots, simple haulers
- Easy to hack, limited combat value
- Good for learning the system

**200-399: Worker Bots** 
- Construction, mining bots
- Medium difficulty, high utility value
- Essential for base building

**400-599: Combat Bots**
- Security drones, patrol bots  
- Hard to hack, high combat value
- Risk/reward for combat situations

**600-799: Advanced Combat**
- Military-grade mechanoids
- Very hard to hack, extremely powerful
- End-game targets

**800-999: Experimental/Unique**
- Special boss-type robots
- Unique mechanics and abilities
- Rare encounters with major rewards

### Hacking Mini-Game Concept
Instead of a complex mini-game, use RimWorld's existing systems:
- **Time-based**: Hacking takes time, leaving player vulnerable
- **Skill checks**: Multiple skill checks during the process
- **Interruption risks**: Can be interrupted by damage or other factors

## Technical Implementation

### C# Components Needed
1. **HackingComp**: Component for pawns with neural interfaces
2. **HackableComp**: Component for robots/mechanoids that can be hacked
3. **ControllerComp**: Manages the relationship between hacker and controlled robot
4. **HackingJobDriver**: Custom job driver for the hacking action
5. **ControlUIOverlay**: UI for managing controlled robots

### Integration Points
- **Surgery system**: Neural interface installation
- **Research system**: Progressive unlocks
- **Combat system**: Controlled robots in combat
- **Work system**: Controlled robots doing colony work
- **AI system**: Controlled robot behaviors

## Art Asset Requirements

### Items
- Neural interface device icon
- Paradroid data core icon
- Various robot/droid textures

### UI Elements
- Hacking progress indicators
- Robot control interface elements
- Research tab icons

### Effects
- Hacking visual effects
- Neural interface glow effects
- Control link indicators

## Balance Considerations

### Power Level
- Neural interface should be mid-to-late game tech
- Controlled robots shouldn't trivialize combat
- Resource costs should be significant

### Limitations
- Neural interface users might have debuffs or risks
- Controlled robots could have loyalty decay
- Hacking should have cooldowns or energy costs

## Future Expansion Ideas

### Additional Features
- Robot customization/upgrades
- Network hacking (multiple robots)
- Counter-hacking by enemy forces
- Robot rebellion mechanics

### Integration Opportunities
- Compatibility with other robot/mechanoid mods
- Integration with existing faction systems
- Special events and storylines

## Testing Checklist

- [ ] Neural interface installation works correctly
- [ ] Research progression functions properly
- [ ] Hacking mechanics are balanced and fun
- [ ] Controlled robots behave appropriately
- [ ] No game-breaking exploits
- [ ] Multiplayer compatibility
- [ ] Performance impact assessment
- [ ] Mod compatibility testing

## Implementation Notes (Race/Pawn Consolidation)

To avoid duplicate or conflicting definitions, Paradroid droids now follow a single-source pattern:

- Race (ThingDef with `<race>` block) is defined once in `ParadroidRaces.xml` (e.g., `ParadroidDroidNeural`, `ParadroidDroidUtility`, etc.).
- PawnKindDefs reference those race defNames directly for spawning, faction integration, and storyteller usage.
- Redundant standalone ThingDefs that attempted to redefine the same unit (e.g., the former `Paradroid_InfluenceDevice_001.xml`) have been removed to prevent duplicate pawn categories and cross-ref noise.

When adding a new droid:
1. If it fits an existing series (utility/worker/service/security/combat/etc.), extend behavior via PawnKind only (adjust `combatPower`, apparel, weapon tags, lifeStage graphics).
2. Only create a new race ThingDef if the chassis genuinely differs in body, body size, movement mode, or fundamental race properties.
3. Keep graphics either on the race definition (shared look) or override per PawnKind with `lifeStages -> bodyGraphicData` if needed.
4. Avoid creating parallel template ThingDefs unless they are strictly abstract (`Abstract="true"`) helpers.

Benefits:
- Cleaner load order, fewer cross-reference errors.
- Easier future C# integration for hacking (single place for comps if needed).
- Reduced maintenance when balancing stats across a series.

Future: Once hacking mechanics are implemented, consider adding a custom comp only to base race(s) instead of duplicating across many concrete ThingDefs.

### 2025-09-27 Cleanup Pass

Actions performed:
- Removed all placeholder per-series ThingDef files (`Paradroid_UtilityDroid_100.xml`, `Paradroid_WorkerDroid_200.xml`, etc.) that merely wrapped existing race definitions. These added no value and produced parent resolution errors when race def load order shifted.
- Removed obsolete `Paradroid_InfluenceDevice_001.xml` (superseded by `ParadroidDroidNeural` race + PawnKind `ParadroidDroid101_Influence`).
- Fixed invalid faction reference `HostileAutomatons` -> `CorruptedSystems` in PawnKind `ParadroidDroid476_Maintenance`.
- Pruned `nameGenerator` references from race defs where no corresponding `RulePackDef` yet exists (kept only Neural + Utility which have rule packs). This eliminates cross-ref errors while preserving future expansion hooks.
- Left a comment marker at each removed generator to make it easy to reinstate once rule packs are authored.

Next naming step options:
1. Add minimal RulePackDefs for remaining chassis (Worker, Service, Security, Combat, HeavyCombat, Specialist, Elite, Experimental) and restore the `nameGenerator` tags.
2. Defer naming variety and keep relying on default mechanoid naming until gameplay systems mature.

Recommended immediate next step: Validate a clean game load (no red errors). If successful, proceed to drafting the C# hacking comp scaffolding.