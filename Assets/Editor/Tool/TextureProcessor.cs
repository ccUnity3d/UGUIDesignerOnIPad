using UnityEngine;
using UnityEditor;

public class TextureProcessor : AssetPostprocessor
{
    public void OnPreprocessTexture()
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;
        if (textureImporter == null) { return; }
        textureImporter.textureType = TextureImporterType.Default;
        textureImporter.mipmapEnabled = false;
        textureImporter.filterMode = FilterMode.Trilinear;
        textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        textureImporter.maxTextureSize = 2048;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
    }
}
