<div align="center">

# 🛡️ BaitMeBruh

### *An extensive complete overhaul of Valheim's vanilla fishing system featuring dynamic line tension, primitive rod progression, biome bait crafting, passive harvesting nets, seated seafaring angling, and culinary expansions.*

[![GitHub Release](https://img.shields.io/github/v/release/Vapok/BaitMeBruh?include_prereleases&logo=github&style=for-the-badge)](https://github.com/Vapok/BaitMeBruh/releases)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Vapok/BaitMeBruh?logo=thunderstore&style=for-the-badge)](https://thunderstore.io/c/valheim/p/Vapok/BaitMeBruh/)
<br>
[![Discord](https://img.shields.io/badge/Discord-Join%20Community-7289da?logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/5YAJkRFBXt)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

</div>

**BaitMeBruh** completely overhauls Valheim's vanilla fishing mechanics from the ground level up. Instead of fishing being gated behind locating the Haldor merchant in the Black Forest and suffering flat stamina attrition, BaitMeBruh establishes an authentic angling progression path starting in the Meadows at Workbench Tier 1. It replaces the vanilla stamina depletion with a responsive line tension and rhythm mini-game, decouples regional baits from merchant purchases and late-game trophies with comprehensive Cauldron crafting recipes, introduces autonomous coastal fish nets and bait creels, and allows anglers to fish while seated aboard sailing vessels with line trolling.

---

<div align="center">

<br>

[![Survival Servers](https://raw.githubusercontent.com/Vapok/BaitMeBruh/main/images/survivalservers_banner.png)](https://www.survivalservers.com/services/game_servers/valheim/?ref=vapok)

</div>

## 🛡️ How BaitMeBruh Overhauls Vanilla Fishing

| Vanilla Fishing System | BaitMeBruh Overhaul |
| :--- | :--- |
| **Gated Behind Haldor**: Must find the merchant in the Black Forest to purchase a fishing rod and basic bait. | **Meadows Tier 1 Progression**: Craft the **Primitive Fishing Rod** at Workbench Tier 1 using early Meadows materials. |
| **Stamina Attrition**: Reeling constantly drains player stamina until depleted, resulting in lost fish. | **Dynamic Tension Rhythm Mini-Game**: Flat stamina drain is replaced by line tension ($T \in [0.0, 1.0]$) with fish struggle and rest cycles. Reeling during struggle builds tension; reeling during rest easily pulls the fish in. |
| **Merchant Bait Bottleneck**: Biome baits require purchasing base bait from Haldor and crafting with rare trophies. | **Decoupled Biome Bait Recipes**: 9 alternative crafting recipes at Workbench and Cauldron allow players to craft all biome baits using organic regional materials without trophies. |
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

## 🎣 Fishing Rods & Early Angling Gear

| Icon | Item & Prefab | Station | Crafting Requirements | Characteristics / Stats |
| :---: | :--- | :---: | :--- | :--- |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fishingrod.png" width="28" height="28" alt="Primitive Fishing Rod" /> | **Primitive Fishing Rod**<br>`FishingRodPrimitive` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/workbench.png" width="20" height="20" alt="Workbench" /> Workbench 1 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/wood.png" width="18" height="18" alt="Wood" /> 5x Wood<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/leatherscraps.png" width="18" height="18" alt="Leather Scraps" /> 4x Leather Scraps<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/bonefragments.png" width="18" height="18" alt="Bone Fragments" /> 2x Bone Fragments | • **Max Cast Range**: 16.0m<br>• **Reel Speed**: 1.0 m/s<br>• **Weight**: 1.0<br>• **Role**: Early inland and shoreline angling |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fishingrod.png" width="28" height="28" alt="Standard Fishing Rod" /> | **Fishing Rod**<br>`FishingRod` | *Haldor Trader* | Purchased from Haldor (350 Coins) | • **Max Cast Range**: 30.0m<br>• **Reel Speed**: 1.5 m/s<br>• **Weight**: 1.5<br>• **Role**: Advanced coastal and seafaring angling |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/HelmetFishingHat.png" width="28" height="28" alt="Fishing Hat" /> | **Fishing Hat**<br>`HelmetFishingHat` | *Haldor / Cauldron* | Crafted or purchased from Haldor | • **Armor**: 1<br>• **Snap Tolerance**: +30% Line snap time buffer<br>• **Bait Salvage**: 25% Chance to retain bait on catch |

---

## 🎒 Passive Harvesting Equipment & Deployable Kits

BaitMeBruh introduces autonomous harvesting pieces and portable deployable net kits. Coastal and deep-sea nets are crafted at stations as portable kits, then placed in the water via the Hammer anywhere without requiring a nearby crafting station.

### 1. Equipment & Kit Crafting Recipes

| Icon | Item / Piece Name | Station | Crafting Requirements | Weight / Stack | Placement & Operational Specs |
| :---: | :--- | :---: | :--- | :---: | :--- |
| <img src="https://raw.githubusercontent.com/Vapok/BaitMeBruh/main/images/I_BaitCreel.png" width="28" height="28" alt="Bait Creel" /> | **Bait Creel**<br>`piece_bait_creel` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/workbench.png" width="20" height="20" alt="Workbench" /> Hammer<br>*(Workbench)* | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/wood.png" width="18" height="18" alt="Wood" /> 10x Wood<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/TrophyNeck.png" width="18" height="18" alt="Neck Trophy" /> 1x Neck Trophy<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/stone.png" width="18" height="18" alt="Stone" /> 2x Stone | — | • **Placement**: Hammer (Crafting tab) in shallow waters ($0.1\text{m} - 2.0\text{m}$)<br>• **Reach**: 3.0m placement reach<br>• **Territory**: Enforces 20.0m minimum distance between creels<br>• **Health**: 200 HP |
| <img src="https://raw.githubusercontent.com/Vapok/BaitMeBruh/main/images/I_FishnetCoastal.png" width="28" height="28" alt="Coastal Fish Net Kit" /> | **Coastal Fish Net Kit**<br>`ItemFishnetCoastal`<br>*(Piece: `piece_fishnet_coastal`)* | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/workbench.png" width="20" height="20" alt="Workbench" /> Workbench 1 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/roundlog.png" width="18" height="18" alt="Core Wood" /> 15x Core Wood<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/finewood.png" width="18" height="18" alt="Fine Wood" /> 10x Fine Wood<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/bronze.png" width="18" height="18" alt="Bronze" /> 4x Bronze<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/bronzenails.png" width="18" height="18" alt="Bronze Nails" /> 8x Bronze Nails<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/troll_hide.png" width="18" height="18" alt="Troll Hide" /> 4x Troll Hide<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/stone.png" width="18" height="18" alt="Stone" /> 4x Stone | **25.0**<br>*(Stack: 1)* | • **Placement**: Hammer anywhere in coastal waters ($1.0\text{m} - 6.0\text{m}$)<br>• **Station Gating**: Consumes kit; **no workbench required nearby**<br>• **Reach**: 8.0m placement reach for shore and boat deployment<br>• **Dismantle**: Demolishing with Hammer refunds the complete kit<br>• **Health**: 400 HP |
| <img src="https://raw.githubusercontent.com/Vapok/BaitMeBruh/main/images/I_FishnetDeep.png" width="28" height="28" alt="Deep-Sea Anchored Net Kit" /> | **Deep-Sea Anchored Net Kit**<br>`ItemFishnetDeep`<br>*(Piece: `piece_fishnet_deep`)* | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/forge.png" width="20" height="20" alt="Forge" /> Forge 1 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/elderbark.png" width="18" height="18" alt="Ancient Bark" /> 15x Ancient Bark<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/iron.png" width="18" height="18" alt="Iron" /> 6x Iron<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/ironnails.png" width="18" height="18" alt="Iron Nails" /> 12x Iron Nails<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/chain.png" width="18" height="18" alt="Chain" /> 4x Chain<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/guck.png" width="18" height="18" alt="Guck" /> 4x Guck | **50.0**<br>*(Stack: 1)* | • **Placement**: Hammer in deep open ocean ($5.0\text{m} - 50.0\text{m}$)<br>• **Station Gating**: Consumes kit; **no forge required nearby**<br>• **Reach**: 10.0m placement reach for deployment over ship gunwales<br>• **Dismantle**: Demolishing with Hammer refunds the complete kit<br>• **Health**: 800 HP |

---

### 2. Fuel, Chum & Harvesting Operations

| Trap / Piece | Fuel / Chum Item | Consumption & Fuel Capacity | Production Cadence | Harvest Holding Capacity | Biome & Catch Behavior |
| :--- | :---: | :---: | :---: | :---: | :--- |
| **Bait Creel** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/necktail.png" width="20" height="20" alt="Neck Tail" /> Neck Tail (`NeckTail`) | **1 Neck Tail** yields **3 Fishing Bait**<br>*(Holds up to 5 tails / 15 bait capacity)* | **1 Bait per 5.0 min**<br>*(Real-world clock)* | **3 Bait**<br>*(Harvest with <kbd>E</kbd>)* | Produces basic **Fishing Bait** in shallow coastal waters. Replenish fuel with Neck Tails using <kbd>E</kbd> or hotbar. |
| **Coastal Fish Net** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="20" height="20" alt="Fishing Bait" /> Regional Fishing Bait | **1 Bait** yields **4 Fish**<br>*(Holds up to 5 baits / 20 catches)* | **1 Fish per 10.0 min**<br>*(Real-world clock)* | **2 Fish**<br>*(Harvest with <kbd>E</kbd>)* | Attracts native biome fish matching the loaded bait type (e.g. Mossy Bait in Black Forest catches Trollfish). If loaded bait does not match local biome, it reverts to basic bait (catching Perch and Pike). |
| **Deep-Sea Anchored Net** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="20" height="20" alt="Fishing Bait" /> Regional Fishing Bait | **1 Bait** yields **4 Fish**<br>*(Holds up to 5 baits / 20 catches)* | **1 Fish per 10.0 min**<br>*(Real-world clock)* | **2 Fish**<br>*(Harvest with <kbd>E</kbd>)* | Attracts deep pelagic and ocean fish (e.g. Heavy Bait catches Tuna & Coral Cod; Misty Bait catches Anglerfish & Pufferfish). Unmatched bait reverts to basic bait. |

---

## 🪱 Biome Bait Crafting Recipes (Workbench & Cauldron)

All regional baits can be crafted at the Workbench or Cauldron using organic biome resources. Boss trophies and merchant purchases are no longer required to advance through fishing tiers.

| Icon | Bait Name & ID | Station | Yield | Required Ingredients | Target Biome | Target Fish Attracted |
| :---: | :--- | :---: | :---: | :--- | :---: | :--- |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="28" height="28" alt="Fishing Bait" /> | **Fishing Bait**<br>`FishingBait` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/workbench.png" width="20" height="20" alt="Workbench" /> Workbench 1 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/necktail.png" width="18" height="18" alt="Neck Tail" /> 5x Neck Tail<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/honey.png" width="18" height="18" alt="Honey" /> 2x Honey<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/bonefragments.png" width="18" height="18" alt="Bone Fragments" /> 10x Bone Fragments | **Meadows** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish1.png" width="20" height="20" alt="Perch" /> **Perch**<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish2.png" width="20" height="20" alt="Pike" /> **Pike** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_forest.png" width="28" height="28" alt="Mossy Fishing Bait" /> | **Mossy Fishing Bait**<br>`FishingBaitForest` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 1 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/troll_hide.png" width="18" height="18" alt="Troll Hide" /> 4x Troll Hide<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/ancientseed.png" width="18" height="18" alt="Ancient Seed" /> 2x Ancient Seed | **Black Forest** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish5.png" width="20" height="20" alt="Trollfish" /> **Trollfish** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_swamp.png" width="28" height="28" alt="Sticky Fishing Bait" /> | **Sticky Fishing Bait**<br>`FishingBaitSwamp` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 2 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/bloodbag.png" width="18" height="18" alt="Bloodbag" /> 4x Bloodbag<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/entrails.png" width="18" height="18" alt="Entrails" /> 4x Entrails | **Swamp** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish6.png" width="20" height="20" alt="Giant Herring" /> **Giant Herring** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_cave.png" width="28" height="28" alt="Cold Fishing Bait" /> | **Cold Fishing Bait**<br>`FishingBaitCave` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 2 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/WolfFang.png" width="18" height="18" alt="Wolf Fang" /> 6x Wolf Fang<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/freezegland.png" width="18" height="18" alt="Freeze Gland" /> 2x Freeze Gland | **Mountains**<br>*(Frost Caves)* | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish4.png" width="20" height="20" alt="Tetra" /> **Tetra** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_plains.png" width="28" height="28" alt="Stingy Fishing Bait" /> | **Stingy Fishing Bait**<br>`FishingBaitPlains` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 3 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/needle.png" width="18" height="18" alt="Needle" /> 4x Needle<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/cloudberry.png" width="18" height="18" alt="Cloudberry" /> 4x Cloudberry | **Plains** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish7.png" width="20" height="20" alt="Grouper" /> **Grouper** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_ocean.png" width="28" height="28" alt="Heavy Fishing Bait" /> | **Heavy Fishing Bait**<br>`FishingBaitOcean` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 3 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/chitin.png" width="18" height="18" alt="Chitin" /> 6x Chitin<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/serpentmeat.png" width="18" height="18" alt="Serpent Meat" /> 2x Serpent Meat | **Ocean** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish3.png" width="20" height="20" alt="Tuna" /> **Tuna**<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish8.png" width="20" height="20" alt="Coral Cod" /> **Coral Cod** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_mistlands.png" width="28" height="28" alt="Misty Fishing Bait" /> | **Misty Fishing Bait**<br>`FishingBaitMistlands` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 4 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/carapace.png" width="18" height="18" alt="Carapace" /> 2x Carapace<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/royaljelly.png" width="18" height="18" alt="Royal Jelly" /> 2x Royal Jelly | **Mistlands** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish9.png" width="20" height="20" alt="Anglerfish" /> **Anglerfish**<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish12.png" width="20" height="20" alt="Pufferfish" /> **Pufferfish** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_ashlands.png" width="28" height="28" alt="Hot Fishing Bait" /> | **Hot Fishing Bait**<br>`FishingBaitAshlands` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 5 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/CharredBone.png" width="18" height="18" alt="Charred Bone" /> 4x Charred Bone<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/Sulfur.png" width="18" height="18" alt="Sulfur" /> 2x Sulfur | **Ashlands** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish11.png" width="20" height="20" alt="Magmafish" /> **Magmafish** |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_deepnorth.png" width="28" height="28" alt="Frosty Fishing Bait" /> | **Frosty Fishing Bait**<br>`FishingBaitDeepNorth` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 5 | **20x** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> 20x Fishing Bait<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/freezegland.png" width="18" height="18" alt="Freeze Gland" /> 4x Freeze Gland<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/feather.png" width="18" height="18" alt="Feathers" /> 4x Feathers | **Deep North** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish10.png" width="20" height="20" alt="Northern Salmon" /> **Northern Salmon** |

---

## 🍲 Culinary Economy & Whole Fish Cooking

BaitMeBruh integrates caught fish directly into camp cooking and alchemy, allowing whole fish to be cooked immediately without requiring late-game filleting stations.

### 1. Campfire Spit Roasting

| Input Item | Cooking Station | Cook Time | Result Item | Nutritional Benefits |
| :--- | :---: | :---: | :--- | :--- |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish1.png" width="24" height="24" alt="Whole Perch" /> **Whole Perch** (`Fish1`) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cooking_station.png" width="24" height="24" alt="Cooking Station" /> Standard Cooking Station | **25.0s** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish_cooked.png" width="24" height="24" alt="Cooked Fish" /> **Cooked Fish** (`FishCooked`) | • **Max Health**: 45<br>• **Max Stamina**: 15<br>• **Duration**: 20 min<br>• **HP Regen**: 2 hp/tick |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish2.png" width="24" height="24" alt="Whole Pike" /> **Whole Pike** (`Fish2`) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cooking_station.png" width="24" height="24" alt="Cooking Station" /> Standard Cooking Station | **25.0s** | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish_cooked.png" width="24" height="24" alt="Cooked Fish" /> **Cooked Fish** (`FishCooked`) | • **Max Health**: 45<br>• **Max Stamina**: 15<br>• **Duration**: 20 min<br>• **HP Regen**: 2 hp/tick |

---

### 2. Whole-Fish Cauldron Cooking Recipes

| Icon | Result Item & ID | Station | Ingredients | Role / Culinary Benefit |
| :---: | :--- | :---: | :--- | :--- |
| <img src="https://raw.githubusercontent.com/Vapok/BaitMeBruh/main/images/trollfish_chowder.png" width="28" height="28" alt="Trollfish Chowder" /> | **Trollfish Chowder**<br>*(Custom Food)*<br>`TrollfishChowder` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 1 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish5.png" width="18" height="18" alt="Trollfish" /> 1x Trollfish (`Fish5`)<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/mushroomyellow.png" width="18" height="18" alt="Yellow Mushroom" /> 2x Yellow Mushroom | • **High-Stamina Soup**: Health 15, Stamina 45, Duration 20 min, HP Regen 2 hp/tick<br>• **Sneak Bonus**: +10 Sneak (*Troll's Guile*, 20 min) |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/MeadBaseFrostResist.png" width="28" height="28" alt="MeadBaseFrostResist" /> | **Frost-Bite Mead Base**<br>`MeadBaseFrostResist` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 1 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish4.png" width="18" height="18" alt="Tetra" /> 1x Tetra (`Fish4_cave`)<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/honey.png" width="18" height="18" alt="Honey" /> 10x Honey<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/thistle.png" width="18" height="18" alt="Thistle" /> 5x Thistle<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/WolfFang.png" width="18" height="18" alt="Wolf Fang" /> 1x Wolf Fang | • **Frost Resistance**: Ferments into Frost Resistance Mead for mountain exploration |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/BlackSoup.png" width="28" height="28" alt="Black Soup" /> | **Swamp Fish Broth**<br>`BlackSoup` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 2 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish6.png" width="18" height="18" alt="Giant Herring" /> 1x Giant Herring (`Fish6`)<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/bloodbag.png" width="18" height="18" alt="Bloodbag" /> 2x Bloodbag<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/entrails.png" width="18" height="18" alt="Entrails" /> 2x Entrails | • **Balanced Soup**: Health 50, Stamina 17, Duration 20 min |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/MeadBaseSwimmer.png" width="28" height="28" alt="MeadBaseSwimmer" /> | **Mariner's Swimmer Mead Base**<br>`MeadBaseSwimmer` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 2 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish3.png" width="18" height="18" alt="Tuna" /> 1x Tuna (`Fish3`)<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/honey.png" width="18" height="18" alt="Honey" /> 10x Honey<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/cloudberry.png" width="18" height="18" alt="Cloudberry" /> 2x Cloudberry | • **Aquatic Utility**: Ferments into Draught of Vananidir granting -50% swim stamina consumption (5 min) |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/MeadBasePoisonResist.png" width="28" height="28" alt="MeadBasePoisonResist" /> | **Puffer Poison Mead Base**<br>`MeadBasePoisonResist` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 2 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish12.png" width="18" height="18" alt="Pufferfish" /> 1x Pufferfish (`Fish12`)<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/honey.png" width="18" height="18" alt="Honey" /> 10x Honey<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/thistle.png" width="18" height="18" alt="Thistle" /> 4x Thistle | • **Poison Immunity**: Ferments into Poison Resistance Mead for swamp survival |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/MeadBaseEitrMinor.png" width="28" height="28" alt="MeadBaseEitrMinor" /> | **Glowfin Eitr Mead Base**<br>`MeadBaseEitrMinor` | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/pieces/cauldron.png" width="20" height="20" alt="Cauldron" /> Cauldron 4 | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish9.png" width="18" height="18" alt="Anglerfish" /> 1x Anglerfish (`Fish9`)<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/honey.png" width="18" height="18" alt="Honey" /> 10x Honey<br><img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/mushroommagecap.png" width="18" height="18" alt="Magecap" /> 2x Magecap | • **Arcane Mana**: Ferments into Minor Eitr Mead restoring magical energy |

---

## 🐟 Valheim Native Fish Species & Biome Compendium

| Icon | Species & Item ID | Primary Habitat / Biome | Preferred Bait | Catch Notes & Ecology |
| :---: | :--- | :--- | :--- | :--- |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish1.png" width="28" height="28" alt="Perch" /> | **Perch**<br>`Fish1` | Meadows, Rivers & Coastlines | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> Fishing Bait | Common shoreline fish. Swims in quiet coves and fresh riverways. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish2.png" width="28" height="28" alt="Pike" /> | **Pike**<br>`Fish2` | Meadows & Black Forest (Rivers & Lakes) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait.png" width="18" height="18" alt="Fishing Bait" /> Fishing Bait | Aggressive predatory fish patrolling deeper inland river pools. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish3.png" width="28" height="28" alt="Tuna" /> | **Tuna**<br>`Fish3` | Ocean (Deep Open Ocean) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_ocean.png" width="18" height="18" alt="Heavy Bait" /> Heavy Fishing Bait | Massive, powerful pelagic swimmer that races through deep open ocean waters. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish4.png" width="28" height="28" alt="Tetra" /> | **Tetra**<br>`Fish4_cave` | Mountains (Subterranean Frost Caves) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_cave.png" width="18" height="18" alt="Cold Bait" /> Cold Fishing Bait | Rare blind cave dweller adapted to subterranean glacial lakes. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish5.png" width="28" height="28" alt="Trollfish" /> | **Trollfish**<br>`Fish5` | Black Forest (Coast & Ocean borders) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_forest.png" width="18" height="18" alt="Mossy Bait" /> Mossy Fishing Bait | Heavy, moss-colored fish lurking near rocky points and troll shorelines. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish6.png" width="28" height="28" alt="Giant Herring" /> | **Giant Herring**<br>`Fish6` | Swamp (Marsh Shorelines & Channels) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_swamp.png" width="18" height="18" alt="Sticky Bait" /> Sticky Fishing Bait | Heavy schooling fish that feeds on decaying organic silt in the murky swamps. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish7.png" width="28" height="28" alt="Grouper" /> | **Grouper**<br>`Fish7` | Plains (Warm Coastal Shallows & Shoals) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_plains.png" width="18" height="18" alt="Stingy Bait" /> Stingy Fishing Bait | Powerful predatory reef fish dwelling near sunny sandy shoals. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish8.png" width="28" height="28" alt="Coral Cod" /> | **Coral Cod**<br>`Fish8` | Ocean (Offshore Waters & Reefs) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_ocean.png" width="18" height="18" alt="Heavy Bait" /> Heavy Fishing Bait | Vibrant deep-water cod swimming along open ocean ridges and reefs. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish9.png" width="28" height="28" alt="Anglerfish" /> | **Anglerfish**<br>`Fish9` | Mistlands (Mist-covered Oceans & Fjords) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_mistlands.png" width="18" height="18" alt="Misty Bait" /> Misty Fishing Bait | Bioluminescent deep-dweller with a glowing lure that pierces heavy fog. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish10.png" width="28" height="28" alt="Northern Salmon" /> | **Northern Salmon**<br>`Fish10` | Deep North (Frigid Coastal Shelf) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_deepnorth.png" width="18" height="18" alt="Frosty Bait" /> Frosty Fishing Bait | Powerful oceanic salmon thriving in freezing ice floe corridors. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish11.png" width="28" height="28" alt="Magmafish" /> | **Magmafish**<br>`Fish11` | Ashlands (Boiling Seas & Lava Banks) | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_ashlands.png" width="18" height="18" alt="Hot Bait" /> Hot Fishing Bait | Armored volcanic swimmer immune to boiling seas and sulfur shores. |
| <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/fish12.png" width="28" height="28" alt="Pufferfish" /> | **Pufferfish**<br>`Fish12` | Mistlands & Deep Ocean | <img src="https://raw.githubusercontent.com/Vapok/ValheimDifferences/main/assets/icons/items/FishingBait_mistlands.png" width="18" height="18" alt="Misty Bait" /> Misty Fishing Bait | Venomous pufferfish that swells defensively with toxic spines when hooked. |

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

## ⚙️ Configuration Reference

Configuration settings are stored in `BepInEx/config/vapok.mods.BaitMeBruh.cfg`.

### Server Settings (Synced)
* **Enable BaitMeBruh**: Toggles all mod features (Default: `true`).
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
* Fully compatible with dedicated servers, player-hosted sessions, and solo local play.

---

## 📦 Installation

### Mod Manager (Recommended)
1. Install via **Gale**, **r2modman**, or **Thunderstore Mod Manager**.
2. Dependencies (`BepInExPack_Valheim`, `Jotunn`) will download automatically.

### Manual Installation
1. Extract the downloaded `.zip` archive.
2. Place the `BaitMeBruh` folder into your `Valheim/BepInEx/plugins/` directory.
3. Launch the game.

---

## 💬 Feedback & Community

* Report issues or request features on [GitHub](https://github.com/Vapok/BaitMeBruh/issues).
* Join the community on [Discord](https://discord.gg/5YAJkRFBXt) for discussions and support.
