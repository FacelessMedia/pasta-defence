using System.Collections.Generic;
using UnityEngine;

namespace PastaDefence.Core
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        private readonly Dictionary<int, Queue<GameObject>> pools = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            int key = prefab.GetInstanceID();

            if (pools.ContainsKey(key) && pools[key].Count > 0)
            {
                GameObject obj = pools[key].Dequeue();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }

            GameObject newObj = Instantiate(prefab, position, rotation);
            newObj.name = prefab.name;
            return newObj;
        }

        public void Return(GameObject obj, GameObject prefab)
        {
            int key = prefab.GetInstanceID();

            if (!pools.ContainsKey(key))
                pools[key] = new Queue<GameObject>();

            obj.SetActive(false);
            pools[key].Enqueue(obj);
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);

            // Fallback: pool by name if prefab reference not available
            int key = obj.name.GetHashCode();

            if (!pools.ContainsKey(key))
                pools[key] = new Queue<GameObject>();

            pools[key].Enqueue(obj);
        }

        public void PrewarmPool(GameObject prefab, int count)
        {
            int key = prefab.GetInstanceID();

            if (!pools.ContainsKey(key))
                pools[key] = new Queue<GameObject>();

            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.name = prefab.name;
                obj.SetActive(false);
                pools[key].Enqueue(obj);
            }
        }

        public void ClearPool(GameObject prefab)
        {
            int key = prefab.GetInstanceID();

            if (!pools.ContainsKey(key)) return;

            while (pools[key].Count > 0)
            {
                GameObject obj = pools[key].Dequeue();
                if (obj != null) Destroy(obj);
            }

            pools.Remove(key);
        }

        public void ClearAll()
        {
            foreach (var pool in pools.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    if (obj != null) Destroy(obj);
                }
            }
            pools.Clear();
        }
    }
}
