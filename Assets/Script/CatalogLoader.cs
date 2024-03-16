using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityEngine.UI;

namespace Script
{
    public class CatalogLoader : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return CheckCatalogs();
            
            PageContainer container = PageContainer.Instances[0];
            container.Push("Assets/TitlePage.prefab", false);
        }

        IEnumerator CheckCatalogs()
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