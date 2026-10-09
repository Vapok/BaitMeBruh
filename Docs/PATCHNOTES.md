# 1.0.0 - Initial Release

### Core Architecture & Framework
* **Target Runtimes & Dependencies**:
  * Compiled for BepInEx 5.4.2350, Jotunn 2.30.2, and Valheim 1.0.17 publicized assemblies under Unity 6 (`6000.0.75f1`) .NET Framework 4.8.
  * Internalized `Vapok.Valheim.Common.dll` (v3.1016.24) via MSBuild `ILRepack` into `BaitMeBruh.dll`.
  * Synchronized server-enforced configurations using `ConfigSyncBase` with local client UI overrides.
  * Isolated all client UI and engine rendering hooks with `GUIManager.IsHeadless()` for dedicated server safety.

### Early Rod Progression & 3D Assets
* **Primitive Fishing Rod (`FishingRodPrimitive`)**:
  * Implemented `RodManager.cs` registering `FishingRodPrimitive` with custom procedural mesh generation in `BuildPrimitiveRod.cs` (tapered hazel wood shaft, woven twine guide rings, bone tip guide, and organic grip wrap).
  * Generated dedicated PBR textures (`T_PrimitiveRod_Wood_D.png`, `T_PrimitiveRod_Wood_N.png`, `T_PrimitiveRod_Bone_D.png`, `T_PrimitiveRod_Bone_N.png`) and embedded custom 256x256 inventory icon `icon_primitive_rod.png`.
  * Configured projectile velocity (22.0 m/s), 2.8s minimum draw duration, 20m maximum casting range, and 1.0 m/s retrieval velocity.

### Dynamic Line Tension Mini-Game
* **Tension Engine (`FishingLineState`)**:
  * Implemented `FishingLineState` component tracking real-time line tension $T \in [0.0, 1.0]$.
  * Harmony patches on `FishingFloat.Update` and `FishingFloat.UpdateLine` to evaluate fish struggle and rest cycles, angular velocity, and distance dampening.
  * Decoupled stamina drain from constant line reeling; hooked stamina consumption to tension resistance and rest phase reeling.
  * Patched line snap threshold ($T = 1.0$ held over 0.5s or casting past overcast buffer limit).

### Casting Meter & HUD Architecture
* **Fishing Tension & Casting HUD (`FishingTensionHud`)**:
  * Procedural uGUI canvas featuring synchronized tension gauge, dynamic casting meter, and active bait display box.
  * Implemented 1-2s safe draw timing with quadratic acceleration ($drawPercentage^2$) scaling from 3-5m tap casts to rod maximum range.
  * Added line break risk alert when casting exceeds safe distance threshold (+10-15m buffer).
  * Bound client configuration entries `HudHorizontalOffset` (-300px default) and `HudVerticalOffset` (-250px default).

### Quick Bait Switching & Input System
* **Bait Selection Pipeline (`BaitSelectionManager`)**:
  * Tracks equipped bait per fishing rod instance, queries player inventory for available baits, and auto-equips valid baits.
  * Input polling in `FishingTensionHud.Update` supports keyboard cycling via `SwitchBaitKey` (`KeyCode.G`, Shift+G for reverse) and gamepad cycling via `SwitchBaitGamepadButton` (`JoyRBumper` / `KeyCode.JoystickButton5`).
  * Input polling guarded against active chat focus and open modal menus.

### Dynamic KeyHints Integration
* **Context-Sensitive Fishing Hints (`KeyHintsUpdateHintsPatch`)**:
  * Harmony postfix on `KeyHints.UpdateHints` targeting both `Keyboard` and `Gamepad` hint hierarchies under `m_fishingHints`.
  * Clones hint elements into `Keyboard` (`BaitMeBruh_SwitchBaitHintKB`) and `Gamepad` (`BaitMeBruh_SwitchBaitHintGP`).
  * Automatically resolves active controller glyphs using `ZInput.instance.GetBoundKeyString("JoyRBumper")` (`<sprite="..." name="...">`) with fallback to `$KEY_RBumper` / `"RB"`.
  * Dynamically updates `Block` action text between localized strings: "Pull" (idle), "Strike" (bite cue), "Reel" (hooked catch, safe tension), and "Release" (high tension warning).

### Sensory Cues & Audio/Visual Feedback
* **Fishing Cue Orchestrator (`FishingCueManager`)**:
  * Centralized audio playback at player position (`owner.transform.position`), particle amplification (2.5x splash burst at float), and transient light beacon spawning (`TransientLightFader.cs`).
  * Harmony postfix on `FishingFloat.RPC_Nibble` to trigger bite cues and golden illumination around the bobber.
  * Harmony postfix on `FishingFloat.TryToHook` to trigger hook-set audio, splash bursts, water flash, and header banner.

### Autonomous Harvesting & Passive Traps
* **Passive Trap Mechanics (`PassiveTrap`)**:
  * Mono component managing fuel consumption, production cadence, holding capacity, and 20m territory proximity checks.
  * Registered `piece_bait_creel` (shallow water creel converting Neck Tails into basic bait), `piece_fishnet_coastal` (coastal water net), and `piece_fishnet_deep` (open ocean anchored net).
  * Implemented `NetKitManager.cs` registering portable deployable kits `ItemFishnetCoastal` and `ItemFishnetDeep` with `Floating` buoyancy physics and refund dismantling via Hammer.

### Recipe & Food Configuration Architecture
* **Factory Pattern & Configuration Synced Recipes**:
  * Implemented `AssetFactory.cs` and `ConfigurableRecipe.cs` with BepInEx synced settings for crafting station, min station level, craft amount, ingredients, and enable/disable toggles.
  * `BaitFactory.cs` registering 9 alternative regional biome bait recipes at Workbench and Cauldron without merchant or trophy restrictions.
  * `FoodFactory.cs` registering `TrollfishChowder`, whole-fish mead bases, Black Soup broth, campfire cooking conversions, and dynamic food/effect stat configurations.
  * Custom consumable `TrollfishChowder` with custom status effect `SE_TrollfishChowder` (+10 Sneak skill, 1200s duration) and custom 3D carved oak bowl drop model (`drop_trollfish_chowder`) compatible with table item stands.

### Seated Angling & Boat Trolling
* **Seafaring & Seated Fishing Patches**:
  * Harmony patches on `Player.UseHotbarItem` and `Humanoid.UseItem` permitting rod casting and reeling while attached to seats, benches, and rudders (`Player.IsAttached()`).
  * Implemented `ShipTrollingPatch` dynamically adjusting line length up to rod maximum distance while moving.

### Localization & Multi-Language Architecture
* **Native Embedded Localization**:
  * Embedded 34 JSON language files as assembly resources loaded via Jotunn `LocalizationManager`.
  * Full native translations across 34 official Valheim languages (English, German, French, Spanish, Italian, Dutch, Swedish, Norwegian, Danish, Finnish, Icelandic, Polish, Russian, Ukrainian, Czech, Slovak, Bulgarian, Romanian, Croatian, Serbian, Macedonian, Portuguese [BR/EU], Lithuanian, Greek, Turkish, Chinese [Simplified/Traditional], Japanese, Korean, Hindi, Thai, and Georgian).
