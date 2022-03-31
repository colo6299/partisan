using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerDisplay : MonoBehaviour
{

    private HarvestNode node;

    public GameObject sweeper;
    public HarvestBox HarvestBox;
    public GameObject dotHolder;
    private Animator animator;

    public float sweepSpeed = 1;
    public float detectDelay = 0.217f;
    public float detectRate = 0.02f;

    private float chargeLockout = 1f;
    private bool cLocked;
    private float cLockTime;

    private float rollTimeAccumulator;

    public static Dictionary<int, int> itemAssociator;

    public Transform hboxTop;
    public Transform hboxBottom;

    private bool scanning;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        itemAssociator = new Dictionary<int, int>();
        itemAssociator[0] = 18;
        itemAssociator[1] = 19;
        itemAssociator[2] = 20;
        itemAssociator[3] = 21;
        itemAssociator[4] = 22;
        itemAssociator[5] = 23;
        itemAssociator[6] = 24;
    }

    private void Update()
    {
        CheckScanning();
        CheckChargeEndVisuals();
    }

    private void CheckScanning()
    {
        if (scanning)
        {
            animator.SetTrigger("scan");
            rollTimeAccumulator += Time.deltaTime;
            if (rollTimeAccumulator > detectDelay)
            {
                rollTimeAccumulator -= detectDelay;
                if ((Random.Range(0f, 1f) < detectRate) & node != null)
                {
                    int typeID = node.GetItem();
                    if (typeID == -1) { Debug.LogError("Node response error."); }
                    GameObject dot = PrefabsTracker.GetPrefab(itemAssociator[typeID]);
                    Vector3 variance = transform.right * Random.Range(-0.085f, 0.085f);
                    Instantiate(dot, sweeper.transform.position + variance, sweeper.transform.rotation, dotHolder.transform);
                }
            }
        }
        else
        {
            animator.SetTrigger("unscan");
            cLocked = true;
            cLockTime = 0;
            CheckChargeEndVisuals();
            for (int i=0; i<dotHolder.transform.childCount; i++)
            {
                Destroy(dotHolder.transform.GetChild(i).gameObject);
            }
        }
    }

    public virtual void SlideBox(float fraction)
    {
        HarvestBox.transform.position = Vector3.Lerp(hboxTop.position, hboxBottom.position, fraction);
    }

    public void StartCharging()
    {
        HarvestBox.gameObject.SetActive(true);
        HarvestBox.transform.position = hboxTop.position;
    }

    public float EndCharging()
    {
        cLocked = true;
        cLockTime = Time.time + chargeLockout*0.5f;
        return chargeLockout;
    }

    public int[] CheckHarvestForMaterials()
    {
        ScanDot[] dots = HarvestBox.GetDots();
        int[] dotTypes = new int[dots.Length];
        int i = 0;
        foreach(ScanDot dot in dots)
        {
            dotTypes[i] = dot.type;
            dot.RemoveDot();
            i++;
        }
        return dotTypes;
    }

    private void CheckChargeEndVisuals()
    {
        if (!cLocked) { return; }
        if (Time.time < cLockTime) { return; }
        cLocked = false;
        HarvestBox.gameObject.SetActive(false);
        HarvestBox.transform.position = hboxTop.position;
    }

    public void ScanNode(HarvestNode scanNode)
    {
        node = scanNode;
    }

    public void DropNode()
    {
        node = null;
    }

    public void Scanning(bool state)
    {
        scanning = state;
    }


}
