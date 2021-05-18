using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningClaw : MonoBehaviour {
    [Header("Rail")]
    public CurveCreator RailEditor;
    private string _currentRailName;
    private int _currentRailIdx;
    private Transform _currentTarget;
    private RailManager _railManager;
     // --- Movement Variables ---
    [Space(5)]
    [Header("Movement")]
    public float MovementSpeed = 1f;
    private float _distanceTolerance = 0.01f;
    private int _currentTravelPointIdx;
    private bool _cartIsPausing;

    // --- Switching Rails Variables ---
    private int _currentIntermediatePointIdx;
    private Vector3 _currentIntermediatePointPos;
    private enum Mode {
        working,
        traveling
    }

    private enum Direction : short {
        forwards = 1, 
        backwards = -1
    }
    private Mode _currentCartMode;
    private Direction _currentDirection;

    private void Start() {
        _railManager = GameObject.Find("Rail Manager").GetComponent<RailManager>();
        
        _currentRailIdx = 0;
        _currentRailName = RailEditor.railNames[_currentRailIdx];
        _currentIntermediatePointIdx = RailEditor.curves[_currentRailIdx].intermediatePointIdx;
        _currentIntermediatePointPos = RailEditor.curves[_currentRailIdx].points[_currentIntermediatePointIdx];

        // Start traveling to the intermediate point
        _currentCartMode = Mode.traveling;
        _currentDirection = Direction.forwards;
    }

    private void Update() {
        if (_currentCartMode == Mode.traveling) {
            if (Vector3.Distance(transform.position, _currentIntermediatePointPos) > _distanceTolerance) {
                // Move towards the intermediate point 
                float step =  MovementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _currentIntermediatePointPos, step);
            }
            else {
                // Go to working stage
                _currentCartMode = Mode.working;
                // Assign the next point to go to
                _currentTravelPointIdx = Mathf.Clamp(_currentIntermediatePointIdx + 1, 0, RailEditor.curves[_currentRailIdx].points.Count - 1);
            }
        }
        else if(_currentCartMode == Mode.working) {
            //Debug.Log(_currentTravelPointIdx);
            if (Vector3.Distance(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPointIdx]) > _distanceTolerance) { 
                // Go to the next point if not close
                float step =  MovementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPointIdx], step);
            }
            else {
                if (_currentTravelPointIdx == 0) {
                    // Start trigger and delay
                    //RailEditor.InvokeStartObject(_currentRailIdx);
                    // Wait for the delay
                    //StartCoroutine(PauseCartForDelay(RailEditor.startDelays[_currentRailIdx]));
                    // Toggle direction
                    ToggleRailDirection();
                }
                else if (_currentTravelPointIdx == RailEditor.curves[_currentRailIdx].points.Count - 1) {
                    // End trigger and delay
                    //RailEditor.InvokeEndObjects(_currentRailIdx);
                    // Wait for the delay
                    //StartCoroutine(PauseCartForDelay(RailEditor.endDelays[_currentRailIdx]));
                    // Toggle direction
                    ToggleRailDirection();
                }
                // Step over to next point on rail
                _currentTravelPointIdx += (int)_currentDirection;
            }
        }
    }

    private void ToggleRailDirection() {
        // Change the direction of the cart
        if (_currentDirection == Direction.forwards) {
            _currentDirection = Direction.backwards;
        }
        else {
            _currentDirection = Direction.forwards;
        }
    }

    public void SwitchRails(string railName, int direction) {

        _currentRailName = railName;
        _currentRailIdx = RailEditor.GetRailIdx(_currentRailName);
        _currentIntermediatePointIdx = RailEditor.curves[_currentRailIdx].intermediatePointIdx;
        _currentIntermediatePointPos = RailEditor.curves[_currentRailIdx].points[_currentIntermediatePointIdx];

        _currentCartMode = Mode.traveling;
        if(direction == (int)Direction.forwards) {
            _currentDirection = Direction.forwards;
        }
        else {
            _currentDirection = Direction.backwards;
        }
    }

    IEnumerator PauseCartForDelay(float pauseTime) {
        // Pause the cart for a set amount of time
        _cartIsPausing = true;
        yield return new WaitForSeconds(pauseTime);
        _cartIsPausing = false;
    }
}
