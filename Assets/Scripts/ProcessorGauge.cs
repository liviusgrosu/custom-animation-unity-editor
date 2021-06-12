using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorGauge : MonoBehaviour
{
    private Quaternion _oldTargetRotation, _currentTargetRotation;
    private Quaternion _minTargetRotation, _maxTargetRotation;
    public float LerpDuration;
    private float _timeElapsed;
    public float Speed;

    private void Start() {
        _minTargetRotation =_oldTargetRotation = _currentTargetRotation = transform.rotation;
        // TODO: figure out how to rotate this locally and not globally
        _maxTargetRotation = _minTargetRotation * Quaternion.Euler(180f, 0, 0);
    }

    private void Update() {
        if(_timeElapsed < LerpDuration) {
            transform.rotation = Quaternion.Lerp(_oldTargetRotation, _currentTargetRotation, _timeElapsed / LerpDuration);
            _timeElapsed += Time.deltaTime * Speed;
        }
    }

    public void UpdateGaugeLevel(float percentage) {
        _oldTargetRotation = transform.rotation;
        _currentTargetRotation = Quaternion.Lerp(_minTargetRotation, _maxTargetRotation, percentage);
        _timeElapsed = 0f;
    }
}
