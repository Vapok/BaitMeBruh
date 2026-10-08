# 0.0.0 - Development Release
* **Initial Project Architecture**:
  * Established modular BepInEx plugin architecture utilizing `Vapok.Common` 3.21.1015 and `JotunnLib` 2.30.2.
  * Synchronized configuration pipeline via `ConfigSyncBase` and `ConfigRegistry`.
  * Configured `ILRepack` MSBuild task to internalize `Vapok.Valheim.Common.dll` into target assembly `GreatCatchBruh.dll`.
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
