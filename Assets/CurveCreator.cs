using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CurveCollection;

[ExecuteInEditMode]
public class CurveCreator : MonoBehaviour {
    [HideInInspector]
    public List<Curve> curves = new List<Curve>();
    [HideInInspector]
    public List<string> railNames = new List<string>();
    [HideInInspector]
    public List<AnimationCurve> animationCurves = new List<AnimationCurve>();
    [HideInInspector]
    public List<string> startStationNames = new List<string>();
    [HideInInspector]
    public List<string> endStationNames = new List<string>();
    [HideInInspector]
    public List<UnityEvent> startTriggerObjs  = new List<UnityEvent>();
    [HideInInspector]
    public List<UnityEvent> endTriggerObjs  = new List<UnityEvent>();
    [HideInInspector]
    public List<float> startDelays = new List<float>();
    [HideInInspector]
    public List<float> endDelays = new List<float>();

    //-------
    [HideInInspector]
    public List<bool> showAnimationRail = new List<bool>();

    public void InvokeStartObject(int curveIdx) {
        startTriggerObjs[curveIdx].Invoke();
    }

    public void InvokeEndObjects(int curveIdx) {
        endTriggerObjs[curveIdx].Invoke();
    }

    public int GetRailIdx(string railName) {
        // Check to see if it belongs in any of the terminal stations
        int startStationIdx = startStationNames.IndexOf(railName);
        int endStationIdx = endStationNames.IndexOf(railName);

        if (startStationIdx != -1) {
            return startStationIdx;
        }
        if (endStationIdx != -1) {
            return endStationIdx;
        }
        return -1;
        //return railNames.FindIndex(a => a.Contains(railName));
    }
}
