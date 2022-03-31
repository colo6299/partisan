using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerDataViewer : MonoBehaviour
{

    private static Vector3[] vecs;
    public GameObject identifier;

    private void Start()
    {
        vecs = new Vector3[0];
    }
    private void OnaDrawGizmos()
    {
        foreach (Vector3 vector in vecs)
        {
            if (true) { continue; }
            //Gizmos.color = Color.magenta;
            //Gizmos.DrawSphere(vector, 4);
        }
    }

    private void Update()
    {
        foreach (Vector3 vector in vecs)
        {
            if (vector == null) { continue; }
            Instantiate(identifier, vector, Quaternion.identity, null);
        }
    }

    public static void FeedVectors(Vector3[] vectors)
    {
        vecs = vectors;
    }
}
