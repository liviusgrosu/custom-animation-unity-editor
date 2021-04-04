using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CurveCollection;

[ExecuteInEditMode]
public class CurveCreator : MonoBehaviour
{
    [HideInInspector]
    public List<Curve> curves = new List<Curve>();
    [HideInInspector]
    public AnimationCurve curve;
    [HideInInspector]
    public UnityEvent startTriggerObj, endTriggerObj;
    [HideInInspector]
    public float startDelay, endDelay;
}
