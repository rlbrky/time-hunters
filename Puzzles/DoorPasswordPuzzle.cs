using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorPasswordPuzzle : MonoBehaviour
{
    [SerializeField] private LayerMask PuzzleButtonLayer;
    [SerializeField] private TextMeshProUGUI PasswordText;
    
    [Header("Puzzle Variables")]
    [SerializeField] private string passwordAnswer;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 10, PuzzleButtonLayer))
            {
                    //hit.transform.GetComponent<ShiftColorForDMG>().PlayEffect();
                if(hit.transform.name == "Reset")
                    ResetPasswordProgress();
                else
                {
                    PasswordText.text += hit.transform.name;
                    if (PasswordText.text.Length == passwordAnswer.Length)
                    {
                        if(PasswordText.text == passwordAnswer)
                            Debug.Log("Correct Password");
                        else
                        {
                            ResetPasswordProgress();
                        }
                    }
                }
            }
        }
    }

    private void ResetPasswordProgress()
    {
        Debug.Log("Wrong answer please try again");
        PasswordText.text = "";
    }
}
