using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor {

    void OnPreprocessTexture()
    {

        if (assetPath.Contains("output"))
        {
            TextureImporter importer = assetImporter as TextureImporter;
            importer.textureType = TextureImporterType.Advanced;
            importer.textureFormat = TextureImporterFormat.ARGB32;
            importer.isReadable = true;
            importer.filterMode = FilterMode.Point;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            //importer.npotScale = TextureImporterNPOTScale.None;

            Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
            if (asset)
            {
                EditorUtility.SetDirty(asset);
            }
            else
            {
                importer.textureType = TextureImporterType.Advanced;
            }
        }
    }

}