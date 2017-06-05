using UnityEditor;
using UnityEngine;

public class ModelProcessor : AssetPostprocessor
{
    public void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter == null) { return; }
        modelImporter.globalScale = 0.01f;
    }
}
