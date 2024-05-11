using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityEngine.UI;

namespace Script
{
    public static class Enviroment
    {
        public static string AssetVersion = "bbb";

        public static string SubBuildTarget
        {
            get
            {
#if false
                return "WebGLMobile";
#else
                return "WebGLPC";
#endif
            }
        }
    }

    public class CatalogLoader : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void IsInCache(string cacheName);

        IEnumerator Start()
        {
            Debug.Log($"Start {Enviroment.AssetVersion} : {Enviroment.SubBuildTarget}");
            yield return CheckCatalogs();
            Debug.Log($"CheckCatalogs END");

            Debug.Log($"LoadAssetAsync Assets/Contents/Overlay Canvas.prefab");
            var handler = Addressables.LoadAssetAsync<GameObject>("Assets/Contents/Overlay Canvas.prefab");
            yield return handler;
            Instantiate(handler.Result);

            Debug.Log($"END LoadAssetAsync Assets/Contents/Overlay Canvas.prefab");

            // activateOnLoadをtrue（デフォルト値）にするとシーン上のアセットの初期化まで行われる
            //var op = Addressables.LoadSceneAsync("Assets/Contents/Scenes/SampleScene1.unity", LoadSceneMode.Additive,true);
            //yield return op;

            //SceneManager.LoadScene(1, LoadSceneMode.Additive);

            Debug.Log($"LoadAssetAsync Assets/TitlePage.prefab");
            PageContainer container = PageContainer.Instances[0];
            container.Push("Assets/TitlePage.prefab", false);
            Debug.Log($"END LoadAssetAsync Assets/TitlePage.prefab");

            yield return new WaitForSeconds(1);

            // 依存アセット表示
            // var handler2 = Addressables.LoadResourceLocationsAsync("Download");
            // yield return handler2;
            // var bundles = handler2.Result.Where(v => v.HasDependencies)
            //     .SelectMany(v => v.Dependencies)
            //     .Distinct()
            //     .ToList();
            // foreach (var location in bundles)
            // {
            //     Debug.Log($"location _ {location}");
            // }
            //
            // var sss = handler2.Result.GroupBy(x => x.DependencyHashCode)
            //     .Select(g => g.SelectMany(v => v.Dependencies).First())
            //     .ToList();
            // foreach (var location in sss)
            // {
            //     Debug.Log($"location3 _ {location}");
            // }

            var handler2 = Addressables.DownloadDependenciesAsync("Download");
            yield return handler2;

            // DependencyHashCode でグルーピングする
            // foreach (IGrouping<int, IResourceLocation> groupedLocations in handler2.Result.GroupBy(x => x.DependencyHashCode))
            // {
            //     Debug.Log($"location2 _ {groupedLocations.ToString()}");
            // }

            // IsInCache("localization-assets-shared_assets_all.bundle");
            // IsInCache("localization-string-tables-japanese(japan)(ja-jp)_assets_all.bundle");
        }

        IEnumerator CheckCatalogs()
        {
            List<string> catalogsToUpdate = new List<string>();
            var checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
            checkForUpdateHandle.Completed += op => { catalogsToUpdate.AddRange(op.Result); };

            yield return checkForUpdateHandle;

            foreach (var location in Addressables.ResourceLocators)
            {
                // Debug.LogError("aaa");
            }

            if (catalogsToUpdate.Count > 0)
            {
                AsyncOperationHandle<List<IResourceLocator>> updateHandle =
                    Addressables.UpdateCatalogs(catalogsToUpdate);
                yield return updateHandle;

                foreach (var location in updateHandle.Result)
                {
                    Debug.LogError(string.Join("a", location.AllLocations.Select(v => v.ToString())));
                }

                Addressables.Release(updateHandle);
            }

            Addressables.Release(checkForUpdateHandle);


            foreach (var location in Addressables.ResourceLocators)
            {
                foreach (var key in location.Keys)
                {
                    // Debug.LogError(key.ToString());
                }
            }
        }

        IEnumerator CheckCatalogs2()
        {
            List<string> catalogsToUpdate = new List<string>();
            AsyncOperationHandle<List<string>> checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
            checkForUpdateHandle.Completed += op => { catalogsToUpdate.AddRange(op.Result); };

            yield return checkForUpdateHandle;

            if (catalogsToUpdate.Count > 0)
            {
                AsyncOperationHandle<List<IResourceLocator>> updateHandle =
                    Addressables.UpdateCatalogs(catalogsToUpdate);
                yield return updateHandle;
                Addressables.Release(updateHandle);
            }

            Addressables.Release(checkForUpdateHandle);
        }

        private void ChangeResolution()
        {
        }

        private void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += AtlasRequested;
        }

        private void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= AtlasRequested;
        }

        private async void AtlasRequested(string tag, System.Action<SpriteAtlas> atlasAction)
        {
            Debug.LogWarning($"AtlasRequested {tag}");

            var handle =
                Addressables.LoadAssetAsync<SpriteAtlas>($"Assets/Contents/Atlas/New Sprite Atlas.spriteatlasv2");
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"AtlasRequested atlasAction :{tag}");
                atlasAction.Invoke(handle.Result);
            }
        }
    }
}