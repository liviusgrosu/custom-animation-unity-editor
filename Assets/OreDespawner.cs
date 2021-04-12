using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        if (col.tag == "Ore") {
            Destroy(col.gameObject);
        }
    }
}
