using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClashVFXType
{
    DestroyBoxInHand
}

public class ClashVFXContainer : MonoBehaviour
{
    [SerializeField] private GameObject destroyBoxInHand;

    private static ClashVFXContainer instance;

    private void Awake()
    {
        // Ensure there is only one instance of ClashVFXContainer
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameObject GetVFXObject(ClashVFXType type)
    {
        if (instance == null)
        {
            Debug.LogError("ClashVFXContainer instance is not set.");
            return null;
        }

        switch (type)
        {
            case ClashVFXType.DestroyBoxInHand:
                return instance.destroyBoxInHand;
            default:
                return null;
        }
    }

    public static void InstantiateVFX(ClashVFXType type, Vector3 position, Transform parent, float destroyAfterSeconds)
    {
        GameObject vfxPrefab = GetVFXObject(type);
        if (vfxPrefab == null)
        {
            Debug.LogError($"VFX prefab for type {type} is not found.");
            return;
        }

        GameObject vfxInstance = Instantiate(vfxPrefab, position, Quaternion.identity, parent);
        if (vfxInstance == null)
        {
            Debug.LogError("Failed to instantiate VFX prefab.");
            return;
        }

        instance.StartCoroutine(DestroyAfterSeconds(vfxInstance, destroyAfterSeconds));
    }
    
    
    public static void InstantiateVFX(ClashVFXType type, Vector3 position, float destroyAfterSeconds)
    {
        GameObject vfxPrefab = GetVFXObject(type);
        if (vfxPrefab == null)
        {
            Debug.LogError($"VFX prefab for type {type} is not found.");
            return;
        }

        GameObject vfxInstance = Instantiate(vfxPrefab, position, Quaternion.identity);
        if (vfxInstance == null)
        {
            Debug.LogError("Failed to instantiate VFX prefab.");
            return;
        }

        instance.StartCoroutine(DestroyAfterSeconds(vfxInstance, destroyAfterSeconds));
    }

    private static IEnumerator DestroyAfterSeconds(GameObject vfxInstance, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(vfxInstance);
    }
}