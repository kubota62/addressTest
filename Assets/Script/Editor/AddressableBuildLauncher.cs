﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using System;
using UnityEngine;

internal class BuildLauncher
{
    public static string build_script
        = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";

    public static string settings_asset
        = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

    public static string profile_webGLMobile = "RemoteWebGLMobile";
    public static string profile_webGLPC = "RemoteWebGLPC";
    private static AddressableAssetSettings settings;


    static void getSettingsObject(string settingsAsset)
    {
        // This step is optional, you can also use the default settings:
        //settings = AddressableAssetSettingsDefaultObject.Settings;

        settings
            = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                as AddressableAssetSettings;

        if (settings == null)
            Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                           $"a settings object.");
    }


    static void setProfile(string profile)
    {
        string profileId = settings.profileSettings.GetProfileId(profile);
        if (String.IsNullOrEmpty(profileId))
            Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                             $"using current profile instead.");
        else
            settings.activeProfileId = profileId;
    }


    static void setBuilder(IDataBuilder builder)
    {
        int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

        if (index > 0)
            settings.ActivePlayerDataBuilderIndex = index;
        else
            Debug.LogWarning($"{builder} must be added to the " +
                             $"DataBuilders list before it can be made " +
                             $"active. Using last run builder instead.");
    }


    static bool buildAddressableContent()
    {
        AddressableAssetSettings
            .BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success)
        {
            Debug.LogError("Addressables build error encountered: " + result.Error);
        }

        return success;
    }


    [MenuItem("Tools/Build Addressables")]
    public static bool BuildAddressablesWebGLMobile()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Build Addressables WebGLMobile", "", 0f);
            TextureFormatter.SetTextureFormatForWebGLMobile();
            EditorUtility.DisplayProgressBar("Build Addressables WebGLMobile", "", 0.5f);
            var flg = BuildAddressables(profile_webGLMobile);
            EditorUtility.DisplayProgressBar("Build Addressables WebGLMobile", "", 1f);

            EditorUtility.DisplayProgressBar("Build Addressables WebGLPC", "", 0f);
            TextureFormatter.SetTextureFormatForWebGLPC();
            EditorUtility.DisplayProgressBar("Build Addressables WebGLPC", "", 0.5f);
            flg &= BuildAddressables(profile_webGLPC);
            EditorUtility.DisplayProgressBar("Build Addressables WebGLPC", "", 1f);

            return flg;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static bool BuildAddressables(string profileName)
    {
        getSettingsObject(settings_asset);
        setProfile(profileName);
        IDataBuilder builderScript
            = AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) as IDataBuilder;

        if (builderScript == null)
        {
            Debug.LogError(build_script + " couldn't be found or isn't a build script.");
            return false;
        }

        setBuilder(builderScript);

        return buildAddressableContent();
    }
}
#endif