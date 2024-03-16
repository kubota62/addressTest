using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Script
{
    public class SpriteChanger : MonoBehaviour
    {
        [SerializeField]
        private Button _button;
        
        [SerializeField]
        private Image _target;

        [SerializeField] private List<AssetReferenceSprite> spriteRefs;

        private int index = 0;
        
        void Start()
        {
            _button.onClick.AddListener(() => ChangeResolution());
        }

        private void ChangeResolution()
        {   
            var handle = spriteRefs[index].LoadAssetAsync<Sprite>();
            handle.Completed += Handle_Completed;
        }
        
        private void Handle_Completed(AsyncOperationHandle<Sprite> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                _target.sprite = spriteRefs[index].Asset as Sprite;
                index++;
            }
            else
            {
                Debug.LogError("AssetReference failed to load.");
            }
        }

        private void OnDestroy()
        {
            foreach (var reference in spriteRefs)
            {
                reference.ReleaseAsset();
            }
        }
    }
}