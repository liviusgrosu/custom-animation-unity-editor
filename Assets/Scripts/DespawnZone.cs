using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnZone : MonoBehaviour
{
    public CoalSpawner CoalSpawner; 

    private void OnCollisionEnter(Collision other) {
        if (other.transform.name.Contains("Coal")) {
            CoalSpawner.DestroyOre(other.gameObject);
        }
    }
}
