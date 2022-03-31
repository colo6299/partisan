using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMissileInit : MonoBehaviour
{
    public PuppetMissile missile;
    public string missileReport;

    private void Start()
    {
        missile.InvariantMissileInitialize(missileReport);
    }
}
