using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetPartisan : MonoBehaviour
{
    //These are TCP values, set once at puppet init
    public string displayName = "NPC";
    public float size;
    public bool rotateDirection;
    private string[] UdpData;
    private Vector3 oldPos;
    private Vector3 velocityInferred;
    private float timeSnapshot;
    private float lastUpdate;

    //These are UDP values
    public bool boosting; //visual indication of boosting
    //movement and rotation UDP controlled


    private void Update()
    {
        Vector3 netPosition = new Vector3(float.Parse(UdpData[1]), 0, float.Parse(UdpData[2]));
        

        if (netPosition != oldPos)
        {
            lastUpdate = Time.time;
            if (true)
            {
                velocityInferred = (netPosition - oldPos) / (Time.time - timeSnapshot);
                timeSnapshot = Time.time;
                oldPos = netPosition;
                transform.position = netPosition;
            }
        }
        else
        {
            if ((Time.time - lastUpdate < 1) & (timeSnapshot != 0)) { return; }
            transform.position += velocityInferred * Time.deltaTime;
        }


        transform.rotation = Quaternion.Euler(0, float.Parse(UdpData[3]), 0);
    }

    //TCP commanded
    public void PuppetFire(/*insert puppet blast data*/)
    {
        //visuals of firing as commanded by server
        //additionally fires and links puppet blast.
    }

    public void PuppetUdpStream(string dataString)
    {
        UdpData = dataString.Split(' ');
    }

}
