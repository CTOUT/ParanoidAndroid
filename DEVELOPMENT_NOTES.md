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