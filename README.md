<div align="center">

# 🛡️ GreatCatchBruh

### *An extensive complete overhaul of Valheim's vanilla fishing system featuring dynamic line tension, primitive rod progression, biome bait crafting, passive harvesting nets, seated seafaring angling, and culinary expansions.*

[![GitHub Release](https://img.shields.io/github/v/release/Vapok/GreatCatchBruh?include_prereleases&logo=github&style=for-the-badge)](https://github.com/Vapok/GreatCatchBruh/releases)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Vapok/GreatCatchBruh?logo=thunderstore&style=for-the-badge)](https://thunderstore.io/c/valheim/p/Vapok/GreatCatchBruh/)
<br>
[![Discord](https://img.shields.io/badge/Discord-Join%20Community-7289da?logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/5YAJkRFBXt)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

</div>

**GreatCatchBruh** completely overhauls Valheim's vanilla fishing mechanics from the ground level up. Instead of fishing being gated behind locating the Haldor merchant in the Black Forest and suffering flat stamina attrition, GreatCatchBruh establishes an authentic angling progression path starting in the Meadows at Workbench Tier 1. It replaces the vanilla stamina depletion with a responsive line tension and rhythm mini-game, decouples regional baits from merchant purchases and late-game trophies with comprehensive Cauldron crafting recipes, introduces autonomous coastal fish nets and bait creels, and allows anglers to fish while seated aboard sailing vessels with line trolling.

---

<div align="center">

<br>

[![Survival Servers](https://raw.githubusercontent.com/Vapok/GreatCatchBruh/main/images/survivalservers_banner.png)](https://www.survivalservers.com/services/game_servers/valheim/?ref=vapok)

</div>

## 🛡️ How GreatCatchBruh Overhauls Vanilla Fishing

| Vanilla Fishing System | GreatCatchBruh Overhaul |
| :--- | :--- |
| **Gated Behind Haldor**: Must find the merchant in the Black Forest to purchase a fishing rod and basic bait. | **Meadows Tier 1 Progression**: Craft the **Primitive Fishing Rod** at Workbench Tier 1 using early Meadows materials. |
| **Stamina Attrition**: Reeling constantly drains player stamina until depleted, resulting in lost fish. | **Dynamic Tension Rhythm Mini-Game**: Flat stamina drain is replaced by line tension ($T \in [0.0, 1.0]$) with fish struggle and rest cycles. Reeling during struggle builds tension; reeling during rest easily pulls the fish in. |
| **Merchant Bait Bottleneck**: Biome baits require purchasing base bait from Haldor and crafting with boss trophies. | **Decoupled Biome Bait Recipes**: 9 alternative crafting recipes at Workbench and Cauldron allow players to craft all biome baits using organic regional materials without trophies. |
| **Strictly Active Fishing Only**: No passive fish or bait harvesting exists in the game. | **Passive Traps & Nets**: Place **Bait Creels** in shallow water for autonomous bait production, and deploy **Coastal Fish Nets** or **Deep-Sea Anchored Nets** to passively harvest regional fish. |
| **No Seated Fishing**: Cannot cast or reel while seated on boats, benches, or rudders. Moving boats snap lines instantly. | **Seated Angling & Boat Trolling**: Fish freely while seated aboard longships, karves, or rafts. Moving vessels spool line dynamically instead of snapping. |
| **Late Filleting Gate**: Fish cannot be cooked whole on campfires without a fish cutting table. | **Campfire Spit Roasting & Cauldron Recipes**: Roast whole Perch and Pike directly over campfires. Brew whole-fish cauldron meads and broths. |

---

## 🎣 Dynamic Line Tension Mini-Game

* **Stamina Drain Removed**: Replaced vanilla's flat stamina attrition with a line tension system ($T \in [0.0, 1.0]$).
* **Escape vs. Rest Cycles**:
  * **Struggle Phase**: The fish thrashes, splashes, and attempts to pull away. Reeling while the fish struggles rapidly spikes line tension. Letting the line run dissipates tension.
  * **Rest Phase**: The fish pauses to catch its breath. Reeling during the rest window expends minimal stamina and quickly pulls the catch toward you.
* **Line Snap Threshold**: Maintaining maximum tension ($T = 1.0$) for more than 0.5 seconds will snap the fishing line, losing the bait and catch.
* **HUD Tension Gauge**: Real-time tension gauge displayed near the crosshair with a dynamic color gradient (calm blue $\rightarrow$ warning amber $\rightarrow$ pulsing red snap alert) accompanied by audio feedback.

---

## 🛶 Seated Angling & Seafaring Trolling

* **Seated Fishing**: Full support for charging, casting, and reeling fishing rods while seated on ship benches, furniture, and raft rudders.
* **Line Trolling**: Moving vessels dynamically spool line length up to the rod's maximum distance rather than snapping immediately, allowing anglers to troll behind sailing ships.

---

## ⏰ Weather, Twilight & Feeding Bonuses

Fish behavior dynamically responds to environmental conditions in the world:
* **Morning Feeding Hours (5:00 AM – 7:00 AM)**: +60% fish bite rate and +50% attraction radius.
* **Rain & Storms**: +50% fish bite rate during wet weather.
* **Fog**: +30% fish attraction radius during foggy weather.
* **Dawn & Dusk**: +40% hook timing window during twilight hours.

---

## 📦 Complete Recipes & Options for Getting Bait and Fish

### 1. Options for Obtaining Bait

#### A. Passive Bait Harvesting (Bait Creel)
* **Bait Creel (`piece_bait_creel`)**:
  * Crafted via Hammer under the **Crafting** tab within Workbench range.
  * Recipe: **10 Wood**, **1 Neck Trophy**, **2 Stone**.
  * Placement: Shallow shoreline water ($0.1\text{m} - 2.0\text{m}$).
  * **Crowding Protection**: Enforces a minimum 20-meter territory between creels (configurable) to prevent dock clustering.
  * **Chum Upkeep**: Add fresh Neck Tails (`NeckTail`) via hotbar or interact key. Each tail yields **3 Fishing Bait** and the creel holds up to 5 tails (15 bait capacity).
  * **Production**: Generates 1 Fishing Bait every 5 real-world minutes (harvest capacity: 3 bait).

#### B. Crafting Bait at Workbench & Cauldron
All regional baits can be crafted without merchant purchases or boss trophies:

| Bait Name | Item ID | Station | Min Level | Yield | Required Ingredients | Target Biome & Fish Attracted |
| :--- | :--- | :---: | :---: | :---: | :--- | :--- |
| **Fishing Bait** | `FishingBait` | Workbench | 1 | 20x | 5 Neck Tail, 2 Honey, 10 Bone Fragments | Meadows (Perch, Pike) |
| **Mossy Fishing Bait** | `FishingBaitForest` | Cauldron | 1 | 20x | 20 Fishing Bait, 4 Troll Hide, 2 Ancient Seed | Black Forest (Trollfish) |
| **Sticky Fishing Bait** | `FishingBaitSwamp` | Cauldron | 2 | 20x | 20 Fishing Bait, 4 Bloodbag, 4 Entrails | Swamp (Giant Herring) |
| **Cold Fishing Bait** | `FishingBaitCave` | Cauldron | 2 | 20x | 20 Fishing Bait, 6 Wolf Fang, 2 Freeze Gland | Mountain Caves (Tetra) |
| **Stingy Fishing Bait** | `FishingBaitPlains` | Cauldron | 3 | 20x | 20 Fishing Bait, 4 Needle, 4 Cloudberry | Plains (Grouper) |
| **Heavy Fishing Bait** | `FishingBaitOcean` | Cauldron | 3 | 20x | 20 Fishing Bait, 6 Chitin, 2 Serpent Meat | Ocean (Coral Cod) |
| **Misty Fishing Bait** | `FishingBaitMistlands` | Cauldron | 4 | 20x | 20 Fishing Bait, 2 Carapace, 2 Royal Jelly | Mistlands (Anglerfish) |
| **Hot Fishing Bait** | `FishingBaitAshlands` | Cauldron | 5 | 20x | 20 Fishing Bait, 4 Charred Bone, 2 Sulfur Stone | Ashlands (Magmafish) |
| **Frosty Fishing Bait** | `FishingBaitDeepNorth` | Cauldron | 5 | 20x | 20 Fishing Bait, 4 Freeze Gland, 4 Feathers | Deep North (Northern Salmon) |

---

### 2. Options for Obtaining Fish

#### A. Active Angling with Fishing Rods
* **Primitive Fishing Rod (`FishingRodPrimitive`)**:
  * Crafted at **Workbench Tier 1** with **5 Wood**, **4 Leather Scraps**, **2 Bone Fragments**.
  * Stats: 16m maximum cast range, 1.0 m/s reel speed, weight 1.0.
* **Standard Fishing Rod (`FishingRod`)**:
  * Purchased from Haldor or acquired through world progression.
  * Stats: 30m maximum cast range, 1.5 m/s reel speed.

#### B. Passive Fish Harvesting with Autonomous Nets
Harvest fish autonomously over time without active angling:

* **Coastal Fish Net (`piece_fishnet_coastal`)**:
  * Floating timber frame buoy with buoyant corner kegs, marker mast, and submerged wave-swaying net.
  * **Crafting Kit**: Crafted at **Workbench Tier 1** as a portable kit (`ItemFishnetCoastal`, Weight: 25): **15 Core Wood**, **10 Fine Wood**, **4 Bronze**, **8 Bronze Nails**, **4 Troll Hide**, **4 Stone**.
  * **Placement**: Placed with the Hammer anywhere in coastal waters and rivers ($1.0\text{m} - 6.0\text{m}$ depth). Consumes the kit item—no workbench needed nearby. Features an 8-meter reach for placement from shorelines or boats. Dismantling with the Hammer returns the kit.
  * **Bait & Catch Mechanics**: Fueled with Fishing Bait. Each bait catches 4 fish (capacity: 5 bait / 20 catches). Attracts native biome fish matching the loaded bait type. If the loaded bait does not match the local biome, it automatically converts to basic bait (attracting Perch and Pike).
  * **Harvest Rate**: Generates 1 fish every 10 real-world minutes (internal holding capacity: 2 fish).

* **Deep-Sea Anchored Net (`piece_fishnet_deep`)**:
  * Heavy ocean buoy cask with forged iron bands, sturdy tripod beacon cage, underwater spreader ring, and mooring chains holding a weighted deep net.
  * **Crafting Kit**: Crafted at **Forge Tier 1** as a heavy portable kit (`ItemFishnetDeep`, Weight: 50): **15 Ancient Bark**, **6 Iron**, **12 Iron Nails**, **4 Chain**, **4 Guck**.
  * **Placement**: Placed with the Hammer in deep open ocean waters ($5.0\text{m} - 50.0\text{m}$ depth). Consumes the kit item—no forge needed nearby. Features a 10-meter reach for deployment over boat railings. Dismantling returns the kit.
  * **Bait & Catch Mechanics**: Fueled with Fishing Bait. Each bait catches 4 fish (capacity: 5 bait / 20 catches). Attracts regional and pelagic fish matching the loaded bait. Unmatched bait reverts to basic bait.
  * **Harvest Rate**: Generates 1 fish every 10 real-world minutes (internal holding capacity: 2 fish).

---

### 3. Culinary Preparation & Whole Fish Recipes

#### A. Campfire Spit Roasting
* **Whole Perch (`Fish1`) & Pike (`Fish2`)**:
  * Placeable directly onto the standard campfire cooking station (`piece_cookingstation`).
  * Cooks in 25 seconds into **Cooked Fish (`FishCooked`)** without requiring a fish cutting table.

#### B. Whole-Fish Cauldron Cooking
* **Trollfish Chowder (`FishSoup`)**: Cauldron Tier 1 (1 Trollfish, 2 Yellow Mushroom).
* **Frost-Bite Mead Base (`MeadBaseFrostResist`)**: Cauldron Tier 1 (1 Tetra, 10 Honey, 5 Thistle, 1 Wolf Fang).
* **Swamp Fish Broth (`BlackSoup`)**: Cauldron Tier 2 (1 Giant Herring, 2 Bloodbag, 2 Entrails).
* **Mariner's Swimmer Mead Base (`MeadBaseSwimmer`)**: Cauldron Tier 2 (1 Grouper, 10 Honey, 2 Cloudberry).
* **Puffer Poison Mead Base (`MeadBasePoisonResist`)**: Cauldron Tier 2 (1 Pufferfish, 10 Honey, 4 Thistle).
* **Glowfin Eitr Mead Base (`MeadBaseEitrMinor`)**: Cauldron Tier 4 (1 Anglerfish, 10 Honey, 2 Magecap).

#### C. Fishing Hat Perks
* Equipping the **Fishing Hat (`HelmetFishingHat`)** grants **+30% line snap tolerance** and a **25% chance to salvage bait** when landing a fish.

---

## 🕹️ Controls Summary

| Action | Input / Control | Description |
| :--- | :--- | :--- |
| **Cast Line** | <kbd>Mouse 0</kbd> (Hold & Release) | Casts fishing float (distance scales with charge time). Operates while standing or seated. |
| **Hook Fish** | <kbd>Mouse 1</kbd> | Sets the hook when a fish nibbles or takes the float. |
| **Reel Line** | <kbd>Mouse 1</kbd> (Hold) | Reels line in. Build tension during struggle; reel during rest to retrieve catch. |
| **Harvest Catch / Bait** | <kbd>E</kbd> | Harvests catches from Bait Creels and Fish Nets. |
| **Add Chum / Bait** | <kbd>1–8</kbd> or <kbd>E</kbd> | Uses Neck Tail on Bait Creel, or Fishing Bait on Coastal/Deep Nets to replenish fuel. |

---

## ⚙️ Configuration

Configuration settings are stored in `BepInEx/config/vapok.mods.GreatCatchBruh.cfg`.

### Server Settings (Synced)
* **Enable GreatCatchBruh**: Toggles all mod features (Default: `true`).
* **Bait Creel Proximity Distance**: Minimum meters required between Bait Creels before waters become overcrowded (Default: `20.0`).
* **Bait Produced Per Chum**: Number of bait yields produced per Neck Tail added as chum (Default: `3`).
* **Bait Creel Minutes Per Bait**: Real-world minutes required to produce one unit of bait (Default: `5.0`).

### UI Settings (Client)
* **HUD Horizontal Offset**: Horizontal pixel offset of the tension gauge relative to screen center (Default: `-180.0`).
* **HUD Vertical Offset**: Vertical pixel offset of the tension gauge relative to screen center (Default: `-46.0`).

---

## 🤝 Compatibility & Requirements

* **BepInEx**: 5.4.2350 or later.
* **Jotunn**: 2.30.2 or later.
* Dedicated server, listen server, and solo play safe.

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
