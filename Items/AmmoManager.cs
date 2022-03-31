using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    private int[] munitionArray = new int[16];
    // ok so ammo types...
    // 0: standard (common-grey)
    // 1: stable   (common-blue) - slightly higher range, slightly lower radius 
    // 2: unstable (common-green) - slightly lower range, slightly higher radius 
    // 3: chaff    (uncommon-brown) - slower, but makes more smoke and appears as a player on enemy radar
    // 4: stealth  (uncommon-black) - makes much less smoke and is less visible to missile-warning systems
    // 5: metastable (rare-purple) - large kill radius, long arm time
    // 6: overload (rare-gold) - much longer range, slightly larger radius 

    public void RecieveMunitionUpdate(string tcpInventoryUpdate)
    {
        
    }

    public void RequestMunitionUpdate()
    {

    }

}
