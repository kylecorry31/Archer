using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ArcherArea : Area
{

    public GameObject target;

    private float distance = 0.0f;

    public void MoveTarget()
    {
        distance = Random.Range(10.0f, 100.0f);
        target.transform.position = new Vector3(distance, target.transform.position.y, target.transform.position.z);
    }

    public float GetTargetDistance()
    {
        return distance;
    }

}
