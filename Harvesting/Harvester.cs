using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour
{

    public GameObject beam;
    private int visualsID = 17;
    private GameObject visualsPrefab;
    private GameObject hitVisuals;
    private float chargeSpeed = 0.5f;
    private float startCharge;
    private bool isCharging;
    private float lockTime;

    public ScannerDisplay scannerDisplay;

    private HarvestNode node;

    private bool isHarvesting;

    private void Start()
    {
        visualsPrefab = PrefabsTracker.GetPrefab(visualsID);
    }
    private void Update()
    {
        CheckHarvest();
        CheckCharging();
    }

    private void CheckHarvest()
    {
        if (!isHarvesting) { beam.SetActive(false); Destroy(hitVisuals); scannerDisplay.Scanning(false); return; }
        scannerDisplay.Scanning(true);
        beam.SetActive(true);
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, 5f);

        if (hit.point != null)
        {
            if (hitVisuals == null) { hitVisuals = Instantiate(visualsPrefab, transform, true); }
            hitVisuals.transform.position = hit.point;
            node = hit.transform.GetComponent<HarvestNode>();
            if (node != null)
            {
               scannerDisplay.ScanNode(node);
            }
            else
            {
                scannerDisplay.DropNode();
            }
        }
        else
        {
            if (hitVisuals != null) { Destroy(hitVisuals); hitVisuals = null; }
            scannerDisplay.DropNode();
        }
    }

    private void CheckCharging()
    {
        if (isHarvesting)
        {
            if (Time.time < lockTime) { return; }
            if (Input.GetButton("Fire1"))
            {
                if (!isCharging)
                {
                    startCharge = Time.time;
                    scannerDisplay.StartCharging();
                    isCharging = true;
                }
                float lerpFraction = (Time.time - startCharge) * chargeSpeed;
                scannerDisplay.SlideBox(lerpFraction);
            }
            else
            {
                if (isCharging)
                {
                    isCharging = false;
                    lockTime = Time.time + scannerDisplay.EndCharging();
                    int[] harvestedMats = scannerDisplay.CheckHarvestForMaterials();
                    foreach(int material in harvestedMats)
                    {
                        FireController.fc.AddMaterial(material);
                    }
                }
            }
        }
    }

    public void SetState(bool state)
    {
        isHarvesting = state;
    }


}
