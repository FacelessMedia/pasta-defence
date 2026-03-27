# Pasta Defence 🍝

**"Use your noodle."**

A kitchen-themed tower defense game where pasta is the enemy and kitchen utensils are your weapons. Built in Unity (C#), targeting Steam release.

## Setup (3 Steps!)

1. **Install Unity Hub** and Unity Editor (2022.3 LTS or newer recommended)
2. Open Unity Hub → **"Add Project from Disk"** → Select this folder → Open the project
3. In Unity, go to the top menu: **Pasta Defence → ONE-CLICK SETUP (Do Everything)**

That's it! The script automatically creates all ScriptableObjects, prefabs, placeholder sprites, scenes, UI, and wires everything together. Hit **Play** to start.

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/       — GameManager, WaveManager, EconomyManager, EventBus, WaypointPath
│   ├── Towers/     — BaseTower, 3 tower subclasses, TowerManager, TowerPlacement, TowerData
│   ├── Enemies/    — BaseEnemy, 3 enemy subclasses, EnemyData, WaveData
│   ├── Hero/       — ChefHero, ChefAbility
│   ├── UI/         — GameHUD, GameOverScreen, PauseMenu, MainMenuUI, EnemyHealthBar
│   ├── Humor/      — QuipDatabase, PunManager, FlavorTextProvider
│   ├── Meta/       — SkillTree, RecipeBook, SaveSystem
│   ├── Audio/      — AudioManager
│   └── Utils/      — ObjectPool, Constants
├── ScriptableObjects/  — Create tower/enemy/wave data assets here
├── Prefabs/            — Tower and enemy prefabs go here
├── Art/                — Sprites, animations, UI art
├── Audio/              — Music and SFX files
└── Scenes/             — Game scenes
```

## Menu Options (All Automated)

After opening the project in Unity, you'll see a **"Pasta Defence"** menu at the top with these options:

| Menu Item | What It Does |
|-----------|-------------|
| **ONE-CLICK SETUP (Do Everything)** | Runs ALL setup steps below in order — this is all you need |
| Create All Data Assets | Creates 10 tower + 15 enemy + 10 wave + 1 map + 1 quip database ScriptableObjects |
| Create All Prefabs | Creates 26 prefabs with placeholder sprites, colliders, and scripts attached |
| Setup Game Scene | Builds the full game scene: camera, path, placements, managers, UI canvas, all wired up |
| Setup Main Menu Scene | Builds the main menu scene with title, buttons, and random taglines |
| Add Scenes to Build Settings | Configures build settings so scenes load correctly |

## Architecture Notes

- **ScriptableObjects** drive all game data — no hardcoded values
- **EventBus** decouples systems — subscribe/trigger via `GameEvent` enum
- **ObjectPool** handles enemy/projectile recycling for performance
- **PunManager** delivers humor — 150+ quips across 13 categories
- **Meta-progression** persists via JSON save files

## Currency

- **Dough** — In-game currency earned from defeating pasta
- **Parmesan** — Premium currency (future IAP)

## Phase 1 Status (Core Prototype)

- [x] Core managers (GameManager, WaveManager, EconomyManager, EventBus)
- [x] Path system (WaypointPath)
- [x] Object pooling
- [x] Base tower system + 3 towers (Rolling Pin, Chef's Knife, Pepper Grinder)
- [x] Tower placement & targeting system
- [x] Base enemy system + 3 enemies (Penne, Spaghetti, Rigatoni)
- [x] ScriptableObject data (TowerData, EnemyData, WaveData)
- [x] Chef Hero with abilities and leveling
- [x] Humor system (QuipDatabase with 150+ quips, PunManager, FlavorTextProvider)
- [x] Full UI system (HUD, tower info, pause, game over, main menu)
- [x] Meta-progression scaffolding (SkillTree, RecipeBook, SaveSystem)
- [x] Audio manager scaffold
- [x] Automated project setup (ONE-CLICK SETUP creates everything)
- [x] Auto-generated placeholder sprites for all towers, enemies, and hero
- [x] Auto-built scenes (MainMenu + Stage_CuttingBoard_01) with full UI
- [ ] Replace placeholder art with hand-drawn sprites
- [ ] Playtesting and balance pass
