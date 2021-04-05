using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    public CurveCreator RailEditor;
    public string RailName;
    public float MovementSpeed = 1f;

    private int _currentRailIdx = -1;
    private int _currentTravelPoint;
    private float _distanceTolerance;

    private void Start() {
        _currentRailIdx = RailEditor.GetRailIdx(RailName);

        if(_currentRailIdx < 0) {
            Debug.LogError("Can't find rail index given name");
        }

        _currentTravelPoint = 0;
        transform.position = RailEditor.curves[0].points[_currentTravelPoint];
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPoint]) > _distanceTolerance) { 
            float step =  MovementSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPoint], step);
        }
        else {

            if (_currentTravelPoint == 0) {
                RailEditor.InvokeStartObject(_currentRailIdx);
            }
            else if (_currentTravelPoint == RailEditor.curves[_currentRailIdx].points.Count - 1) {
                RailEditor.InvokeEndObjects(_currentRailIdx);
            }

            // TODO: check index of path
            // TODO: invoke start and end trigger
            // TODO: trigger delay functions

            _currentTravelPoint = _currentTravelPoint + 1;
            if (_currentTravelPoint >= RailEditor.curves[_currentRailIdx].points.Count) {
                _currentTravelPoint = 0;
            }
        }
    }
}
