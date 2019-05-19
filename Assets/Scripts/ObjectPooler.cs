using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler {
    private const int DefaultCapacity = 100;

    private readonly GameObject prefab;
    private readonly Stack<GameObject> pool;

    public ObjectPooler(GameObject prefab) {
        this.prefab = prefab;
        pool = new Stack<GameObject>(DefaultCapacity);
    }

    /// <summary>
    /// Get a gameobject out of the pool and enable it if necessary
    /// </summary>
    /// <returns>null if the init prefab was null, else a clone of it</returns>
    public GameObject GetObject() {
        if (prefab == null)
            return null;

        if (pool.Count >= 1) {
            var go = pool.Pop();
            ToggleGameObject(go, true);  // enable the pooled gameobject
            return go;
        }
        // return a new gameobject if the pool was empty
        return Object.Instantiate(prefab, Vector3.zero, prefab.transform.rotation);
    }

    /// <summary>
    /// Disable a gameobject and add it to the pool of unused objects
    /// </summary>
    /// <param name="gameObject">The gameobject no more used</param>
    public void AddObject(GameObject gameObject) {
        ToggleGameObject(gameObject, false);
        pool.Push(gameObject);
    }

    // Disable or enable a gameobject
    private static void ToggleGameObject(GameObject gameObject, bool enable) {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = enable;
    }
}