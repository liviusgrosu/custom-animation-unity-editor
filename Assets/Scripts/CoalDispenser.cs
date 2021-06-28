using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalDispenser : MonoBehaviour, IDispensable {
    private BoxCollider HatchCollider;
    private void Awake() {
        HatchCollider = GetComponent<BoxCollider>();
    }
    public void DispenseOre(float secondsToWait) {
        StartCoroutine(HoldClawOpen(secondsToWait));
    }

    IEnumerator HoldClawOpen(float secondsToWait) {
        // Open and close the claw
        HatchCollider.enabled = false;
        yield return new WaitForSeconds(secondsToWait);
        HatchCollider.enabled = true;
    }
}
