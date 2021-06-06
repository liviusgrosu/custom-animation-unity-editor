using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CranePickupPoint : MonoBehaviour {
    public TargetFinder CraneTargetFinder;
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Ore") {
            CraneTargetFinder.ProvideNewTarget(other.transform.position);
        }
    }
}
