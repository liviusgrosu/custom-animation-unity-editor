using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalSpawner : MonoBehaviour
{
    public GameObject CoalPrefab;
    public int CoalCapacity;
    public float SecondsToWait = 1f;
    private List<GameObject> CoalInstances;

    void Awake() {
        CoalInstances = new List<GameObject>();
        InvokeRepeating("SpawnOre", 1f, SecondsToWait);
    }

    void SpawnOre() {
        if(CoalInstances.Count < CoalCapacity) {
            GameObject coalInstance = Instantiate(CoalPrefab, transform.position, Quaternion.identity);
            CoalInstances.Add(coalInstance);
        }
        // Destroy in case they fall off to the void 
    }

    public void DestroyOre(GameObject ore) {
        CoalInstances.Remove(ore);
        Destroy(ore);
    }
}
