using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalSpawner : MonoBehaviour
{
    public GameObject CoalPrefab;
    public int CoalCapacity;
    public float SecondsToWait = 1f;
    private List<GameObject> _coalInstances;

    void Awake() {
        _coalInstances = new List<GameObject>();
        InvokeRepeating("SpawnOre", 1f, SecondsToWait);
    }

    void SpawnOre() {
        if(_coalInstances.Count < CoalCapacity) {
            GameObject coalInstance = Instantiate(CoalPrefab, transform.position, Quaternion.identity);
            _coalInstances.Add(coalInstance);
        }
    }

    public void DestroyOre(GameObject ore) {
        try {
            _coalInstances.Remove(ore);
            Destroy(ore);
        }
        catch (Exception e) {
            Debug.LogError("Ore doesn't exist in list");
        }
    }
}
