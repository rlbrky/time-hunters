using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimation : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Transform gateUp;
    [SerializeField] private Transform gateDown;

    [Header("Timing")] 
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private float gateStayOpenTime = 1.5f;
    
    [Header("Location Setup")]
    [SerializeField] private Vector3 locationUp = new Vector3(0.0f, 5.16f, 0.0f);
    [SerializeField] private Vector3 locationDown = new Vector3(0.0f, -6.46f, 0.0f);

    private Vector3 startPosition_Up;
    private Vector3 startPosition_Down;

    public bool test = false;
    
    private void Start()
    {
        startPosition_Up = gateUp.localPosition;
        startPosition_Down = gateDown.localPosition;
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            PlayGateAnimation();
        }
    }

    public void PlayGateAnimation()
    {
        Debug.Log("Up location: " + locationUp);
        Debug.Log("Down location: " + locationDown);
        StartCoroutine(PlayGateAnimation_Open());
    }
    
    IEnumerator PlayGateAnimation_Open()
    {
        float time = 0;
        while (time <= animDuration)
        {
            time += Time.deltaTime;
            gateUp.localPosition = new Vector3(gateUp.localPosition.x, Mathf.Lerp(gateUp.localPosition.y, locationUp.y, time / animDuration), gateUp.localPosition.z);
            gateDown.localPosition = new Vector3(gateDown.localPosition.x, Mathf.Lerp(gateDown.localPosition.y, locationDown.y, time / animDuration), gateDown.localPosition.z);
            yield return null;
        }
        
        gateUp.localPosition = new Vector3(gateUp.localPosition.x, locationUp.y, gateUp.localPosition.z);
        gateDown.localPosition = new Vector3(gateDown.localPosition.x, locationDown.y, gateDown.localPosition.z);

        yield return new WaitForSeconds(gateStayOpenTime);
        StartCoroutine(PlayGateAnimation_Close());
    }

    IEnumerator PlayGateAnimation_Close()
    {
        float time = 0;

        while (time <= animDuration)
        {
            gateUp.localPosition = new Vector3(gateUp.localPosition.x, Mathf.Lerp(gateUp.localPosition.y, startPosition_Up.y, time / animDuration), gateUp.localPosition.z);
            gateDown.localPosition = new Vector3(gateDown.localPosition.x, Mathf.Lerp(gateDown.localPosition.y, startPosition_Down.y, time / animDuration), gateDown.localPosition.z);
            time += Time.deltaTime;
            yield return null;
        }
        
        gateUp.localPosition = new Vector3(gateUp.localPosition.x, startPosition_Up.y, gateUp.localPosition.z);
        gateDown.localPosition = new Vector3(gateDown.localPosition.x, startPosition_Down.y, gateDown.localPosition.z);
    }
}
