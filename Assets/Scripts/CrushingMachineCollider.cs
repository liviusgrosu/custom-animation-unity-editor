using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushingMachineCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider colliderObj) {
        if (colliderObj.tag == "Ore") {
            colliderObj.GetComponent<OrePhysicalState>().CrushOre();
        }
    }
}