using System;
using UnityEngine;

namespace BaitMeBruh.Content;

public static class TrapAssetManager
{
    private static AssetBundle _bundle;

    public static AssetBundle GetBundle()
    {
        if (_bundle == null)
        {
            _bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("greatcatch", typeof(BaitMeBruh).Assembly);
        }

        return _bundle;
    }

    public static Sprite GetSprite(string spriteName)
    {
        AssetBundle bundle = GetBundle();
        if (bundle == null)
        {
            return null;
        }

        Sprite sprite = bundle.LoadAsset<Sprite>(spriteName);
        if (sprite != null)
        {
            return sprite;
        }

        sprite = bundle.LoadAsset<Sprite>("Assets/Textures/" + spriteName + ".png");
        if (sprite != null)
        {
            return sprite;
        }

        sprite = bundle.LoadAsset<Sprite>("assets/textures/" + spriteName.ToLowerInvariant() + ".png");
        if (sprite != null)
        {
            return sprite;
        }

        string[] allAssetNames = bundle.GetAllAssetNames();
        for (int i = 0; i < allAssetNames.Length; i++)
        {
            string assetName = allAssetNames[i];
            if (assetName.IndexOf(spriteName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sprite = bundle.LoadAsset<Sprite>(assetName);
                if (sprite != null)
                {
                    return sprite;
                }

                UnityEngine.Object[] subAssets = bundle.LoadAssetWithSubAssets(assetName);
                if (subAssets != null)
                {
                    for (int s = 0; s < subAssets.Length; s++)
                    {
                        if (subAssets[s] is Sprite subSprite)
                        {
                            return subSprite;
                        }
                    }
                }

                Texture2D tex = bundle.LoadAsset<Texture2D>(assetName);
                if (tex != null)
                {
                    return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
        }

        Texture2D directTex = bundle.LoadAsset<Texture2D>(spriteName);
        if (directTex != null)
        {
            return Sprite.Create(directTex, new Rect(0f, 0f, directTex.width, directTex.height), new Vector2(0.5f, 0.5f));
        }

        return null;
    }
}
