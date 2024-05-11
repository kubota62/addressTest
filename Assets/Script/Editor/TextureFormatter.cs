using UnityEditor;
using UnityEngine;

/// <summary>
/// Spriteのフォーマットを一括で書き換える
/// </summary>
public class TextureFormatter
{
    private static TextureImporterPlatformSettings WebGLMobileSetting = new TextureImporterPlatformSettings
    {
        name = "WebGL",
        overridden = true,
        maxTextureSize = 2048,
        resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
        format = TextureImporterFormat.ASTC_8x8,
        textureCompression = TextureImporterCompression.Compressed,
        compressionQuality = 50,
    };
    
    private static TextureImporterPlatformSettings WebGLPCSetting = new TextureImporterPlatformSettings
    {
        name = "WebGL",
        overridden = true,
        maxTextureSize = 2048,
        resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
        format = TextureImporterFormat.DXT5,
        textureCompression = TextureImporterCompression.Compressed,
        compressionQuality = 50,
    };
    
    private static readonly string[] TargetFolders = { @"Assets\Contents\" };
    private static readonly string ProgressBarTitle = "Setting Texture Format";

    /// <summary>
    /// テクスチャーのフォーマットを一括設定する
    /// </summary>
    [MenuItem("Tools/テクスチャーフォーマット一括設定/WebGL Mobile")]
    public static void SetTextureFormatForWebGLMobile()
    {
        SetTextureFormat(WebGLMobileSetting);
    }
    
    [MenuItem("Tools/テクスチャーフォーマット一括設定/WebGL PC")]
    public static void SetTextureFormatForWebGLPC()
    {
        SetTextureFormat(WebGLPCSetting);
    }

    private static void SetTextureFormat(TextureImporterPlatformSettings textureSetting)
    {
        string[] guids = AssetDatabase.FindAssets("t:texture2D", TargetFolders);

        try
        {
            EditorUtility.DisplayProgressBar(ProgressBarTitle, "", 0f);
            for (var i = 0; i < guids.Length; ++i)
            {
                string guid = guids[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                SetTextureCompression(textureSetting, assetPath, i, guids.Length);
            }

            EditorUtility.DisplayProgressBar(ProgressBarTitle, "Refresh AssetDatabase ...", 1f);
            AssetDatabase.Refresh();
            Debug.Log("Texture format conversion is completed.");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void SetTextureCompression(TextureImporterPlatformSettings textureSetting, string assetPath, int count, int totalCount)
    {
        // Font テクスチャは除外
        if (assetPath.EndsWith(".ttf"))
        {
            return;
        }

        if (assetPath.EndsWith(".otf"))
        {
            return;
        }

        var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (textureImporter == null)
        {
            Debug.LogError($"TextureImporter not found : {assetPath}");
            return;
        }

        // 設定済みの場合
        var currentSetting = textureImporter.GetPlatformTextureSettings("WebGL");
        if (currentSetting.overridden && currentSetting.format == textureSetting.format)
        {
            return;
        }

        // Note. プログレスバーの表示はわずかに硬直時間があるようなので実際に処理する場合だけ表示
        float progress = (float)count / totalCount;
        EditorUtility.DisplayProgressBar(ProgressBarTitle, $"{assetPath} ({count} / {totalCount})", progress);

        SetImportSettings(textureImporter, textureSetting, assetPath);
        AssetDatabase.ImportAsset(assetPath);
    }

    private static void SetImportSettings(TextureImporter textureImporter, TextureImporterPlatformSettings textureSetting, string path)
    {
        var format = TextureImporterFormat.DXT5;
        var maxSize = textureImporter.maxTextureSize;

        textureSetting.maxTextureSize = textureImporter.maxTextureSize;
        textureImporter.SetPlatformTextureSettings(textureSetting);
        
        Debug.Log($"【{textureSetting.name} SpriteFormat】: {path} Set {format}");
    }
}