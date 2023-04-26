using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleHandler : MonoBehaviour
{
    [SerializeField] private List<string> assetBundleNames;

    private void Start()
    {
        for (int i = 0; i < assetBundleNames.Count; i++)
        {
            LoadAssetBundle(assetBundleNames[i]);
        }
    }

    private void LoadAssetBundle(string assetBundleName)
    {
        // Load the asset bundle with the specified name
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundles/" + assetBundleName);

        // Load all assets from the asset bundle
        Object[] assets = assetBundle.LoadAllAssets();

        // Instantiate the loaded assets in the scene
        foreach (Object asset in assets)
        {
            Instantiate(asset);
        }

        // Unload the asset bundle when you're done with it
        assetBundle.Unload(false);
    }
}
