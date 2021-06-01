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

    private struct Direction {
        public Direction(int forwards, int backwards)
        {
            forwardIncrement = forwards;
            backwardIncrement = backwards;
            currentDirection = -1;
        }
        public int forwardIncrement;
        public int backwardIncrement;
        public int currentDirection;
    }

    private Mode _currentCartMode;
    private Direction _direction;

    private void Start() {
        SwitchRails("Mine", "Mine to Processor");
    }

    private void Update() {
        if (_cartIsPausing) {
            return;
        }
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
                _currentTravelPointIdx = Mathf.Clamp(_currentIntermediatePointIdx + _direction.currentDirection, 0, RailEditor.curves[_currentRailIdx].Points.Count - 1);
            }
        }
        else if(_currentCartMode == Mode.working) {
            //Debug.Log(_currentTravelPointIdx);
            if (Vector3.Distance(transform.position, RailEditor.curves[_currentRailIdx].Points[_currentTravelPointIdx]) > _distanceTolerance) { 
                // Go to the next point if not close
                float step =  MovementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, RailEditor.curves[_currentRailIdx].Points[_currentTravelPointIdx], step);
            }
            else {
                if (_currentTravelPointIdx == 0) {
                    // Start trigger and delay
                    RailEditor.InvokeStartObject(_currentRailIdx);
                    // Wait for the delay
                    StartCoroutine(PauseCartForDelay(RailEditor.startDelays[_currentRailIdx]));
                    // Toggle direction
                    ToggleRailDirection();
                }
                else if (_currentTravelPointIdx == RailEditor.curves[_currentRailIdx].Points.Count - 1) {
                    // End trigger and delay
                    RailEditor.InvokeEndObjects(_currentRailIdx);
                    // Wait for the delay
                    StartCoroutine(PauseCartForDelay(RailEditor.endDelays[_currentRailIdx]));
                    // Toggle direction
                    ToggleRailDirection();
                }
                // Step over to next point on rail
                _currentTravelPointIdx += _direction.currentDirection;   
            }
        }
    }

    private void ToggleRailDirection() {
        // Change the direction of the cart
        if (_direction.currentDirection == _direction.forwardIncrement) {
            _direction.currentDirection = _direction.backwardIncrement;
        }
        else {
            _direction.currentDirection = _direction.forwardIncrement;
        }
    }

    public void SwitchRails(string stationName, string railName) {
        // Store the rails points
        _currentRailName = railName;
        _currentRailIdx = RailEditor.GetRailIdx(_currentRailName);
        _currentIntermediatePointIdx = RailEditor.curves[_currentRailIdx].IntermediatePointIdx;
        _currentIntermediatePointPos = RailEditor.curves[_currentRailIdx].Points[_currentIntermediatePointIdx];

        // Store the directions increment
        _direction = new Direction(RailEditor.curves[_currentRailIdx].ForwardDirectionIncrement, 
                                    RailEditor.curves[_currentRailIdx].BackwardDirectionIncrement);

        // Assign direction
        if (stationName == RailEditor.startTriggerObjs[_currentRailIdx].GetPersistentTarget(0).name) {
            _direction.currentDirection = _direction.forwardIncrement;
        }
        else if (stationName == RailEditor.endTriggerObjs[_currentRailIdx].GetPersistentTarget(0).name) {
            _direction.currentDirection = _direction.backwardIncrement;
        }
        _currentCartMode = Mode.traveling;
    }

    IEnumerator PauseCartForDelay(float pauseTime) {
        // Pause the cart for a set amount of time
        _cartIsPausing = true;
        yield return new WaitForSeconds(pauseTime);
        _cartIsPausing = false;
    }
}
