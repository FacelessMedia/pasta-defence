# Pasta Defence 🍝

**"Use your noodle."**

A kitchen-themed tower defense game where pasta is the enemy and kitchen utensils are your weapons. Built in Unity (C#), targeting Steam release.

## Setup

1. **Install Unity Hub** and Unity Editor (2022.3 LTS or newer recommended)
2. Open Unity Hub → "Add Project from Disk" → Select this folder
3. Unity will create the project files around the existing `Assets/` folder
4. Open `Assets/Scenes/MainMenu` or create a new scene to get started

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

## First Steps After Unity Project Creation

1. Create a new Scene (e.g., `Stage_CuttingBoard_01`)
2. Add empty GameObjects for: `GameManager`, `WaveManager`, `EconomyManager`, `TowerManager`, `ObjectPool`, `PunManager`, `AudioManager`
3. Attach the corresponding scripts to each
4. Create ScriptableObject assets for towers, enemies, and waves via `Create > Pasta Defence > ...`
5. Create a QuipDatabase asset via `Create > Pasta Defence > Quip Database`
6. Set up a WaypointPath with child Transform waypoints
7. Create tower/enemy prefabs with sprites and attach the tower/enemy scripts
8. Build out the UI Canvas referencing the HUD script

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
- [ ] Unity scene setup + prefab wiring (requires Unity Editor)
- [ ] Placeholder art / sprites
- [ ] Playtesting and balance pass
