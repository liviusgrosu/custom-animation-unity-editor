using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurveCollection {
    [System.Serializable]
    public class Curve {
        public List<Vector3> Points = new List<Vector3>();
        public int IntermediatePointIdx;
        public int FirstStationIdx, SecondStationIdx;
        public int ForwardDirectionIncrement, BackwardDirectionIncrement;
    }
}