using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDirector : MonoBehaviour
{

    //Missile UID, puppet
    public static Dictionary<string, PuppetMissile> missiles;
    public static MissileDirector director;


    private void Awake()
    {
        director = this;
        missiles = new Dictionary<string, PuppetMissile>();
    }



    public void AddMissile(PuppetMissile missile)
    {
        missiles[missile.GetUID()] = missile;
    }

    public void DetonateMissile(string missileUID)
    {
        missiles[missileUID].FlagDetonate();
    }

    public void RemoveMissile(string missileUID)
    {

    }
}
