using UnityEngine;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using BaitMeBruh.Components;

namespace BaitMeBruh.Content;

public static class TrapPieceManager
{
    private static bool _registered;

    public static void Initialize()
    {
        PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        PieceManager.OnPiecesRegistered += FixTrapRequirements;
    }

    private static void RegisterPieces()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;

        AssetBundle bundle = TrapAssetManager.GetBundle();

        RegisterBaitCreel(bundle);
        RegisterCoastalNet(bundle);
        RegisterDeepNet(bundle);
    }

    private static void SetupPrefabVisuals(GameObject prefab)
    {
        int pieceLayer = LayerMask.NameToLayer("piece");
        if (pieceLayer >= 0)
        {
            prefab.layer = pieceLayer;
            foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = pieceLayer;
            }
        }

        Shader pieceShader = Shader.Find("Custom/Piece");
        if (pieceShader != null)
        {
            foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>(true))
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
    }

    private static void RegisterBaitCreel(AssetBundle bundle)
    {
        Sprite creelIcon = TrapAssetManager.GetSprite("I_BaitCreel");
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_bait_creel",
            Description = "$piece_bait_creel_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Crafting",
            Enabled = true,
            CraftingStation = "piece_workbench",
            Icon = creelIcon,
            Requirements = new[]
            {
                new RequirementConfig { Item = "Wood", Amount = 10, Recover = true },
                new RequirementConfig { Item = "TrophyNeck", Amount = 1, Recover = true },
                new RequirementConfig { Item = "Stone", Amount = 2, Recover = true }
            }
        };

        GameObject customPrefab = bundle != null ? bundle.LoadAsset<GameObject>("piece_bait_creel") : null;
        if (customPrefab == null)
        {
            throw new System.InvalidOperationException("[BaitMeBruh] Embedded bundle missing 'piece_bait_creel' prefab.");
        }

        SetupPrefabVisuals(customPrefab);

        Piece pieceComp = customPrefab.GetComponent<Piece>();
        if (pieceComp == null)
        {
            pieceComp = customPrefab.AddComponent<Piece>();
        }

        pieceComp.m_name = "$piece_bait_creel";
        pieceComp.m_description = "$piece_bait_creel_desc";
        pieceComp.m_category = Piece.PieceCategory.Crafting;
        pieceComp.m_enabled = true;
        pieceComp.m_waterPiece = false;
        pieceComp.m_noInWater = false;
        pieceComp.m_groundPiece = false;
        pieceComp.m_groundOnly = false;
        pieceComp.m_noClipping = true;
        pieceComp.m_extraPlacementDistance = 3;
        if (creelIcon != null)
        {
            pieceComp.m_icon = creelIcon;
        }

        WearNTear wearNTear = customPrefab.GetComponent<WearNTear>();
        if (wearNTear == null)
        {
            wearNTear = customPrefab.AddComponent<WearNTear>();
        }

        wearNTear.m_noRoofWear = false;
        wearNTear.m_noSupportWear = false;
        wearNTear.m_supports = true;
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

        CustomPiece piece = new CustomPiece(customPrefab, fixReference: true, config);
        PieceManager.Instance.AddPiece(piece);
    }

    private static void RegisterCoastalNet(AssetBundle bundle)
    {
        Sprite netIcon = TrapAssetManager.GetSprite("I_FishnetCoastal");
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_fishnet_coastal",
            Description = "$piece_fishnet_coastal_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Crafting",
            Enabled = true,
            CraftingStation = null,
            Icon = netIcon,
            Requirements = new[]
            {
                new RequirementConfig { Item = "ItemFishnetCoastal", Amount = 1, Recover = true }
            }
        };

        GameObject customPrefab = bundle != null ? bundle.LoadAsset<GameObject>("piece_fishnet_coastal") : null;
        if (customPrefab == null)
        {
            throw new System.InvalidOperationException("[BaitMeBruh] Embedded bundle missing 'piece_fishnet_coastal' prefab.");
        }

        SetupPrefabVisuals(customPrefab);

        Piece pieceComp = customPrefab.GetComponent<Piece>();
        if (pieceComp == null)
        {
            pieceComp = customPrefab.AddComponent<Piece>();
        }

        pieceComp.m_name = "$piece_fishnet_coastal";
        pieceComp.m_description = "$piece_fishnet_coastal_desc";
        pieceComp.m_category = Piece.PieceCategory.Crafting;
        pieceComp.m_enabled = true;
        pieceComp.m_craftingStation = null;
        pieceComp.m_waterPiece = true;
        pieceComp.m_noInWater = false;
        pieceComp.m_groundPiece = false;
        pieceComp.m_groundOnly = false;
        pieceComp.m_noClipping = true;
        pieceComp.m_extraPlacementDistance = 8;

        if (netIcon != null)
        {
            pieceComp.m_icon = netIcon;
        }
        else
        {
            GameObject kitPrefab = PrefabManager.Instance.GetPrefab("ItemFishnetCoastal");
            if (kitPrefab != null)
            {
                ItemDrop kitDrop = kitPrefab.GetComponent<ItemDrop>();
                if (kitDrop != null && kitDrop.m_itemData != null)
                {
                    pieceComp.m_icon = kitDrop.m_itemData.GetIcon();
                }
            }
        }

        WearNTear wearNTear = customPrefab.GetComponent<WearNTear>();
        if (wearNTear == null)
        {
            wearNTear = customPrefab.AddComponent<WearNTear>();
        }

        wearNTear.m_noRoofWear = false;
        wearNTear.m_noSupportWear = false;
        wearNTear.m_supports = true;
        wearNTear.m_health = 300f;
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

        trap.m_trapType = TrapType.CoastalNet;
        trap.m_secPerUnit = 600f;
        trap.m_maxCapacity = 2;
        trap.m_minDepth = 1.0f;
        trap.m_maxDepth = 6.0f;

        TrapNetWaveSway sway = customPrefab.GetComponent<TrapNetWaveSway>();
        if (sway == null)
        {
            customPrefab.AddComponent<TrapNetWaveSway>();
        }

        CustomPiece piece = new CustomPiece(customPrefab, fixReference: true, config);
        AssignPieceRequirement(piece.PiecePrefab, "ItemFishnetCoastal");
        PieceManager.Instance.AddPiece(piece);
    }

    private static void RegisterDeepNet(AssetBundle bundle)
    {
        Sprite netIcon = TrapAssetManager.GetSprite("I_FishnetDeep");
        PieceConfig config = new PieceConfig
        {
            Name = "$piece_fishnet_deep",
            Description = "$piece_fishnet_deep_desc",
            PieceTable = PieceTables.Hammer,
            Category = "Crafting",
            Enabled = true,
            CraftingStation = null,
            Icon = netIcon,
            Requirements = new[]
            {
                new RequirementConfig { Item = "ItemFishnetDeep", Amount = 1, Recover = true }
            }
        };

        GameObject customPrefab = bundle != null ? bundle.LoadAsset<GameObject>("piece_fishnet_deep") : null;
        if (customPrefab == null)
        {
            throw new System.InvalidOperationException("[BaitMeBruh] Embedded bundle missing 'piece_fishnet_deep' prefab.");
        }

        SetupPrefabVisuals(customPrefab);

        Piece pieceComp = customPrefab.GetComponent<Piece>();
        if (pieceComp == null)
        {
            pieceComp = customPrefab.AddComponent<Piece>();
        }

        pieceComp.m_name = "$piece_fishnet_deep";
        pieceComp.m_description = "$piece_fishnet_deep_desc";
        pieceComp.m_category = Piece.PieceCategory.Crafting;
        pieceComp.m_enabled = true;
        pieceComp.m_craftingStation = null;
        pieceComp.m_waterPiece = true;
        pieceComp.m_noInWater = false;
        pieceComp.m_groundPiece = false;
        pieceComp.m_groundOnly = false;
        pieceComp.m_noClipping = true;
        pieceComp.m_extraPlacementDistance = 10;

        if (netIcon != null)
        {
            pieceComp.m_icon = netIcon;
        }
        else
        {
            GameObject kitPrefab = PrefabManager.Instance.GetPrefab("ItemFishnetDeep");
            if (kitPrefab != null)
            {
                ItemDrop kitDrop = kitPrefab.GetComponent<ItemDrop>();
                if (kitDrop != null && kitDrop.m_itemData != null)
                {
                    pieceComp.m_icon = kitDrop.m_itemData.GetIcon();
                }
            }
        }

        WearNTear wearNTear = customPrefab.GetComponent<WearNTear>();
        if (wearNTear == null)
        {
            wearNTear = customPrefab.AddComponent<WearNTear>();
        }

        wearNTear.m_noRoofWear = false;
        wearNTear.m_noSupportWear = false;
        wearNTear.m_supports = true;
        wearNTear.m_health = 600f;
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

        trap.m_trapType = TrapType.DeepNet;
        trap.m_secPerUnit = 600f;
        trap.m_maxCapacity = 2;
        trap.m_minDepth = 5.0f;
        trap.m_maxDepth = 50.0f;

        TrapNetWaveSway sway = customPrefab.GetComponent<TrapNetWaveSway>();
        if (sway == null)
        {
            customPrefab.AddComponent<TrapNetWaveSway>();
        }

        CustomPiece piece = new CustomPiece(customPrefab, fixReference: true, config);
        AssignPieceRequirement(piece.PiecePrefab, "ItemFishnetDeep");
        PieceManager.Instance.AddPiece(piece);
    }

    private static void FixTrapRequirements()
    {
        AssignPieceRequirement("piece_fishnet_coastal", "ItemFishnetCoastal");
        AssignPieceRequirement("piece_fishnet_deep", "ItemFishnetDeep");
    }

    private static void AssignPieceRequirement(string piecePrefabName, string itemPrefabName)
    {
        GameObject piecePrefab = PrefabManager.Instance.GetPrefab(piecePrefabName);
        if (piecePrefab == null)
        {
            return;
        }

        AssignPieceRequirement(piecePrefab, itemPrefabName);
    }

    private static void AssignPieceRequirement(GameObject piecePrefab, string itemPrefabName)
    {
        if (piecePrefab == null)
        {
            return;
        }

        Piece pieceComp = piecePrefab.GetComponent<Piece>();
        if (pieceComp == null)
        {
            return;
        }

        GameObject itemPrefab = PrefabManager.Instance.GetPrefab(itemPrefabName);
        if (itemPrefab == null)
        {
            return;
        }

        ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
        if (itemDrop == null)
        {
            return;
        }

        pieceComp.m_category = Piece.PieceCategory.Crafting;
        pieceComp.m_enabled = true;
        pieceComp.m_resources = new Piece.Requirement[]
        {
            new Piece.Requirement
            {
                m_resItem = itemDrop,
                m_amount = 1,
                m_recover = true
            }
        };
    }
}
