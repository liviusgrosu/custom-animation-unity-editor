using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneQueueCollider : MonoBehaviour
{
    public TargetFinder TargetFinder;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Ore") {
            TargetFinder.ProvideNewTarget(other.transform.position);
        }
    }
}
