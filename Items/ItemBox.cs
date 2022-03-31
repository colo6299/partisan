using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public Color shaderColor;
    public GameObject materialObject;
    private Material material;

    private void Awake()
    {
        if (materialObject == null) { materialObject = gameObject; }
        MeshRenderer meshMaterial = materialObject.GetComponent<MeshRenderer>();
        if (meshMaterial == null)
        {
            meshMaterial = materialObject.GetComponentsInChildren<MeshRenderer>()[0];
        }
        material = meshMaterial.material;
        material.SetColor("_Color", shaderColor);
        material.SetColor("_EmissionColor", shaderColor);
    }
}
