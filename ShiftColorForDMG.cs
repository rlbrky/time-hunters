using System.Collections;
using UnityEngine;

public class ShiftColorForDMG : MonoBehaviour
{
    [SerializeField] MeshRenderer rend;


    Material[] cCcMatscCc;

    public float _takeDamgTime = 0.5f;
    public float _changeMultiplier = 1f;
    bool changin = false;
    bool stopCor = false;

    private void Start()
    {
        cCcMatscCc = new Material[rend.materials.Length];

        for (int i = 0; i < rend.materials.Length; i++)
        {
            //rend.materials[i].EnableKeyword("_EMISSION");
            cCcMatscCc[i] = new Material(rend.materials[i]);
        }

    }

    public void PlayEffect()
    {
        if (!changin)
        {
            StartCoroutine(ChangeMaterial());
        }
        else
        {
            stopCor = true;
            foreach (var mat in rend.materials)
            {
                mat.SetColor("_EmissiveColor", Color.white);
                mat.SetFloat("_Smoothness", 0f);
            }

            StartCoroutine(ChangeMaterial());
        }

        changin = true;
    }

    public IEnumerator ChangeMaterial()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        foreach (var mat in rend.materials)
        {
            mat.SetColor("_EmissiveColor", Color.white);
            mat.SetFloat("_Smoothness", 0f);
        }
        
        float currTime = _takeDamgTime;
        float changeSpeed;

        while (currTime > 0f)
        {
            if(stopCor)
            {
                stopCor = false;
                break;
            }

            currTime -= Time.deltaTime;
            changeSpeed = (Time.deltaTime / _takeDamgTime)* _changeMultiplier;

            for (int i = 0; i < rend.materials.Length; i++)
            {
                // rend.materials[i].SetColor("_EmissionColor", Color.Lerp(rend.materials[i].GetColor("_EmissionColor"), cCcMatscCc[i].GetColor("_EmissionColor"), changeSpeed));
                rend.materials[i].SetColor("_EmissiveColor", MoveTColor(rend.materials[i].GetColor("_EmissiveColor"), cCcMatscCc[i].GetColor("_EmissiveColor"), changeSpeed));
                rend.materials[i].SetFloat("_Smoothness", Mathf.MoveTowards(rend.materials[i].GetFloat("_Smoothness"), cCcMatscCc[i].GetFloat("_Smoothness"), changeSpeed));
            }
            if (rend.materials[0].GetFloat("_Smoothness") == cCcMatscCc[0].GetFloat("_Smoothness") && rend.materials[0].GetColor("_EmissiveColor") == cCcMatscCc[0].GetColor("_EmissiveColor"))
                break;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        changin = false;
    }

    Color MoveTColor(Color a, Color b, float speed)
    {
        return new Color(
                Mathf.MoveTowards(a.r, b.r, speed),
                Mathf.MoveTowards(a.g, b.g, speed),
                Mathf.MoveTowards(a.b, b.b, speed),
                Mathf.MoveTowards(a.a, b.a, speed)
            ) ;
    }
}
