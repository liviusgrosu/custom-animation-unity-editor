using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorBarrierMovement : MonoBehaviour
{
    public Transform StartPosition, EndPosition;
    public float MovementDuration = 0.5f;
    private float _timeElapsed;

    void Update() {
        if (_timeElapsed < MovementDuration) {
            _timeElapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(StartPosition.position, EndPosition.position, _timeElapsed / MovementDuration);
        }
    }

    public void Reset() {
        _timeElapsed = 0f;
        transform.position = StartPosition.position;
    }
}
