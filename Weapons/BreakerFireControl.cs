using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerFireControl : FireController
{

    protected override void OnAwake()
    {
        base.OnAwake();
        projectileAssociator[0] = 1;
    }
}
