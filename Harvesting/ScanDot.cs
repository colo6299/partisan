using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanDot : MonoBehaviour
{

    public int type;

    public int DotType()
    {
        return type;
    }

    public void RemoveDot()
    {
        Destroy(gameObject);
    }
}
