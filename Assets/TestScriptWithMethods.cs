using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptWithMethods : MonoBehaviour
{
    public void functionA() 
    {
        Debug.Log("function A Invoked");
    }
    public void functionB(int val) 
    {
        Debug.Log("function B Invoked");
    }
    public void functionC() 
    {
        Debug.Log("function C Invoked");
    }
}
