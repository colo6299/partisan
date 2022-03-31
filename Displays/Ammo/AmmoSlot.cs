using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSlot : MonoBehaviour
{
    private static AmmoSlot chamber;
    public AmmoSlot nextSlot;
    //public AmmoSlot previousSlot;
    public bool isChamber;

    public AmmoItem ammo;
    private float slerpTime = 0;
    private float slerpSpeed = 0.5f;

    private void Awake()
    {
        if (isChamber)
        {
            chamber = this;
        }
        if (ammo == null)
        {
            ammo = GetComponentInChildren<AmmoItem>();
            slerpTime = Time.time;
        }
    }
    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (ammo != null)
        {
            float slerpValue = (Time.time - slerpTime) * slerpSpeed;
            ammo.transform.position = Vector3.Lerp(ammo.transform.position, transform.position, slerpValue);
            ammo.transform.rotation = Quaternion.Lerp(ammo.transform.rotation, transform.rotation, slerpValue);
        }
    }

    public bool HasAmmo()
    {
        if (ammo != null) { return true; }
        else { return false; }
    }

    public bool AddAmmo(AmmoItem addAmmo)
    {
        if (HasAmmo() == false)
        {
            ammo = addAmmo;
            slerpTime = Time.time;
            return true;
        }
        else
        {
            if (nextSlot != null)
            {
                return nextSlot.AddAmmo(addAmmo);
            }
            else
            {
                return false;
            }
        }
    }

    public void PullAmmoFromNext()
    {
        if (nextSlot != null) { PullAmmoFromSlot(nextSlot); }
    }

    public bool PullAmmoFromSlot(AmmoSlot fromSlot)
    {
        if (ammo != null) {  }
        else { ammo = fromSlot.AmmoRequest(); slerpTime = Time.time; }
        
        if (fromSlot != null) { fromSlot.PullAmmoFromNext(); }
        
        if (ammo != null) { return true; }
        else { return false; }
    }

    public AmmoItem AmmoRequest()
    {
        if (ammo != null)
        {
            AmmoItem oldAmmo = ammo;
            ammo = null;
            return oldAmmo;
        }
        else
        {
            return null;
        }
    }
}
