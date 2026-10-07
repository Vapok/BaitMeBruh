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
            Category = "Misc",
            CraftingStation = "piece_workbench",
            Requirements = new[]
            {
                new RequirementConfig { Item = "Wood", Amount = 10, Recover = true },
                new RequirementConfig { Item = "FineWood", Amount = 4, Recover = true },
                new RequirementConfig { Item = "Stone", Amount = 2, Recover = true }
            }
        };

        CustomPiece piece = new CustomPiece("piece_bait_creel", "piece_chest", config);

        Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
        if (pieceComp != null)
        {
            pieceComp.m_waterPiece = true;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_extraPlacementDistance = 3;
        }

        WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
        if (wearNTear != null)
        {
            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = true;
        }

        Container container = piece.PiecePrefab.GetComponent<Container>();
        if (container != null)
        {
            container.m_name = "$piece_bait_creel";
            container.m_width = 4;
            container.m_height = 2;
        }

        PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
        trap.m_trapType = TrapType.BaitCreel;
        trap.m_secPerUnit = 360f;
        trap.m_maxCapacity = 20;
        trap.m_minDepth = 0.5f;
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
            Category = "Misc",
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

        Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
        if (pieceComp != null)
        {
            pieceComp.m_waterPiece = true;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_extraPlacementDistance = 4;
        }

        WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
        if (wearNTear != null)
        {
            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = true;
        }

        Container container = piece.PiecePrefab.GetComponent<Container>();
        if (container != null)
        {
            container.m_name = "$piece_fishnet_coastal";
            container.m_width = 3;
            container.m_height = 1;
        }

        PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
        trap.m_trapType = TrapType.CoastalNet;
        trap.m_secPerUnit = 900f;
        trap.m_maxCapacity = 3;
        trap.m_minDepth = 1.5f;
        trap.m_maxDepth = 4.0f;

        PieceManager.Instance.AddPiece(piece);
    }

    private static void RegisterDeepNet()
    {
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_fishnet_deep",
            Description = "$piece_fishnet_deep_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Misc",
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

        Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
        if (pieceComp != null)
        {
            pieceComp.m_waterPiece = true;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_extraPlacementDistance = 5;
        }

        WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
        if (wearNTear != null)
        {
            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = true;
        }

        Container container = piece.PiecePrefab.GetComponent<Container>();
        if (container != null)
        {
            container.m_name = "$piece_fishnet_deep";
            container.m_width = 3;
            container.m_height = 2;
        }

        PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
        trap.m_trapType = TrapType.DeepNet;
        trap.m_secPerUnit = 600f;
        trap.m_maxCapacity = 6;
        trap.m_minDepth = 3.5f;
        trap.m_maxDepth = 12.0f;

        PieceManager.Instance.AddPiece(piece);
    }
}
