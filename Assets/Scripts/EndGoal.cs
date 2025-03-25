using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoal : MonoBehaviour
{
    [Header("References")]
    private MeshRenderer mr;
    public int killsNeeded;
    public Material transparent;
    public Material solid;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (killsNeeded == 0)
        {
            mr.material = solid;
        }
        else
        {
            mr.material = transparent;
        }
    }
}
