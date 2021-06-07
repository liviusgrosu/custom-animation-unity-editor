   using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetFinder : MonoBehaviour
{
    public Transform IKLeg;
    public float LerpDuration = 0.5f;
    /*  
        We use different types here because current target is translation sensitive
        while old target isn't
    */
    private Transform _currentTarget;
    private Vector3 _oldTarget;
    private float _timeElapsed;
    private CraneArmIK _IKLegScript;
    private bool _calculateMovement;

    public Transform MidPoint;
    public Transform EndPoint;
    private List<Transform> _queuedTargets;

    public float craneMovementSpeed;
    private Transform _oreGrabbedOnto;

    void Awake() {
        _queuedTargets = new List<Transform>();
    }

    void Start() {
        _IKLegScript = IKLeg.GetComponent<CraneArmIK>();
        _IKLegScript.ProvideNewPosition(MidPoint.position);
    }

    void Update() {
        if (_calculateMovement) {
            // Move towards the next target
            Vector3 lerpPos = Vector3.Lerp(_oldTarget, _currentTarget.position, _timeElapsed / LerpDuration);
            _IKLegScript.ProvideNewPosition(lerpPos);
            _timeElapsed += Time.deltaTime * craneMovementSpeed;
            if (_timeElapsed > LerpDuration) {
                if (_currentTarget.gameObject.tag == "Ore") {
                    // Pick up an ore
                    _oreGrabbedOnto = _currentTarget;
                    _oreGrabbedOnto.parent = IKLeg.transform;
                    _oreGrabbedOnto.GetComponent<Rigidbody>().isKinematic = true;
                }
                else if (_currentTarget == EndPoint) {
                    // Drop off the ore
                    _oreGrabbedOnto.GetComponent<Rigidbody>().isKinematic = false;
                    _oreGrabbedOnto.parent = null;
                    _oreGrabbedOnto = null;
                }
                _calculateMovement = false;
                return;
            }
        }
        else {
            if (_queuedTargets.Count > 0) {
                _calculateMovement = true;
                _timeElapsed = 0f;
                // Start processing the next target point
                _currentTarget = _queuedTargets[0];
                _queuedTargets.RemoveAt(0);
                _oldTarget = IKLeg.position;
            }
        }
    }
    public void ProvideNewTarget(Transform target) {
        // Check if its already being calculated in the translation
        if (_oreGrabbedOnto == target || 
            _currentTarget == target ||
            _queuedTargets.Any(obj => obj.GetInstanceID() == target.GetInstanceID())) {
            // Need to do this because coal will trigger on exit too
            return;
        }

        // Setup the next pathing targets
        _queuedTargets.Add(target);
        _queuedTargets.Add(MidPoint);
        _queuedTargets.Add(EndPoint);
        _queuedTargets.Add(MidPoint);
    }

    public void RemoveTarget(Transform target) {
        if (_oreGrabbedOnto == target || _currentTarget == target) {
            // Don't remove the ore if its already being translated
            return;    
        }

        int targetIdx = _queuedTargets.IndexOf(target);
        for (int i = 0; i < 4; i++) {
            // Remove the target and its following path points
            _queuedTargets.RemoveAt(targetIdx);
        }
    }
}
