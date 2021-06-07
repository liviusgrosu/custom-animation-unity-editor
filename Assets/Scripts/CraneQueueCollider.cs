using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneQueueCollider : MonoBehaviour
{
    public TargetFinder TargetFinder;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Ore") {
            // Add the ore to the pickup queue if it enters the pick up zone
            TargetFinder.ProvideNewTarget(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ore") {
            // Remove the ore if it leaves the pick up zone
            TargetFinder.RemoveTarget(other.transform);
        }
    }
}
