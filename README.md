# Endless Runner

A lane-based endless runner built in Unity, in the spirit of *Subway Surfers* and *Temple Run*. The player sprints down an infinitely generated street, dodging traffic and obstacles by switching lanes, jumping, and rolling, while collecting coins and power-ups. Coins feed a persistent upgrade shop that levels up each power-up between runs.

> **Status:** Actively being reworked for my portfolio — the codebase is being cleaned up and reorganised, and several 3D models are being rebuilt in Blender to improve the visuals.

---

## Gameplay

- **Three-lane running** — move left/right, jump over low obstacles, and roll under high ones.
- **Endless procedural world** — the road, buildings, streetlights and props stream in continuously and recycle behind the player.
- **Adaptive difficulty** — tile difficulty ramps up over time, with built-in "breather" stretches so the run never feels unfair.
- **Power-ups** — Fly, Invincibility, High Jump, and Double Points, each with a timed duration.
- **Coin economy & shop** — spend collected coins to upgrade power-up duration and strength across 10 levels.
- **Health system** — a two-stage health model with a grace window: one hit slows you and forces a lane change; a second hit inside the window ends the run.
- **Persistent progress** — high score, total coins, and shop upgrades are saved between sessions.

## Controls

| Action       | Keyboard            | Touch              |
|--------------|---------------------|--------------------|
| Move left    | `A` / `←`           | Swipe left         |
| Move right   | `D` / `→`           | Swipe right        |
| Jump         | `Space`             | Swipe up           |
| Roll         | `Left Ctrl`         | Swipe down         |
| Pause        | `Esc`               | —                  |

---

## Architecture

The game is built around a set of singleton managers that communicate through C# events, keeping gameplay systems decoupled from the UI and audio layers.

- **`GameManager`** — owns game state (Playing / Paused / Game Over), scoring, distance, the coin balance, and high-score persistence. Broadcasts events that the UI and audio systems subscribe to.
- **`TileManager`** — streams the world: spawns, positions and recycles road tiles, side buildings, streetlights and props. Handles the speed curve and a weighted difficulty system with anti-frustration pacing.
- **`TilePopulator`** — populates each tile with obstacles, vehicles, coins and power-ups, respecting blocked lanes so runs stay playable.
- **`Movement` / `PlayerHealth`** — player input, lane switching, jump/roll/fly states, and the staged damage model.
- **`PowerUpManager` + effect classes** — a small polymorphic system (`PowerUpEffect` subclasses) for applying and removing timed effects cleanly.
- **`ShopManager` / `ShopUI`** — upgrade levels, costs and stat scaling per power-up, persisted via `PlayerPrefs`.
- **`UIManager`** — HUD, menus, pause and game-over panels, driven entirely by `GameManager` events.
- **`AudioManager`** — music and SFX, with scene-aware track switching and player settings.

### Project structure

```
Assets/
├── Scripts/
│   ├── Core/        # GameManager
│   ├── Player/      # Movement, PlayerHealth, animation
│   ├── Tiles/       # TileManager, TilePopulator, generation
│   ├── Power Up/    # Power-up effects, pickups, base classes
│   ├── UI/          # ShopManager, ShopUI, upgrade rows
│   ├── Settings/    # UIManager, Main Menu
│   └── Audio/       # AudioManager
├── Prefab/          # Tiles, obstacles, pickups
└── Scenes/          # Main Menu, Main Game


---

## Built with

- **Unity** (Unity 6 / current LTS — update to match your editor version)
- **C#**
- **TextMesh Pro** for UI
- **Blender** for 3D models

---

## Getting started

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/<repo-name>.git
   ```
2. Open the project in Unity Hub with the matching editor version.
3. Open the `Main Menu` scene under `Assets/Scenes/`.
4. Press **Play**.

---

## About
This project started as a university assignment and is being reworked as a portfolio piece — both to demonstrate clean, organised gameplay code and to practise 3D modelling in Blender.
