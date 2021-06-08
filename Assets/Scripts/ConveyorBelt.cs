using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    public bool _stopped;

    private Transform[] axels;

    private void Start() {
        axels = transform.parent.GetComponentsInChildren<Transform>().Where(t => t.name == "Axel").ToArray();
        if (_stopped) {
            // We do this so that we can stop the axels as well
            SetState(_stopped);
        }
    }

    private void OnCollisionStay(Collision other) {        
        if(other.transform.tag == "Ore" && !_stopped) {
            Vector3 movement = -transform.up * speed * Time.deltaTime;
            other.transform.GetComponent<Rigidbody>().position -= movement;
            other.transform.GetComponent<Rigidbody>().MovePosition(other.transform.position + movement);
        }
    }

    public void SetState(bool isStopping) {
        _stopped = isStopping;

        foreach(Transform axel in axels) {
            axel.GetComponent<Spinning>().SetState(_stopped);
        }
    } 
}
