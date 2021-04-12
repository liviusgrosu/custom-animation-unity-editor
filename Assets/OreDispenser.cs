using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDispenser : MonoBehaviour
{
    public Transform OreSpawn;
    public GameObject[] OreObjects;
    public int OreMaxAmount = 5;

    private float _maxOreSpawnLife = 30f;

    public void DispenseOre(float secondsToWait) {
        StartCoroutine(SpawnOre(secondsToWait));
    }

    IEnumerator SpawnOre(float secondsToWait) {
        for(int oreIdx = 0; oreIdx < OreMaxAmount; oreIdx++){
            GameObject oreInstance = Instantiate(OreObjects[Random.Range(0, OreObjects.Length)], OreSpawn.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsToWait / OreMaxAmount);
            // Destroy in case they fall off infinitely 
            Destroy(oreInstance, _maxOreSpawnLife);
        }
    }
}
