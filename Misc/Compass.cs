using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Inverse(player.rotation);
    }
}
