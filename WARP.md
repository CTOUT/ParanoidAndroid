# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

This is a RimWorld mod inspired by the classic 1980s game Paradroid, implementing robot hacking and control mechanics. The mod is currently in early development with XML definitions established but C# implementation pending.

## Development Commands

### RimWorld Development
```powershell
# Launch RimWorld with dev mode enabled (from RimWorld installation directory)
& "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64.exe" -dev

# Copy mod to RimWorld mods folder for testing (if not already in Steam location)
Copy-Item -Recurse "." "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimWorld-Paradroid-Mod"
```

### C# Development (when implemented)
```powershell
# Build C# assemblies (when Source directory contains .csproj files)
dotnet build Source/ParadroidMod.csproj

# Clean build artifacts
dotnet clean Source/ParadroidMod.csproj
```

### XML Validation
```powershell
# Validate XML definition files
Get-ChildItem -Path "Defs" -Filter "*.xml" -Recurse | ForEach-Object {
    try {
        [xml]$xml = Get-Content $_.FullName
        Write-Host "✓ Valid: $($_.Name)" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Invalid: $($_.Name) - $($_.Exception.Message)" -ForegroundColor Red
    }
}
```

### Testing
```powershell
# Run single test scenario in RimWorld (manual process)
# 1. Launch RimWorld with -dev flag
# 2. Enable mod in mod configuration
# 3. Use debug actions to spawn test items/scenarios
```

## Architecture Overview

### Core Systems Design

The mod implements a **component-based architecture** following RimWorld's patterns:

**Key Components (Planned C# Implementation):**
- `HackingComp`: Attached to pawns with neural interfaces, manages hacking capabilities
- `HackableComp`: Attached to robots/mechanoids, defines hackability parameters  
- `ControllerComp`: Manages hacker-robot relationships and control states
- `HackingJobDriver`: Custom job driver implementing the hacking action sequence
- `ControlUIOverlay`: UI system for managing controlled robots

**Integration Points:**
- **Surgery System**: Neural interface installation via RimWorld's surgery framework
- **Research System**: Progressive technology unlocks through research tree
- **Combat System**: Controlled robots participate in colony combat
- **Work System**: Controlled robots perform colony tasks
- **AI System**: Custom behaviors for controlled robot entities

### Robot Classification System

Following Paradroid's numbering convention adapted for RimWorld:

- **100-199**: Basic utility bots (cleaning, hauling) - Easy to hack, low combat value
- **200-399**: Worker bots (construction, mining) - Medium difficulty, high utility
- **400-599**: Combat bots (security, patrol) - Hard to hack, high combat value  
- **600-799**: Advanced combat mechanoids - Very hard to hack, extremely powerful
- **800-999**: Experimental/unique robots - Special mechanics, rare encounters

### Data Flow Architecture

```
Player Action → HackingJobDriver → Skill Checks → HackingComp → ControllerComp → Robot Behavior
     ↑                                                              ↓
Neural Interface ← Surgery System                              UI Updates ← ControlUIOverlay
```

### XML Definition Structure

**ThingDefs**: Define physical items (neural interfaces, data cores)
**ResearchProjectDefs**: Define research progression and prerequisites  
**JobDefs**: Define custom jobs (hacking, robot control)
**WorkGiverDefs**: Define work priorities and assignments (not yet implemented)

## Development Guidelines

### Code Organization

- **Defs/**: XML definitions following RimWorld conventions
- **Source/**: C# source code (structured as Visual Studio project when implemented)
- **Textures/**: Art assets organized by category (items, ui, effects)
- **About/**: Mod metadata and Steam Workshop information

### XML Definition Patterns

When creating new definitions:
- Use consistent defName patterns: `Paradroid[Category][Name]`
- Follow RimWorld's parent inheritance structure
- Include proper research prerequisites and tech levels
- Maintain balance with vanilla game content

### C# Implementation Approach (Future Development)

**Component Pattern**: Use RimWorld's `ThingComp` system for modular functionality
**Job System Integration**: Extend `JobDriver` classes for custom actions  
**UI Framework**: Leverage RimWorld's immediate mode GUI system
**Performance Considerations**: Cache expensive calculations, use object pooling for frequent allocations

### Balancing Philosophy

- Neural interfaces should be **mid-to-late game technology**
- Controlled robots **shouldn't trivialize combat** encounters
- **Resource costs should be significant** to prevent exploitation
- Implement **risk/reward mechanics** for different robot types

### Testing Strategy

**Manual Testing Workflow:**
1. Launch RimWorld with `-dev` flag
2. Use debug actions to spawn test scenarios
3. Verify XML definitions load without errors
4. Test research progression and item crafting
5. Validate mod compatibility with vanilla systems

**Key Test Scenarios:**
- Neural interface installation surgery
- Research tree progression
- Item crafting and resource requirements  
- Mod compatibility with other mechanoid mods

### Future Architecture Considerations

**Planned Extensions:**
- Robot customization and upgrade systems
- Network hacking (multiple simultaneous robots)
- Counter-hacking mechanics from enemy forces
- Robot rebellion and loyalty decay systems

**Integration Opportunities:**
- Compatibility hooks for other robot/mechanoid mods
- Special faction interactions and storylines
- Custom events and incident systems

## Important Files

- `DEVELOPMENT_NOTES.md`: Detailed design philosophy and technical planning
- `About/About.xml`: Mod metadata for RimWorld mod loader
- `Defs/ResearchProjectDefs/ParadroidResearch.xml`: Research progression tree
- `Defs/ThingDefs/ParadroidDevices.xml`: Item definitions and crafting recipes
- `Defs/JobDefs/ParadroidJobs.xml`: Custom job definitions

## Mod Development Context

This project follows **RimWorld modding conventions** and requires understanding of:
- RimWorld's XML definition system
- C# modding with Harmony patching (when implemented)  
- Unity engine integration for UI and visual effects
- Steam Workshop publishing process

The mod is designed to integrate seamlessly with RimWorld's existing systems while adding strategic depth through the Paradroid-inspired robot control mechanics.