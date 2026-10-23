using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    private static VfxPool instance;
    public static VfxPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<VfxPool>();
                if (instance == null)
                {
                    GameObject poolGo = new GameObject("[VfxPool]");
                    instance = poolGo.AddComponent<VfxPool>();
                }
            }
            return instance;
        }
    }

    [Header("Pool Settings")]
    [SerializeField] private bool persistAcrossScenes = false;

    private readonly Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<GameObject, string> instanceToKeyMap = new Dictionary<GameObject, string>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float autoReturnDuration = 2f)
    {
        if (prefab == null) return null;

        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        GameObject instance = null;

        while (poolDictionary[key].Count > 0)
        {
            GameObject candidate = poolDictionary[key].Dequeue();
            if (candidate != null)
            {
                instance = candidate;
                break;
            }
        }

        if (instance == null)
        {
            instance = Instantiate(prefab, position, rotation, transform);
        }
        else
        {
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.SetActive(true);
        }
        
        ParticleSystem[] particleSystems = instance.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            ps.Clear();
            ps.Play();
        }

        instanceToKeyMap[instance] = key;

        if (autoReturnDuration > 0f)
        {
            StartCoroutine(AutoReturnRoutine(instance, key, autoReturnDuration));
        }

        return instance;
    }

    public void ReturnToPool(GameObject instance)
    {
        if (instance == null) return;

        if (instanceToKeyMap.TryGetValue(instance, out string key))
        {
            ReturnToPoolInternal(instance, key);
        }
        else
        {
            Destroy(instance, 0.1f);
        }
    }

    private void ReturnToPoolInternal(GameObject instance, string key)
    {
        if (instance == null) return;

        instance.SetActive(false);
        instance.transform.SetParent(transform);

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        poolDictionary[key].Enqueue(instance);
    }

    private IEnumerator AutoReturnRoutine(GameObject instance, string key, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (instance != null && instance.activeSelf)
        {
            ReturnToPoolInternal(instance, key);
        }
    }
}
