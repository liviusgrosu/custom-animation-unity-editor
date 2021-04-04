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
    public List<string> railName = new List<string>();
    [HideInInspector]
    public List<AnimationCurve> animationCurve = new List<AnimationCurve>();
    [HideInInspector]
    public List<UnityEvent> startTriggerObj  = new List<UnityEvent>();
    [HideInInspector]
    public List<UnityEvent> endTriggerObj  = new List<UnityEvent>();
    [HideInInspector]
    public List<float> startDelay = new List<float>();
    [HideInInspector]
    public List<float> endDelay = new List<float>();

    //-------
    [HideInInspector]
    public List<bool> showAnimationRail = new List<bool>();
}
