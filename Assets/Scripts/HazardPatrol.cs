using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HazardPatrol : MonoBehaviour
{
    NavMeshAgent HazardNavmesh;

    public Transform[] waypoints;
    private int points = 0;

    // Start is called before the first frame update
    void Start()
    {
        HazardNavmesh = GetComponent<NavMeshAgent>();
        hazardPatrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (!HazardNavmesh.pathPending && HazardNavmesh.remainingDistance < 0.5f)
        {
            hazardPatrol();
        }
    }

    void hazardPatrol()
    {
        if (waypoints.Length == 0)
            return;

        HazardNavmesh.destination = waypoints[points].position;

        points = (points + 1) % waypoints.Length;
    }
}
