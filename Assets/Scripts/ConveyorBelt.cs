using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    public bool _movementTurnedOff, _powerTurnedOff;

    private Transform[] axels;

    private void Start() {
        axels = transform.parent.GetComponentsInChildren<Transform>().Where(t => t.name == "Axel").ToArray();
        if (_movementTurnedOff) {
            // We do this so that we can stop the axels as well
            SetMovementState(_movementTurnedOff);
        }
    }

    private void OnCollisionStay(Collision other) {        
        if(other.transform.tag == "Ore" && !_movementTurnedOff) {
            Vector3 movement = -transform.up * speed * Time.deltaTime;
            other.transform.GetComponent<Rigidbody>().position -= movement;
            other.transform.GetComponent<Rigidbody>().MovePosition(other.transform.position + movement);
        }
    }

    public void SetMovementState(bool isStopping) {
        _movementTurnedOff = isStopping;

        foreach(Transform axel in axels) {
            axel.GetComponent<Spinning>().SetState(_movementTurnedOff);
        }
    }

    public void SetPowerState(bool isOff) {
        _powerTurnedOff = isOff;
        if (!_powerTurnedOff) {
            // Only stop movement when turned off
            // Otherwise turning them on would also move them
            SetMovementState(!_powerTurnedOff);
        }
    }
}
