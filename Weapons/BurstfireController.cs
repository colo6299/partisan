using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstfireController : FireController
{
    public int roundsInBurst = 3;
    public float interBurstDelay = 2;
 
    public override bool TryFire()
    {
        return base.TryFire();
    }
}
