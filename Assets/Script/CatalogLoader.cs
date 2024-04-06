using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityEngine.UI;

namespace Script
{
    public class CatalogLoader : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void IsInCache(string cacheName);
        
        IEnumerator Start()
        {
            yield return CheckCatalogs();

            var handler = Addressables.LoadAssetAsync<GameObject>("Assets/Contents/Overlay Canvas.prefab");
            yield return handler;
            Instantiate(handler.Result);

            // activateOnLoadをtrue（デフォルト値）にするとシーン上のアセットの初期化まで行われる
            var op = Addressables.LoadSceneAsync("Assets/Contents/Scenes/SampleScene1.unity", LoadSceneMode.Additive, true);
            yield return op;
                
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
                
            PageContainer container = PageContainer.Instances[0];
            container.Push("Assets/TitlePage.prefab", false);

            yield return new WaitForSeconds(1);
            
            IsInCache("localization-assets-shared_assets_all.bundle");
            IsInCache("localization-string-tables-japanese(japan)(ja-jp)_assets_all.bundle");
        }

        IEnumerator CheckCatalogs()
        {

            List<string> catalogsToUpdate = new List<string>();
            var checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
            checkForUpdateHandle.Completed += op => { catalogsToUpdate.AddRange(op.Result); };

            yield return checkForUpdateHandle;

            foreach (var location in Addressables.ResourceLocators)
            {
                Debug.LogError("aaa");
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
            
            ContentCatalogData.ResourceLocator

            foreach (var location in Addressables.ResourceLocators)
            {
                foreach (var key in location.Keys)
                {
                    Debug.LogError(key.ToString());
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
    }
}