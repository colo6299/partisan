using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public static FireController fc;
    private GameObject missilePrefab;
    public Transform missileHolder;
    public Transform enemyMissileHolder;
    public PlayerPartisan player;
    public string team = "Blue";
    public float missileSpeed = 30;
    public int missileType = 0;
    public float fireDelay = 1;
    public float reloadTime = 1;
    public int magSize = 1;
    public Chamber chamber;

    private int firedFromMag = 0;
    private float nextFireTime = 0;

    private int[] materialInventory;
    private int invCount;

    private HashSet<string> knownIDs;

    private Queue<string> missileQueue;

    public static Dictionary<int, int> projectileAssociator;


    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        fc = this;
        projectileAssociator = new Dictionary<int, int>();
        materialInventory = new int[16];
        knownIDs = new HashSet<string>();
        missileQueue = new Queue<string>();
    }

    private void Start()
    {
        AddMaterial(0, 5);
        missilePrefab = PrefabsTracker.GetPrefab(projectileAssociator[missileType]);
    }

    private void Update()
    {
        if (missileQueue.Count > 0)
        {
            FireAt(missileQueue.Dequeue());
        }
        if (Input.GetButtonDown("Reload"))
        {
            AddMaterial(0);
        }
    }

    public bool AddMaterial(int item, int amount=1)
    {
        int numberAdded = 0;
        for (int i = 0; i < amount; i++)
        {
            bool addedSuccessfully = chamber.InstanceToSlots(item);
            if (addedSuccessfully)
            {
                numberAdded += 1;
            }
            else { break; }
        }
        materialInventory[item] += numberAdded; //add goodness
        invCount += numberAdded;
        return true;
    }


    public virtual bool TryFire()
    {
        if(Time.time > nextFireTime)
        {
            if (firedFromMag == 0) 
            {
                missileType = RedeemMagazine(); 
                if (missileType == -1) { Debug.Log("DetectEmpty"); return false; }
                else { missilePrefab = PrefabsTracker.GetPrefab(projectileAssociator[missileType]); }
            }
            Fire();
            nextFireTime = Time.time + fireDelay;
            firedFromMag += 1;
            if (firedFromMag == magSize) { Reload(); }
            else if (firedFromMag > magSize) { Debug.LogError("Mag exceeded..."); }
            return true;
        }
        else
        {
            return false;
        }
    }


    private void Reload()
    {
        chamber.Reload();
        firedFromMag = 0;
        nextFireTime = Time.time + reloadTime;
    }

    public void QueueMissile(string missileReport)
    {
        if (knownIDs.Contains(missileReport.Split(' ')[0]) != true)
        {
            knownIDs.Add(missileReport.Split(' ')[0]);
            missileQueue.Enqueue(missileReport);
        }
    }

    private void FireAt(string missileReport)
    {
        int missileType = int.Parse(missileReport.Split(' ')[2]);
        GameObject typePrefab = PrefabsTracker.GetPrefab(projectileAssociator[missileType]);
        PuppetMissile missile = Instantiate(typePrefab, new Vector3(0, -100, 0), Quaternion.identity, enemyMissileHolder).GetComponent<PuppetMissile>();
        missile.InvariantMissileInitialize(missileReport);
        MissileDirector.director.AddMissile(missile);
    }

    private void RedeemMaterial()
    {
        string redeemString;
        
    }

    private int RedeemMagazine()
    {
        string redeemString;
        string ammoToRedeem;
        //ammo to redeem is the chambered mag

        int ammoType = chamber.RedeemToChamber();
        if (ammoType == -1) { return ammoType; }

        //build TCP redeem string

        return ammoType;
    }

    private void Fire()
    {
        PuppetMissile missile = Instantiate(missilePrefab, transform.position, transform.rotation, missileHolder).GetComponent<PuppetMissile>();

        string reportString;
        
        string missileID = Guid.NewGuid().ToString();
        reportString = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
            missileID,
            team,
            missileType,
            0,
            transform.position.x,
            transform.position.z,
            transform.forward.x * missileSpeed + player.velocity.x,
            transform.forward.z * missileSpeed + player.velocity.z
        );
        missile.InvariantMissileInitialize(reportString);
        MissileDirector.director.AddMissile(missile);

        string tcpReport = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
            missileID,
            team,
            missileType,
            StreamWorldWatcher.GetNetOffset() + Time.realtimeSinceStartup,
            transform.position.x,
            transform.position.z,
            transform.forward.x * missileSpeed + player.velocity.x,
            transform.forward.z * missileSpeed + player.velocity.z
        );

        StreamWorldWatcher.AddTcpCommand(StreamWorldWatcher.uID + " FireMissile " + tcpReport);
    }
}
