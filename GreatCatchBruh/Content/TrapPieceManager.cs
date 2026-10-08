using UnityEngine;
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

        AssetBundle bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("greatcatch", typeof(GreatCatchBruh).Assembly);
        GameObject customPrefab = bundle != null ? bundle.LoadAsset<GameObject>("piece_bait_creel") : null;

        CustomPiece piece;
        if (customPrefab != null)
        {
            int pieceLayer = LayerMask.NameToLayer("piece");
            if (pieceLayer >= 0)
            {
                customPrefab.layer = pieceLayer;
                foreach (Transform child in customPrefab.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = pieceLayer;
                }
            }

            Shader pieceShader = Shader.Find("Custom/Piece");
            if (pieceShader != null)
            {
                foreach (Renderer renderer in customPrefab.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (Material mat in renderer.materials)
                    {
                        if (mat != null)
                        {
                            mat.shader = pieceShader;
                        }
                    }
                }
            }

            Piece pieceComp = customPrefab.GetComponent<Piece>();
            if (pieceComp == null)
            {
                pieceComp = customPrefab.AddComponent<Piece>();
            }

            pieceComp.m_name = "$piece_bait_creel";
            pieceComp.m_description = "$piece_bait_creel_desc";
            pieceComp.m_category = Piece.PieceCategory.Crafting;
            pieceComp.m_waterPiece = false;
            pieceComp.m_noInWater = false;
            pieceComp.m_groundPiece = false;
            pieceComp.m_groundOnly = false;
            pieceComp.m_noClipping = true;
            pieceComp.m_extraPlacementDistance = 3;

            Sprite creelIcon = bundle != null ? bundle.LoadAsset<Sprite>("I_BaitCreel") : null;
            if (creelIcon == null && bundle != null)
            {
                Texture2D iconTex = bundle.LoadAsset<Texture2D>("I_BaitCreel");
                if (iconTex != null)
                {
                    creelIcon = Sprite.Create(iconTex, new Rect(0f, 0f, iconTex.width, iconTex.height), new Vector2(0.5f, 0.5f));
                }
            }

            if (creelIcon != null)
            {
                pieceComp.m_icon = creelIcon;
            }
            else
            {
                GameObject barrelSample = PrefabManager.Instance.GetPrefab("piece_chest_barrel");
                if (barrelSample != null)
                {
                    Piece samplePiece = barrelSample.GetComponent<Piece>();
                    if (samplePiece != null && samplePiece.m_icon != null)
                    {
                        pieceComp.m_icon = samplePiece.m_icon;
                    }
                }
            }

            WearNTear wearNTear = customPrefab.GetComponent<WearNTear>();
            if (wearNTear == null)
            {
                wearNTear = customPrefab.AddComponent<WearNTear>();
            }

            wearNTear.m_noRoofWear = true;
            wearNTear.m_noSupportWear = true;
            wearNTear.m_supports = false;
            wearNTear.m_health = 200f;
            wearNTear.m_materialType = WearNTear.MaterialType.Wood;
            wearNTear.m_staticPosition = true;

            ZNetView netView = customPrefab.GetComponent<ZNetView>();
            if (netView == null)
            {
                netView = customPrefab.AddComponent<ZNetView>();
            }

            netView.m_persistent = true;
            netView.m_type = (ZDO.ObjectType)2;
            netView.m_distant = false;
            netView.m_syncInitialScale = false;

            PassiveTrap trap = customPrefab.GetComponent<PassiveTrap>();
            if (trap == null)
            {
                trap = customPrefab.AddComponent<PassiveTrap>();
            }

            trap.m_trapType = TrapType.BaitCreel;
            trap.m_secPerUnit = 300f;
            trap.m_maxCapacity = 3;
            trap.m_minDepth = 0.1f;
            trap.m_maxDepth = 2.0f;

            TrapNetWaveSway sway = customPrefab.GetComponent<TrapNetWaveSway>();
            if (sway == null)
            {
                customPrefab.AddComponent<TrapNetWaveSway>();
            }

            piece = new CustomPiece(customPrefab, fixReference: true, config);
        }
        else
        {
            piece = new CustomPiece("piece_bait_creel", "piece_chest_barrel", config);
            Container container = piece.PiecePrefab.GetComponent<Container>();
            if (container != null)
            {
                UnityEngine.Object.DestroyImmediate(container);
            }

            Piece pieceComp = piece.PiecePrefab.GetComponent<Piece>();
            if (pieceComp != null)
            {
                pieceComp.m_name = "$piece_bait_creel";
                pieceComp.m_description = "$piece_bait_creel_desc";
                pieceComp.m_category = Piece.PieceCategory.Crafting;
                pieceComp.m_waterPiece = true;
                pieceComp.m_noInWater = false;
                pieceComp.m_groundPiece = false;
                pieceComp.m_groundOnly = false;
                pieceComp.m_noClipping = true;
                pieceComp.m_extraPlacementDistance = 3;
            }

            WearNTear wearNTear = piece.PiecePrefab.GetComponent<WearNTear>();
            if (wearNTear != null)
            {
                wearNTear.m_noRoofWear = true;
                wearNTear.m_noSupportWear = true;
                wearNTear.m_supports = true;
            }

            PassiveTrap trap = piece.PiecePrefab.AddComponent<PassiveTrap>();
            trap.m_trapType = TrapType.BaitCreel;
            trap.m_secPerUnit = 300f;
            trap.m_maxCapacity = 3;
            trap.m_minDepth = 0.1f;
            trap.m_maxDepth = 2.0f;
        }

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
