using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : MonoBehaviour
{

    public int ammoType = 0;
    public float reloadDelay;

    public bool Carried()
    {
        return false;
    }

    public void FireAnimate()
    {
        
    }

    public void ReloadAnimate()
    {
        Destroy(gameObject, 0.2f);
    }

    public float ReloadAnimateDelay()
    {
        return reloadDelay;
    }
}
