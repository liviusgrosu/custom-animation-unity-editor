using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawCollider : MonoBehaviour
{
    private Animator _animator;

    private void Awake() {
        _animator = transform.parent.GetComponent<Animator>();
    }

    private void Update() {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) {
            foreach(OreParentClamp oreScript in FindObjectsOfType(typeof(OreParentClamp))) {
                // Take all the ores clamped to the bucket and unassign their parent
                oreScript.gameObject.transform.parent = null;
            }
        }
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
