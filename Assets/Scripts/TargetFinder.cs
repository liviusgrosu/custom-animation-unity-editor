   using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetFinder : MonoBehaviour
{
    public Transform IKLeg;
    public float LerpDuration = 0.5f;
    private Vector3 _currentTarget, _oldTarget;
    private float _timeElapsed;
    private CraneArmIK _IKLegScript;
    private bool _calculateMovement;

    public Transform MidPoint;
    public Transform EndPoint;
    private Queue<Vector3> _queuedTargets;

    public float speed;

    void Awake() {
        _queuedTargets = new Queue<Vector3>();
    }

    void Start() {
        _IKLegScript = IKLeg.GetComponent<CraneArmIK>();
        _IKLegScript.ProvideNewPosition(MidPoint.position);
    }

    /// <remarks>
    /// Using late update so that velocity clamp calculates first
    /// </remarks>
    void Update() {
        // TODO: Might want to consider switching _currentTarget to a Transform
        if (_calculateMovement) {
            Vector3 lerpPos = Vector3.Lerp(_oldTarget, _currentTarget, _timeElapsed / LerpDuration);
            _IKLegScript.ProvideNewPosition(lerpPos);
            _timeElapsed += Time.deltaTime * speed;
            if (_timeElapsed > LerpDuration) {
                // Find new target
                _calculateMovement = false;
                return;
            }
        }
        else {
            if (_queuedTargets.Count > 0) {
                _calculateMovement = true;
                _timeElapsed = 0f;
                // Find new target
                StartNextTarget();
            }
        }
    }
    public void ProvideNewTarget(Vector3 target) {
        // Setup the next targets
        _queuedTargets.Enqueue(target);
        _queuedTargets.Enqueue(MidPoint.position);
        _queuedTargets.Enqueue(EndPoint.position);
        _queuedTargets.Enqueue(MidPoint.position);
    }

    private void StartNextTarget() {
        _currentTarget = _queuedTargets.Dequeue();
        _oldTarget = IKLeg.position;
    }
}
