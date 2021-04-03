using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class CurveCreator : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();
    [HideInInspector]
    public AnimationCurve curve;
    [HideInInspector]
    public UnityEvent startTriggerObj, endTriggerObj;
    [HideInInspector]
    public float startDelay, endDelay;
}
