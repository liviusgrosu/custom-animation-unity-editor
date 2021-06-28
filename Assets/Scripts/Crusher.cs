using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour, IToggleMachine {
    private Animator _animator;
    private float _animationSpeed;

    //Temp 
    private bool _tempState;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _animationSpeed = _animator.speed;
    }

    public void ToggleState(bool state) {
        _animator.speed = state ? _animationSpeed : 0f;
    }

    public void SetPowerState(bool state) {
        ToggleState(state);
    }

    private void OnTriggerEnter(Collider colliderObj) {
        if (colliderObj.tag == "Ore") {
            colliderObj.GetComponent<OrePhysicalState>().CrushOre();
        }
    }
}
