﻿using System.Collections;
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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _tempState = !_tempState;
            ToggleState(_tempState);
        }
    }

    public void ToggleState(bool state) {
        _animator.speed = state ? _animationSpeed : 0f;
    }

    public void SetPowerState(bool state) {
        ToggleState(state);
    }
}
