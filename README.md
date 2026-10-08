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

### 2. Early Rod & Progression
* **Primitive Fishing Rod (`FishingRodPrimitive`)**:
  * Craftable at **Workbench Tier 1** (5 Wood, 4 Leather Scraps, 2 Bone Fragments).
  * Balanced for early inland angling with a 16m cast range and 1.0 m/s reel speed.
* **Hugin Tutorial Hints**:
  * Introduces custom tutorials for crafting your first rod (hinting at neck chum), placing bait creels, landing your first catch, and discovering regional baits.

### 3. Passive Harvesting Build Pieces
* **Bait Creel (`piece_bait_creel`)**:
  * Craftable at Workbench with early Meadows materials (10 Wood, 1 Neck Trophy, 2 Stone).
  * Placed in shallow water ($0.1\text{m} - 2.0\text{m}$).
  * **Crowding Protection**: Enforces a minimum 20-meter territory between creels (configurable) to prevent dock spam.
  * **Chum Upkeep**: Fueled with fresh Neck Tails. Each tail yields 3 bait, holding up to 5 tails (15 bait capacity).
  * **Real-World Time Generation**: Generates 1 bait every 5 real-world minutes (configurable, max capacity: 3 harvested bait).
* **Coastal Fish Net (`piece_fishnet_coastal`)**:
  * Placed in coastal/river water ($1.0\text{m} - 5.0\text{m}$).
  * Harvests local freshwater and near-shore fish species (capacity: 3).
* **Deep-Sea Anchored Net (`piece_fishnet_deep`)**:
  * Placed in deep water and ocean shelves ($3.0\text{m} - 20.0\text{m}$).
  * Harvests pelagic ocean species and deep shelf fish (capacity: 6).

### 4. Seated Angling & Seafaring
* **Seated Fishing**: Full support for holding, charging, casting, and reeling fishing rods while seated on ship benches, furniture, and raft rudders.
* **Line Trolling**: Moving vessels dynamically spool line length up to the rod maximum rather than snapping immediately.

### 5. Weather, Twilight & Morning Bonuses
* **Morning Feeding Hours (5:00 AM – 7:00 AM)**: Heightened fish activity with +60% bite rate and +50% attraction radius.
* **Rain & Storms**: +50% fish bite rate during wet weather.
* **Fog**: +30% fish attraction radius during foggy weather.
* **Dawn & Dusk**: +40% hook timing window during twilight hours.

### 6. Culinary Economy & Whole Fish Cooking
* **Campfire Spit Roasting**: Whole Perch and Pike can be roasted directly over standard campfire cooking racks for hearty early sustenance.
* **Progression-Locked Filleting**: Raw fish filleting remains aligned with vanilla progression (fish cutting table).
* **Whole-Fish Cauldron Recipes**: Trollfish Chowder, Frost-Bite Mead Base (Tetra), Swamp Broth (Giant Herring), Mariner's Swimmer Mead (Grouper), Puffer Poison Brew, and Glowfin Eitr Ration (Anglerfish).
* **Fishing Hat Perks**: Wearing `HelmetFishingHat` grants +30% line snap tolerance and a 25% chance to salvage bait on catch.

---

## 🕹️ Controls Summary

| Action | Input / Control | Description |
| :--- | :--- | :--- |
| **Cast Line** | <kbd>Mouse 0</kbd> (Hold & Release) | Casts fishing float (distance scales with charge time). Works while standing or seated. |
| **Hook Fish** | <kbd>Mouse 1</kbd> | Sets the hook when a fish bites the float. |
| **Reel Line** | <kbd>Mouse 1</kbd> (Hold) | Pulls the float/fish toward the angler. |
| **Extract Catch / Bait** | <kbd>E</kbd> | Harvests catches from Bait Creels and Fish Nets. |
| **Add Chum** | <kbd>1–8</kbd> or <kbd>E</kbd> | Uses Neck Tail on Bait Creel to replenish chum fuel. |

---

## ⚙️ Configuration

Configuration settings are stored in `BepInEx/config/vapok.mods.GreatCatchBruh.cfg`.

### Server Settings (Synced)
* **Enable GreatCatchBruh**: Toggles all mod features (Default: `true`).
* **Bait Creel Proximity Distance**: Minimum meters required between Bait Creels before waters become overcrowded (Default: `20.0`).
* **Bait Produced Per Chum**: Number of bait yields produced per Neck Tail added as chum (Default: `3`).
* **Bait Creel Minutes Per Bait**: Real-world minutes required to produce one unit of bait (Default: `5.0`).

### UI Settings (Client)
* **HUD Horizontal Offset**: Horizontal pixel offset of the tension and retrieval HUD relative to screen center (Default: `-180.0`).
* **HUD Vertical Offset**: Vertical pixel offset of the tension and retrieval HUD relative to screen center (Default: `-46.0`).

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
