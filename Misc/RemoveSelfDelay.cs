using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSelfDelay : MonoBehaviour
{
    public float delay;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }
    void Update()
    {
        if (Time.time - startTime > delay)
        {
            Destroy(gameObject);
        }
    }
}
