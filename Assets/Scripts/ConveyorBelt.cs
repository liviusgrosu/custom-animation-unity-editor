using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    private void OnCollisionStay(Collision other) {        
        if(other.transform.tag == "Ore") {
            Vector3 movement = -transform.up * speed * Time.deltaTime;
            other.transform.GetComponent<Rigidbody>().position -= movement;
            other.transform.GetComponent<Rigidbody>().MovePosition(other.transform.position + movement);
        }
    }
}
