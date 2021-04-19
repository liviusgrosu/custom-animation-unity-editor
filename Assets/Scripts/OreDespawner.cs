using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDespawner : MonoBehaviour
{
    public OreManager OreManager;

    private void OnTriggerEnter(Collider col) {
        if (col.tag != "Ore") {
            return;
        }

        string oreName = "";
        
        if (col.name.Contains("Gold")) {
            oreName = "Gold";
        } else if (col.name.Contains("Silver")) {
            oreName = "Silver";
        } else if (col.name.Contains("Coal")) {
            oreName = "Coal";
        }

        OreManager.IncrementOre(oreName);

        Destroy(col.gameObject);
    }
}
