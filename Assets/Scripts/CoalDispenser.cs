using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalDispenser : MonoBehaviour, IStation {
    private Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    public void DispenseOre(float secondsToWait) {
        StartCoroutine(HoldClawOpen(secondsToWait));
    }

    IEnumerator HoldClawOpen(float secondsToWait) {
        // Open and close the claw
        _animator.SetTrigger("Open Hatch");
        yield return new WaitForSeconds(secondsToWait);
        _animator.SetTrigger("Close Hatch");
    }
}
