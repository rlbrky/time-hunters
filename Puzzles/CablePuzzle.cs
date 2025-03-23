using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePuzzle : MonoBehaviour
{
    [SerializeField] private Animator switchAnimator;
    [SerializeField] private RopeSpawner cableSpawner;
    [SerializeField] private LayerMask connectPointLayerMask;
    
    private Transform target1;
    private Transform target2;

    //Sub connections for each color and positive-negative difference.
    private bool[] subConnection_Red_P = new bool[3];
    private bool[] subConnection_Red_M = new bool[3];
    private bool[] subConnection_Blue_P = new bool[3];
    private bool[] subConnection_Blue_M = new bool[3];

    private int onHash = Animator.StringToHash("On");
    private int offHash = Animator.StringToHash("Off");
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(target1!=null)
                Debug.Log(target1.position);
            if(target2!=null)
                Debug.Log(target2.position);
                
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 10, connectPointLayerMask))
            {
                Debug.Log("Cable Puzzle Hit: " + hit.transform.position);
                if (hit.collider.name == "Kol")
                {
                    if (subConnection_Red_P[0] && subConnection_Red_P[1] && subConnection_Red_P[2] &&
                        subConnection_Red_M[0] && subConnection_Red_M[1] && subConnection_Red_M[2] &&
                        subConnection_Blue_P[0] && subConnection_Blue_P[1] && subConnection_Blue_P[2] &&
                        subConnection_Blue_M[0] && subConnection_Blue_M[1] && subConnection_Blue_M[2]) //If every connection is correct.
                    {
                        Debug.Log("Puzzle Solved.");
                        StartCoroutine(PlaySwitchAnimation(true));
                        //Quit puzzle screen and do something.
                    }
                    else
                    {
                        StartCoroutine(PlaySwitchAnimation(false));
                        cableSpawner.ResetRope();
                        Array.Clear(subConnection_Red_P, 0, 3);
                        Array.Clear(subConnection_Red_M, 0, 3);
                        Array.Clear(subConnection_Blue_P, 0,3);
                        Array.Clear(subConnection_Blue_M, 0,3);
                        target1 = null;
                        target2 = null;
                        Debug.Log("Reset happened.");
                    }
                }
                else
                { 
                    //hit.transform.GetComponent<ShiftColorForDMG>().PlayEffect();
                    if (hit.transform.name[0] != 'R') //Check the first letter to ensure it is not a Response point.
                    {
                        target1 = hit.transform;
                    }
                    else if(target1 != null && target2 == null && hit.transform.name[0] != 'C')
                    {
                        target2 = hit.transform;

                        switch (target1.name)
                        {
                            case "C_Red.P.1":
                                if(target2.name == "R_Red.P.2")
                                    subConnection_Red_P[0] = true;
                                break;
                            case "C_Red.P.3":
                                if(target2.name == "R_Red.P.4")
                                    subConnection_Red_P[1] = true;
                                break;
                            case "C_Red.P.5":
                                if(target2.name == "R_Red.P.6")
                                    subConnection_Red_P[2] = true;
                                break;
                            case "C_Red.M.1":
                                if(target2.name == "R_Red.M.2")
                                    subConnection_Red_M[0] = true;
                                break;
                            case "C_Red.M.3":
                                if(target2.name == "R_Red.M.4")
                                    subConnection_Red_M[1] = true;
                                break;
                            case "C_Red.M.5":
                                if(target2.name == "R_Red.M.6")
                                    subConnection_Red_M[2] = true;
                                break;
                            case "C_Blue.P.1":
                                if(target2.name == "R_Blue.P.2")
                                    subConnection_Blue_P[0] = true;
                                break;
                            case "C_Blue.P.3":
                                if(target2.name == "R_Blue.P.4")
                                    subConnection_Blue_P[1] = true;
                                break;
                            case "C_Blue.P.5":
                                if(target2.name == "R_Blue.P.6")
                                    subConnection_Blue_P[2] = true;
                                break;
                            case "C_Blue.M.1":
                                if(target2.name == "R_Blue.M.2")
                                    subConnection_Blue_M[0] = true;
                                break;
                            case "C_Blue.M.3":
                                if(target2.name == "R_Blue.M.4")
                                    subConnection_Blue_M[1] = true;
                                break;
                            case "C_Blue.M.5":
                                if(target2.name == "R_Blue.M.6")
                                    subConnection_Blue_M[2] = true;
                                break;
                        }
                                            
                        CreateCable();
                    }
                    else
                    {
                        Debug.Log("Nothing happened.");
                    }
                }
            }
        }
    }
    
    private void CreateCable()
    {
        float distance = Vector3.Distance(target1.position, target2.position);
        cableSpawner.ropeLength = distance;
        
        if (target1.name.Contains("Red"))
            cableSpawner.SpawnRope(Color.red);
        else
            cableSpawner.SpawnRope(Color.blue);
        
        
        cableSpawner.GetFirst().transform.position = target1.position;
        cableSpawner.GetLast().transform.position = target2.position;
        
        //Reset targets
        target1 = null;
        target2 = null;
    }

    private IEnumerator PlaySwitchAnimation(bool didSolvePuzzle)
    {
        if (didSolvePuzzle)
        {
            switchAnimator.SetTrigger(onHash);
            yield return null;
        }
        else
        {
            switchAnimator.SetTrigger(onHash);
            yield return new WaitForSeconds(1);
            switchAnimator.SetTrigger(offHash);
        }
    }
}
