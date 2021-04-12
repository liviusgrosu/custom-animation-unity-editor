using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningClawAnimation : MonoBehaviour {
    private Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void DropOre(float secondsToWait) {
        StartCoroutine(HoldClawOpen(secondsToWait));
    }

    IEnumerator HoldClawOpen(float secondsToWait) {
        // Open and close the claw
        _animator.SetTrigger("Open Claw");
        yield return new WaitForSeconds(secondsToWait);
        _animator.SetTrigger("Close Claw");
    }
}
