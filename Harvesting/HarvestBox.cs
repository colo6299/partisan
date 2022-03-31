using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestBox : MonoBehaviour
{

    private List<ScanDot> contactDots;
    private void FixedUpdate()
    {
        contactDots = new List<ScanDot>();
    }

    private void OnTriggerStay(Collider other)
    {
        contactDots.Add(other.GetComponentInParent<ScanDot>());
    }

    public ScanDot[] GetDots()
    {
        ScanDot[] dots = new ScanDot[contactDots.Count];
        int counter = 0;
        foreach(ScanDot dot in contactDots)
        {
            dots[counter] = dot;
            counter++;
        }
        return dots;
    }
}
