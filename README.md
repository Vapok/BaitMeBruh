<div align="center">

# 🛡️ GreatCatchBruh

### *A comprehensive Valheim fishing system overhaul featuring dynamic line tension, early rod progression, coastal fish traps, and seafaring integration.*

[![GitHub Release](https://img.shields.io/github/v/release/Vapok/GreatCatchBruh?include_prereleases&logo=github&style=for-the-badge)](https://github.com/Vapok/GreatCatchBruh/releases)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Vapok/GreatCatchBruh?logo=thunderstore&style=for-the-badge)](https://thunderstore.io/c/valheim/p/Vapok/GreatCatchBruh/)
<br>
[![Discord](https://img.shields.io/badge/Discord-Join%20Community-7289da?logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/5YAJkRFBXt)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

</div>

**GreatCatchBruh** completely overhauls Valheim's fishing mechanics. It replaces flat stamina depletion with a dynamic tension rhythm mini-game, unlocks early-game angling at Workbench Tier 1, decouples biome baits from late-game trophies, introduces passive fish traps and bait creels, and enables fishing from boat seats with line trolling.

---

<div align="center">

<br>

[![Survival Servers](https://raw.githubusercontent.com/Vapok/GreatCatchBruh/main/images/survivalservers_banner.png)](https://www.survivalservers.com/services/game_servers/valheim/?ref=vapok)

</div>

## 🛡️ Key Features

### 1. Dynamic Line Tension Mini-Game
* **Stamina Attrition Removed**: Replaced vanilla's flat stamina drain with a line tension system ($T \in [0.0, 1.0]$).
* **Escape vs. Rest Cycles**:
  * **Struggle Phase**: Fish thrashes and escapes. Reeling during struggle rapidly spikes line tension. Letting the line run dissipates tension.
  * **Rest Phase**: Fish pauses. Reeling during rest expends minimal stamina and quickly pulls the catch closer.
* **Line Snap Threshold**: Maintaining maximum tension ($T = 1.0$) for more than 0.5 seconds snaps the line.
* **Crosshair Tension Gauge**: Displays real-time tension with a color gradient (blue $\rightarrow$ amber $\rightarrow$ pulsing red) and audio cues.

### 2. Early Rod & Bait Progression
* **Primitive Fishing Rod (`FishingRodPrimitive`)**:
  * Craftable at **Workbench Tier 1** (5 Wood, 4 Leather Scraps, 2 Bone Fragments).
  * Balanced for early inland angling with a 16m cast range and 1.0 m/s reel speed.
* **Alternative Biome Bait Recipes (Batch: 20)**:
  * Decoupled from the late-game Food Preparation Table (`piece_preptable`) and rare mini-boss trophies.
  * Craftable at the **Workbench** (Meadows) and **Cauldron** (Tiers 1–5) using standard biome materials (e.g. Resin, Honey, Bone Fragments, Troll Hide, Ancient Seeds, Bloodbags, Entrails, Wolf Fangs, Needles, Chitin, and Charred Bones).

### 3. Passive Harvesting Build Pieces
* **Bait Creel (`piece_bait_creel`)**:
  * Placed in shallow water ($0.5\text{m} - 2.0\text{m}$).
  * Slowly generates biome-specific fishing bait over time (capacity: 20).
* **Coastal Fish Net (`piece_fishnet_coastal`)**:
  * Placed in coastal/river water ($1.5\text{m} - 4.0\text{m}$).
  * Harvests local freshwater and near-shore fish species (capacity: 3).
* **Deep-Sea Anchored Net (`piece_fishnet_deep`)**:
  * Placed in deep water and ocean shelves ($3.5\text{m} - 12.0\text{m}$).
  * Harvests pelagic ocean species and deep shelf fish (capacity: 6).
* Built on vanilla chest/container archetypes. Looking at traps displays water depth status and capacity.

### 4. Seafaring & Ship Fishing
* **Seated Angling**: Players can hold and cast fishing rods while seated on ship benches.
* **Line Trolling**: Moving vessels dynamically spool line length up to the rod maximum rather than snapping immediately.

### 5. Weather & Time-of-Day Bonuses
* **Rain & Storms**: +50% fish bite rate during wet weather.
* **Fog**: +30% fish attraction radius during foggy weather.
* **Dawn & Dusk**: +40% hook timing window during twilight hours.

### 6. Culinary Economy & Star Butchering
* **Early Butchering**: Unlocks raw fish butchering at **Workbench Tier 1**, scaling yields by star quality:
  * Quality 1: 2 Raw Fish
  * Quality 2: 6 Raw Fish
  * Quality 3: 10 Raw Fish
  * Quality 4: 14 Raw Fish
  * Quality 5: 18 Raw Fish
* **Campfire Spit Roasting**: Raw Perch and Pike can be roasted directly on the campfire cooking station.
* **Whole-Fish Cauldron Recipes**: Trollfish Chowder, Frost-Bite Mead Base (Tetra), Swamp Broth (Giant Herring), Mariner's Swimmer Mead (Coral Ostracod), Puffer Poison Brew, and Glowfin Eitr Ration (Anglerfish).
* **Fishing Hat Perks**: Wearing `HelmetFishingHat` grants +30% line snap tolerance and a 25% chance to salvage bait on catch.

---

## 🕹️ Controls Summary

| Action | Input / Control | Description |
| :--- | :--- | :--- |
| **Cast Line** | <kbd>Mouse 0</kbd> (Hold & Release) | Casts fishing float (distance scales with charge time). |
| **Hook Fish** | <kbd>Mouse 1</kbd> | Sets the hook when a fish bites the float. |
| **Reel Line** | <kbd>Mouse 1</kbd> (Hold) | Pulls the float/fish toward the angler. |
| **Inspect / Harvest Traps** | <kbd>E</kbd> | Opens inventory of Bait Creels and Fish Nets. |

---

## ⚙️ Configuration

Configuration settings are stored in `BepInEx/config/vapok.mods.greatcatchbruh.cfg`.

### Local Config
* **Show Splash on Startup**: Toggles display of the mod overview window upon game launch (Default: `true`).
* **Enable Anonymous Telemetry**: Toggles anonymous telemetry reporting to track active mod versions and crashes (Default: `true`).

---

## 🤝 Compatibility & Requirements

* **BepInEx**: 5.4.2350 or later.
* **Jotunn**: 2.30.2 or later.
* Compatible with common inventory and building mods.

---

## 📦 Installation

### Mod Manager (Recommended)
1. Install via **Gale**, **r2modman**, or **Thunderstore Mod Manager**.
2. Dependencies (`BepInExPack_Valheim`, `Jotunn`) will download automatically.

### Manual Installation
1. Extract the downloaded `.zip` archive.
2. Place the `GreatCatchBruh` folder into your `Valheim/BepInEx/plugins/` directory.
3. Launch the game.

---

## 💬 Feedback & Community

* Report issues or request features on [GitHub](https://github.com/Vapok/GreatCatchBruh/issues).
* Join the community on [Discord](https://discord.gg/5YAJkRFBXt) for discussions and support.
