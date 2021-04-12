using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningClaw : MonoBehaviour
{
    [Header("Rail")]
    public CurveCreator RailEditor;
    public string RailName;
    public bool LoopRail;
    [Space(5)]
    [Header("Movement")]
    public float MovementSpeed = 1f;

    private int _currentRailIdx = -1;
    private int _currentTravelPoint;
    private int _railDirection = 1; // 1: forwards, -1: backwards
    private float _distanceTolerance;
    private bool _cartIsPausing;

    private void Start() {
        _currentRailIdx = RailEditor.GetRailIdx(RailName);

        if(_currentRailIdx < 0) {
            Debug.LogError("Can't find rail index given name");
        }

        // Start at the first point
        transform.position = RailEditor.curves[_currentRailIdx].points[0];
        _currentTravelPoint = 1;
    }

    private void Update()
    {
        if (_cartIsPausing) {
            return;
        }
        if (Vector3.Distance(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPoint]) > _distanceTolerance) { 
            float step =  MovementSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPoint], step);
        }
        else {
            
            if (_currentTravelPoint == 0) {
                RailEditor.InvokeStartObject(_currentRailIdx);
                StartCoroutine(PauseCartForDelay(RailEditor.startDelays[_currentRailIdx]));
                // Toggle direction
                _railDirection *= -1;
            }
            else if (_currentTravelPoint == RailEditor.curves[_currentRailIdx].points.Count - 1) {
                RailEditor.InvokeEndObjects(_currentRailIdx);
                StartCoroutine(PauseCartForDelay(RailEditor.endDelays[_currentRailIdx]));
                ToggleRailDirection();
                // Toggle direction
            }
            // Step over to next point on rail
            _currentTravelPoint += _railDirection;
        }
    }

    private void ToggleRailDirection() {
        // Change the direction of the cart
        _railDirection *= -1;
    }

    IEnumerator PauseCartForDelay(float pauseTime) {
        // Pause the cart for a set amount of time
        _cartIsPausing = true;
        yield return new WaitForSeconds(pauseTime);
        _cartIsPausing = false;
    }
}
