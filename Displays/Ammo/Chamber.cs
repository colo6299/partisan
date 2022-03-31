using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber : AmmoSlot
{
    private bool reloading;
    private float slotReloadAt;
    public static Dictionary<int, int> magDisplayAssociator;


    private void Awake()
    {
        OnAwake();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (reloading)
        {
            if (Time.time > slotReloadAt)
            {
                reloading = false;
                PullAmmoFromNext();
            }
        }
    }

    protected virtual void OnAwake()
    {
        magDisplayAssociator = new Dictionary<int, int>();
    }

    public bool InstanceToSlots(int ammoType)
    {
        GameObject pfb = PrefabsTracker.GetPrefab(magDisplayAssociator[ammoType]);
        GameObject newMagazine = Instantiate(pfb, transform.position, transform.rotation, null);
        bool success = AddAmmo(newMagazine.GetComponent<AmmoItem>());
        if (!success) { Destroy(newMagazine); }
        return success;
    }

    public int RedeemToChamber()
    {
        if (HasAmmo())
        {
            return ammo.ammoType;
        }
        else
        {
            return -1;
        }
    }
    
    public void FireVisuals()
    {

    }
    public void Reload()
    {
        reloading = true;
        slotReloadAt = Time.time + ammo.reloadDelay;
        ammo.ReloadAnimate();
        ammo = null;
    }
}
