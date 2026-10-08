using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using GreatCatchBruh.Components;

namespace GreatCatchBruh.Content;

public static class TrapPieceManager
{
    private static bool _registered;

    public static void Initialize()
    {
        PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
    }

    private static void RegisterPieces()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        RegisterBaitCreel();
        RegisterCoastalNet();
        RegisterDeepNet();
    }

    private static void RegisterBaitCreel()
    {
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_bait_creel",
            Description = "$piece_bait_creel_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Crafting",
            CraftingStation = "piece_workbench",
            Requirements = new[]
            {
                new RequirementConfig { Item = "Wood", Amount = 10, Recover = true },
                new RequirementConfig { Item = "TrophyNeck", Amount = 1, Recover = true },
                new RequirementConfig { Item = "Stone", Amount = 2, Recover = true }
            }
        };

        CustomPiece piece = new CustomPiece("piece_bait_creel", "piece_beehive", config);

        Beehive beehive = piece.PiecePrefab.GetComponent<Beehive>();
        if (beehive != null)
        {
            UnityEngine.Object.DestroyImmediate(beehive);
        }

        SpawnOnDamaged spawnOnDamaged = piece.PiecePrefab.GetComponent<SpawnOnDamaged>();
        if (spawnOnDamaged != null)
        {
            UnityEngine.Object.DestroyImmediate(spawnOnDamaged);
        }

        UnityEngine.Transform beeEffect = piece.PiecePrefab.transform.Find("BeeEffect");
        if (beeEffect != null)
        {
            UnityEngine.Object.DestroyImmediate(beeEffect.gameObject);
        }

        Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
        if (pieceComp != null)
        {
            pieceComp.m_name = "$piece_bait_creel";
            pieceComp.m_description = "$piece_bait_creel_desc";
            pieceComp.m_category = Piece.PieceCategory.Crafting;
            pieceComp.m_waterPiece = false;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_noClipping = false;
            pieceComp.m_extraPlacementDistance = 3;
        }

        WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
        if (wearNTear != null)
        {
            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = false;
            wearNTear.m_supports = true;
        }

        PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
        trap.m_trapType = TrapType.BaitCreel;
        trap.m_secPerUnit = 300f;
        trap.m_maxCapacity = 3;
        trap.m_minDepth = 0.1f;
        trap.m_maxDepth = 2.0f;

        PieceManager.Instance.AddPiece(piece);
    }

    private static void RegisterCoastalNet()
    {
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_fishnet_coastal",
            Description = "$piece_fishnet_coastal_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Crafting",
            CraftingStation = "piece_workbench",
            Requirements = new[]
            {
                new RequirementConfig { Item = "CoreWood", Amount = 10, Recover = true },
                new RequirementConfig { Item = "LeatherScraps", Amount = 6, Recover = true },
                new RequirementConfig { Item = "BronzeNails", Amount = 4, Recover = true },
                new RequirementConfig { Item = "Stone", Amount = 2, Recover = true }
            }
        };

        CustomPiece piece = new CustomPiece("piece_fishnet_coastal", "piece_chest", config);

        Container container = piece.PiecePrefab.GetComponent<Container>();
        if (container != null)
        {
            UnityEngine.Object.DestroyImmediate(container);
        }

        Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
        if (pieceComp != null)
        {
            pieceComp.m_waterPiece = true;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_extraPlacementDistance = 4;
            pieceComp.m_category = Piece.PieceCategory.Crafting;
        }

        WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
        if (wearNTear != null)
        {
            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = false;
            wearNTear.m_supports = true;
        }

        PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
        trap.m_trapType = TrapType.CoastalNet;
        trap.m_secPerUnit = 600f;
        trap.m_maxCapacity = 3;
        trap.m_minDepth = 1.0f;
        trap.m_maxDepth = 5.0f;

        PieceManager.Instance.AddPiece(piece);
    }

    private static void RegisterDeepNet()
    {
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_fishnet_deep",
            Description = "$piece_fishnet_deep_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Crafting",
            CraftingStation = "forge",
            Requirements = new[]
            {
                new RequirementConfig { Item = "AncientBark", Amount = 10, Recover = true },
                new RequirementConfig { Item = "IronNails", Amount = 8, Recover = true },
                new RequirementConfig { Item = "Guck", Amount = 4, Recover = true },
                new RequirementConfig { Item = "Chain", Amount = 2, Recover = true }
            }
        };

        CustomPiece piece = new CustomPiece("piece_fishnet_deep", "piece_chest", config);

        Container container = piece.PiecePrefab.GetComponent<Container>();
        if (container != null)
        {
            UnityEngine.Object.DestroyImmediate(container);
        }

        Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
        if (pieceComp != null)
        {
            pieceComp.m_waterPiece = true;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_extraPlacementDistance = 5;
            pieceComp.m_category = Piece.PieceCategory.Crafting;
        }

        WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
        if (wearNTear != null)
        {
            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = false;
            wearNTear.m_supports = true;
        }

        PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
        trap.m_trapType = TrapType.DeepNet;
        trap.m_secPerUnit = 450f;
        trap.m_maxCapacity = 6;
        trap.m_minDepth = 3.0f;
        trap.m_maxDepth = 20.0f;

        PieceManager.Instance.AddPiece(piece);
    }
}
