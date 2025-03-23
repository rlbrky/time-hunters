using UnityEngine;

public class RopeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ropeParent_Prefab;
    
    [SerializeField] [Range(0.01f, 1000)] public float ropeLength = 1;

    [SerializeField] private float partDistance = 0.2f;

    [SerializeField] private bool reset, spawn, snapFirst, snapLast;

    [SerializeField] public GameObject rope_PartPrefab;
    
    private GameObject rope_First, rope_Last;
    
    private void Update()
    {
        if (reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Rope"))
            {
                Destroy(tmp);
            }
            
            reset = false;
        }

        if (spawn)
        {
            SpawnRope(Color.gray);
            
            spawn = false;
        }
    }

    public void SpawnRope(Color ropeColor)
    {
        int count = (int) (ropeLength / partDistance);

        for (int i = 0; i < count; i++)
        {
            GameObject rope = Instantiate(rope_PartPrefab, new Vector3(transform.position.x, transform.position.y + partDistance * (i + 1), transform.position.z), Quaternion.identity, ropeParent_Prefab.transform);
            rope.GetComponent<Renderer>().material.color = ropeColor;
            rope.transform.eulerAngles = new Vector3(180, 0, 0);
            
            rope.name = ropeParent_Prefab.transform.childCount.ToString();
            
            if (i == 0)
            {
                Destroy(rope.GetComponent<ConfigurableJoint>());

                if (snapFirst)
                {
                    rope.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    rope_First = rope.gameObject;
                }
            }
            else
            {
                rope.GetComponent<ConfigurableJoint>().connectedBody =
                    ropeParent_Prefab.transform.Find((ropeParent_Prefab.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        if (snapLast)
        {
            rope_Last = ropeParent_Prefab.transform.Find(ropeParent_Prefab.transform.childCount.ToString()).gameObject;
                
            rope_Last.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void ResetRope()
    {
        foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Rope"))
        {
            Destroy(tmp);
        }
    }
    
    public GameObject GetFirst()
    {
        return rope_First;
    }

    public GameObject GetLast()
    {
        return rope_Last;
    }
}
