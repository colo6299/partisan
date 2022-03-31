using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetMissile : MonoBehaviour
{
    private string uID;
    private string team;
    private string type;
    private float fireTimeSeconds;
    private Vector3 startPos;
    private Vector3 startVelocity;
    private bool detonateFlag;
    private bool removeFlag;
    private float autoRemoveTime = 20;

    public GameObject explodeVisualsPrefab;

    private void Update()
    {
        CalculatePosition();
        CalculateRotation();
        CalculateDestroy();
    }

    private void CalculateDestroy()
    {
        if (detonateFlag) { Detonate(); }
        else if (removeFlag) { Remove(); }
        if (Time.time - fireTimeSeconds > autoRemoveTime)
        {
            Remove();
        }
    }

    private void CalculatePosition()
    {
        float snapshotDeltaTime = Time.realtimeSinceStartup - fireTimeSeconds;
        transform.position = (startVelocity * snapshotDeltaTime) + startPos;
    }

    private void CalculateRotation()
    {
        Quaternion rot = transform.rotation;
        rot.SetLookRotation(startVelocity.normalized, Vector3.up);
        rot = Quaternion.Slerp(transform.rotation, rot, 0.2f*Time.deltaTime);
        transform.rotation = rot;
    }

    /// <summary>
    /// 0  ID 
    /// 1  team
    /// 2  type
    /// 3  number of miliseconds firetime is in the past 
    /// 4  fireCoordinate x  (y=0)
    /// 5  fireCoordinate z
    /// 6  fireVecAzimuth x  (y=0)
    /// 7  fireVecAzimuth z
    /// </summary>
    /// <param name="missileReport"></param>
    public void InvariantMissileInitialize(string missileReport, bool netType=false)
    {

        string[] data = missileReport.Split(' ');
        uID = data[0];
        team = data[1];
        type = data[2];
        if (!netType) { fireTimeSeconds = (Time.realtimeSinceStartup); }
        else 
        { 
            fireTimeSeconds = ((float)StreamWorldWatcher.GetNetOffset() + Time.realtimeSinceStartup) - float.Parse(data[3]);
        }

        startPos = new Vector3(float.Parse(data[4]), 0, float.Parse(data[5]));
        startVelocity = new Vector3(float.Parse(data[6]), 0, float.Parse(data[7]));

        CalculateRotation();

    }

    public string GetUID()
    {
        return uID;
    }

    public void FlagDetonate()
    {
        detonateFlag = true;
    }

    public void FlagRemove()
    {
        removeFlag = true;
    }

    private void Detonate()
    {
        Instantiate(explodeVisualsPrefab, transform.position, transform.rotation, null);
        Destroy(gameObject);
    }

    private void Remove()
    {
        Destroy(gameObject);
    }









}
