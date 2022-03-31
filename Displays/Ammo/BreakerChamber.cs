using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerChamber : Chamber
{
    protected override void OnAwake()
    {
        base.OnAwake();
        magDisplayAssociator[0] = 25;
    }
}
