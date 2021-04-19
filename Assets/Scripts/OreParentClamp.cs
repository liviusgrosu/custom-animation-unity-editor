using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreParentClamp : MonoBehaviour
{
    private Transform _parent;

    public void AssignParentToClampTo(Transform parent) {
        
        // Assign parent to clamp too
        _parent = parent;
        transform.parent = parent;
    }

    private void OnCollisionStay(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        // Check if its an ore and doesnt have the clamp script
        if (collisionObject.tag == "Ore" && collisionObject.GetComponent<OreParentClamp>() == null) {
            collisionObject.AddComponent<OreParentClamp>();
            collisionObject.GetComponent<OreParentClamp>().AssignParentToClampTo(transform.parent);
        }
    }
}
