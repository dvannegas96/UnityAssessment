using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleEditor : EditorWindow
{
    private string assetBundleName;
    private string[] existingAssetBundles;
    private Texture2D previewTexture;
    private Vector2 scrollPosition;
    private string[] assetPaths;

    [MenuItem("Tools/Asset Bundle Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AssetBundleEditor));
    }

    private void OnEnable()
    {
        // Get a list of all existing asset bundles in the project
        existingAssetBundles = Directory.GetFiles(Application.dataPath + "/AssetBundles", "*.assetbundle");
        for (int i = 0; i < existingAssetBundles.Length; i++)
        {
            existingAssetBundles[i] = Path.GetFileName(existingAssetBundles[i]);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Asset Bundle", EditorStyles.boldLabel);
        assetBundleName = EditorGUILayout.TextField("Asset Bundle Name", assetBundleName);

        // Allow drag and drop of files to create asset bundle
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and drop assets here");
        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }
        else if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            assetPaths = new string[DragAndDrop.objectReferences.Length];
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[i]);
            }
            previewTexture = AssetPreview.GetAssetPreview(DragAndDrop.objectReferences[0]);
            evt.Use();
        }

        if (GUILayout.Button("Create"))
        {
            if (assetPaths != null && assetPaths.Length > 0)
            {
                CreateAssetBundle(assetBundleName, assetPaths);
                assetPaths = null;
                previewTexture = null;
            }
            else
            {
                Debug.LogError("No assets selected. Please drag and drop assets to the drop area.");
            }
        }

        GUILayout.Space(10);

        // Show preview of assets that will be included in the asset bundle
        if (previewTexture != null)
        {
            GUILayout.Label("Assets to include in asset bundle:");
            GUILayout.BeginHorizontal();
            GUILayout.Label(previewTexture, GUILayout.Width(64), GUILayout.Height(64));
            GUILayout.BeginVertical();
            foreach (string assetPath in assetPaths)
            {
                GUILayout.Label(assetPath);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    private void CreateAssetBundle(string assetBundleName, string[] assetPaths)
    {
        // Create a new asset bundle with the specified name
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = assetBundleName;
        buildMap[0].assetNames = assetPaths;

        // Build the asset bundle
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
        Debug.Log("Asset bundle created successfully!");
    }
}