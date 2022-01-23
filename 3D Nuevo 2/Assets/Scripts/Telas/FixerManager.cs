using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixerManager : MonoBehaviour
{
    public Fixer[] fixersFromManager;


    //store all fixers in children
    private void Awake() {
        fixersFromManager = GetComponentsInChildren<Fixer>();
    }
}
