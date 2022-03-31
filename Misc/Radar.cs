using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public float range = 300;
    public float segments = 5;
    public float radarSize = 0.47f;

    public Transform puppetParent;
    public Transform missileParent;
    public Transform player;
    public Transform radarPlate;
    public GameObject radarDotPrefab;
    public GameObject missileDotPrefab;



    void Update()
    {
        UpdateRadar(puppetParent, radarDotPrefab, false);
        UpdateRadar(missileParent, missileDotPrefab, true);
    }

    void UpdateRadar(Transform parentObject, GameObject dotPrefab, bool rotate = false)
    {
        for (int i=0; i<parentObject.childCount; i++)
        {
            Transform puppet = parentObject.GetChild(i);
            if (puppet.parent != null)
            {
                if ((puppet.position - player.position).sqrMagnitude <= (range * range))
                {
                    Vector3 radarLocalPos = RadarLinearLocalizer(puppet);
                    GameObject dot = Instantiate(dotPrefab, radarPlate);
                    dot.transform.localPosition = radarLocalPos * radarSize;
                    if (rotate)
                    {
                        dot.transform.localRotation = puppet.rotation;
                    }
                }
            }
        }
    }

    private Vector3 RadarLinearLocalizer(Transform puppet)
    {
        Vector3 radarPos = puppet.position - player.position;
        return radarPos /= range;
    }

    /// <summary>
    /// Returns the local position of the dot spawn relative to the radar compass plate.
    /// </summary>
    /// <param name="puppet"></param>
    /// <returns></returns>
    private Vector3 RadarLogLocalizer(Transform puppet)
    {
        Vector3 radarPos = puppet.position - player.position;
        float distance = radarPos.magnitude / range;
         //magnitude 0-1 of max range
        distance = 1 - Mathf.Pow(2, (-distance * segments));
        radarPos.Normalize();
        radarPos *= distance;
        return radarPos;
    }
}
