using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoal : MonoBehaviour
{
    [Header("References")]
    private MeshRenderer _mr;
    public Material _transparent;
    public Material _solid;

    [Header("Variables")]
    public int iKillsNeeded;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (iKillsNeeded <= 0)
        {
            _mr.material = _solid;
        }
        else
        {
            _mr.material = _transparent;
        }
    }
}
