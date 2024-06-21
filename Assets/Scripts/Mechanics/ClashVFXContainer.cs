using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClashVFXType
{
    DestroyBoxInHand,
    DestroyBoxOnConveyor,
    PlaceBoxOnConveyor,
    SpeedPowerUp,
    StrengthPowerUp,
}

public class ClashVFXContainer : MonoBehaviour
{
    [SerializeField] private GameObject destroyBoxInHand;
    [SerializeField] private GameObject destroyBoxOnConveyor;
    [SerializeField] private GameObject placeBoxOnConveyor;
    [SerializeField] private GameObject speedPowerUp;
    [SerializeField] private GameObject strengthPowerUp;

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
            case ClashVFXType.SpeedPowerUp:
                return instance.speedPowerUp;
            case ClashVFXType.StrengthPowerUp:
                return instance.strengthPowerUp;
            case ClashVFXType.DestroyBoxOnConveyor:
                return instance.destroyBoxOnConveyor;
            case ClashVFXType.PlaceBoxOnConveyor:
                return instance.placeBoxOnConveyor;
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

        GameObject vfxInstance = Instantiate(vfxPrefab, parent);
        vfxInstance.transform.localPosition = position;
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
    
    public static void InstantiateVFX(ClashVFXType type, Vector3 position, float destroyAfterSeconds, Color color)
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

        if (type == ClashVFXType.PlaceBoxOnConveyor)
        {
            vfxInstance.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }

        ChangeParticlesColor(vfxInstance, color);

        instance.StartCoroutine(DestroyAfterSeconds(vfxInstance, destroyAfterSeconds));
    }
    

    private static IEnumerator DestroyAfterSeconds(GameObject vfxInstance, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(vfxInstance);
    }

    public static void ChangeParticlesColor(GameObject vfxInstance, Color color)
    {
        ParticleSystem[] particleSystems = vfxInstance.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            var mainModule = ps.main;
            mainModule.startColor = color;
        }
    }
}