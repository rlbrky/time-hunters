using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlickeringLightSC : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light flickeringLight1;
    [SerializeField] private Light flickeringLight2;

    [Header("Flicker Properties")]
    [SerializeField] private float minFlickerSpeed = 0.01f;
    [SerializeField] private float maxFlickerSpeed = 0.1f;
    [SerializeField] private float minLightIntensity = 0;
    [SerializeField] private float maxLightIntensity = 1;
    
    //private float emissiveIntensity;
    private Material _parentLightMaterial;
    private Color _parentOriginalColor;
    private readonly Color _emissiveColor = Color.black;
    
    private void Awake()
    {
        _parentLightMaterial = GetComponent<Renderer>().material;
        _parentOriginalColor = _parentLightMaterial.GetColor("_EmissiveColor");
        Debug.Log(_parentOriginalColor);
    }

    private void Start()
    {
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            _parentLightMaterial.SetColor("_EmissiveColor", _parentOriginalColor);
            flickeringLight1.enabled = true;
            flickeringLight1.intensity = Random.Range(minLightIntensity, maxLightIntensity);
            flickeringLight2.enabled = true;
            flickeringLight2.intensity = Random.Range(minLightIntensity, maxLightIntensity);
            yield return new WaitForSeconds (Random.Range(minFlickerSpeed, maxFlickerSpeed));
            _parentLightMaterial.SetColor("_EmissiveColor", _emissiveColor);
            flickeringLight1.enabled = false;
            flickeringLight2.enabled = false;
            yield return new WaitForSeconds (Random.Range(minFlickerSpeed, maxFlickerSpeed));
        }
    }
}
