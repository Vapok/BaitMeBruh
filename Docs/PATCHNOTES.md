# 0.0.2 - Development Release
* **Custom Culinary Content**:
  * Implemented `RegisterCustomFoods()` in `FishCulinaryManager.cs` registering custom consumable `TrollfishChowder` cloned from `CarrotSoup` template.
  * Embedded custom 256x256 icon `trollfish_chowder.png` as an assembly manifest resource loaded via `Jotunn.Utils.AssetUtils.LoadImage`.
  * Configured food stats: 15 Health, 45 Stamina, 1200s (20 min) burn time, 2 hp/tick regen.
* **Status Effect Integration**:
  * Created and registered custom `SE_Stats` (`SE_TrollfishChowder`) with Jotunn's `ItemManager`.
  * Configured `m_skillLevel = Skills.SkillType.Sneak` and `m_skillLevelModifier = 10f` for 1200s duration.
  * Bound status effect to `ItemDrop.ItemData.SharedData.m_consumeStatusEffect` for automatic execution on item consumption.
* **3D Drop Model & Willybach HD Texture Support**:
  * Implemented procedural 3D model generator in `BuildTrollfishChowder.cs` producing `drop_trollfish_chowder` within the `greatcatch` AssetBundle.
  * Designed custom 24-segment radial carved Scandinavian oak bowl mesh, concave soup surface disc with edge meniscus, and 3D garnish meshes (3 yellow mushroom caps + organic fan-shaped dorsal fin).
  * Generated high-definition (1024x1024 / 2K) PBR textures: `T_BowlWood_HD.png`, `T_TrollfishSoup_HD.png`, and `T_TrollfishGarnish_HD.png` for full compatibility with Willybach's HD Valheim.
  * Configured physical `BoxCollider` (`0.36m x 0.165m x 0.36m`) and `Rigidbody` for natural freeform world dropping.
  * Added `"attach"` child transform on `ItemPrefab` and registered `customChowder.ItemDrop` in `itemstandh.m_supportedItems` for seamless snapping onto horizontal table item stands.
* **Localization & Documentation**:
  * Added `$item_trollfish_chowder`, `$item_trollfish_chowder_desc`, `$se_trollfish_chowder`, and `$se_trollfish_chowder_tooltip` keys in `Translations/English.json`.
  * Updated README whole-fish cooking table and added the Valheim Native Fish Species & Biome Compendium.

# 0.0.1 - Development Release
* **Refactoring & Technical Debt Elimination**:
  * Extracted `FishSpawnerBackup` from `SpawnSystemUpdateSpawnListPatch.cs` into standalone class file `FishSpawnerBackup.cs` to adhere strictly to single-class per file invariant.
  * Centralized hammer piece discovery and recipe unlocking into `PieceUnlockHelper.cs`, consolidating duplicate reflection logic across `PlayerOnInventoryChangedPatch` and `PlayerUpdateKnownRecipesListPatch`.
  * Removed dead / problematic `PlayerUpdatePlacementGhostPatch.cs` which had imposed an artificial -3m vertical translation on water pieces, resolving the issue where placed fish nets sank below the surface.
  * Enforced fail-fast invariants in `TrapPieceManager.cs` (`InvalidOperationException`) when embedded asset bundle prefabs are missing, removing legacy fallback code that created chest containers.
  * Cleaned up unused `_fishingFloat` field in `FishingLineState.cs` and speculative forward check for `FishingRodReinforced` in `FishingTensionManager.cs`.
  * Corrected indentation and formatting in `FishingFloatCatchPatch.cs`.
* **Localization & Guidance**:
  * Added missing `$tutorial_gc_firstfish_text` localization key to `Translations/English.json` to complete Hugin tutorial sequence on initial fish catch.
* **Branding & Presentation**:
  * Staged and packaged authentic in-game low-poly boat angling scene icon with Norse knotwork title typography.

# 0.0.0 - Development Release
* **Initial Project Architecture**:
  * Established modular BepInEx plugin architecture utilizing `Vapok.Common` 3.21.1015 and `JotunnLib` 2.30.2.
  * Synchronized configuration pipeline via `ConfigSyncBase` and `ConfigRegistry`.
  * Configured `ILRepack` MSBuild task to internalize `Vapok.Valheim.Common.dll` into target assembly `BaitMeBruh.dll`.
* **Dynamic Tension Engine**:
  * Implemented `FishingFloat_UpdateLine_Patch` and `FishingFloat_Update_Patch` managing real-time tension calculus $T \in [0.0, 1.0]$.
  * Decoupled stamina drain from line reeling; hooked stamina consumption to tension build and rest phase reeling.
  * Added `TrapNetWaveSway` and custom procedural animations for buoyant net frames and wave dynamics.
* **Passive Trap & Net Mechanics**:
  * Implemented `PassiveTrap` mono component handling fuel (chum / bait), timer cadence, harvest capacity, and proximity territory checks.
  * Registered `piece_bait_creel`, `piece_fishnet_coastal`, and `piece_fishnet_deep` as piece tables on `PieceTables.Hammer`.
  * Implemented `NetKitManager` registering portable deployable kits `ItemFishnetCoastal` and `ItemFishnetDeep` with custom crate visuals and drop box colliders.
  * Added `PlayerUpdateKnownRecipesListPatch` to enforce unlock tracking when kit items or station requirements are met.
* **Bait & Culinary Systems**:
  * Implemented `BaitRecipeManager` registering alternative recipe configurations for `FishingBait` and 8 regional biome baits.
  * Implemented `FishCulinaryManager` adding campfire conversion entries to `piece_cookingstation` for `Fish1` and `Fish2` (25s cook time) and Cauldron whole-fish recipes.
* **Seated Fishing & Seafaring**:
  * Implemented `PlayerUseHotbarItemPatch` and `HumanoidUseItemPatch` permitting rod casting while attached to seats/rudders (`Player.IsAttached()`).
  * Implemented `ShipTrollingPatch` dynamically adjusting line length up to rod maximum distance while moving.
* **Dedicated Server Safety**:
  * Isolated client UI and game startup hooks using `GUIManager.IsHeadless()`.
* **Game Reference Alignment**:
  * Built against Valheim 1.0.17 publicized assemblies and Unity 6 (6000.0.75f1) engine runtime.
