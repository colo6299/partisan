using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestNode : MonoBehaviour
{
    private int extractionsRemaining = 2;
    private float nodeQuality = 1;
    private string uID;
    private Queue<int> sweeps;


    private void Awake()
    {
        sweeps = new Queue<int>();
        for (int i = 0; i<100; i++)
        {
            int id = (int)Mathf.Floor(Random.Range(0, 7));
            if (id == 7) { id = 6; }
            sweeps.Enqueue(id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetItem()
    {
        int nextItem = -1;
        if (sweeps.Count > 0) { nextItem = sweeps.Dequeue(); }
        else { EmptyToPlayer(); }

        return nextItem;
    }

    private void EmptyToPlayer()
    {

    }

    public string GetUID()
    {
        return uID;
    }

    public void BuidNode(string nodeData)
    {

    }

    public void AddSweeps(string sweepsData)
    {

    }
}
