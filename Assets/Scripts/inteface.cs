using UnityEngine;
using System.Collections;

public interface IDispensable {
    void DispenseOre(float secondsToWait);
}

public interface IToggleMachine {
    void SetPowerState(bool isOff);
}