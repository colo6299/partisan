using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkSelfDestroy : MonoBehaviour
{
    public float max;
    public float min;
    public float duration;
    private Vector3 scale;
    private float startTime;

    private void Awake()
    {
        
    }

    private void Start()
    {
        scale = transform.localScale;
        startTime = Time.time;
    }

    private void Update()
    {
        transform.localScale = scale * Mathf.Lerp(max, min, (Time.time - startTime) / duration);
        if ((Time.time-startTime) > duration) { Destroy(gameObject); }
    }


}
