using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum States
{
    Patrol,
    Seek,
    Flee,
    ResumePatrol
}

public class EnemyState : MonoBehaviour
{
    NavMeshAgent GuardNavmesh;
    //make range for enemy sight
    public int maxRange;
    int waypoint = 0;
    public float multiplyBy;
    private float Timer = 0.0f;
    public float fleeTimer;

    //guard health
    

    //waypoints for patrol
    public GameObject waypointOne;
    public GameObject waypointTwo;

    //what the guard is chasing or runing from
    public GameObject Player;
    public GameObject EnemyHazard;

    bool Hunting = false;

    private States currentState;

    // Start is called before the first frame update
    void Start()
    {
        GuardNavmesh = GetComponent<NavMeshAgent>();
        currentState = States.ResumePatrol;
        GuardNavmesh.autoBraking = false;
    }

    // Update is called once per frame
    void Update()
    {

        ChangeStates();
        switch(currentState)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Seek:
                Seek();
                break;
            case States.Flee:
                Flee();
                break;
            case States.ResumePatrol:
                ResumePatrol();
                break;
            default:
                Debug.LogError("Invalid state!");
                break;
        }

    }

    void ChangeStates()
    {
        Vector3 targetDir = Player.transform.position - transform.position;
        float angleToPlayer = (Vector3.Angle(targetDir, transform.forward));
        float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);

        Vector3 hazardDir = EnemyHazard.transform.position - transform.position;
        float angleToHazard = (Vector3.Angle(hazardDir, transform.forward));
        float distanceToHazard = Vector3.Distance(transform.position, EnemyHazard.transform.position);

        RaycastHit Seekhit;
        if (currentState != States.Flee || Timer >= fleeTimer)
        { 
            if (angleToPlayer >= -90 && angleToPlayer <= 90 && distanceToPlayer <= maxRange)
            {
                if(Physics.Raycast(transform.position, targetDir, out Seekhit, 100.0f) && Seekhit.transform.tag == "Player")
                {
                    currentState = States.Seek;
                }
            }
            else if (angleToHazard >= -90 && angleToHazard <= 90 && distanceToHazard <= maxRange)
            {
                Timer = 0.0f;
                currentState = States.Flee;
            }
            else
            {
                if (currentState != States.Patrol)
                    currentState = States.ResumePatrol;
                else
                    currentState=States.Patrol;
            }
        }
    }
    void Patrol()
    {
        if (waypoint ==0 && Vector3.Distance(transform.position, waypointOne.transform.position) < 0.5f)
        {
            waypoint = 1;
            GuardNavmesh.SetDestination(waypointTwo.transform.position);
            return;
        }

        if (waypoint ==1 && Vector3.Distance(transform.position, waypointTwo.transform.position) < 0.5f)
        {
            waypoint = 0;
            GuardNavmesh.SetDestination(waypointOne.transform.position);
            return;
        }
    }

    void ResumePatrol()
    {
        //set to waypointOne;
        waypoint = 0;
        GuardNavmesh.SetDestination(waypointOne.transform.position);

        currentState = States.Patrol;
    }
    //void GetWaypoint() { 
    //if (Hunting == true)
    //    {
    //        Hunting = false;
    //    }

    //    if (waypoint == 0)
    //    {
    //        GuardNavmesh.SetDestination(waypointOne.transform.position);
    //        //if(Vector3.Distance(transform.position, waypointOne.transform.position) < 0.5f)
    //        //{
    //        //    waypoint = 1;
    //        //}
    //    }
    //    else
    //    {
    //        GuardNavmesh.SetDestination(waypointTwo.transform.position);
    //        //if (Vector3.Distance(transform.position, waypointTwo.transform.position) < 0.5f)
    //        //{
    //        //    waypoint = 0;
    //        //}
    //    }
    //    currentState = States.Patrol;
    //}


    void Seek()
    {
        if (Hunting == false)
        {
            maxRange += 10;
            Hunting = true;
        }

        GuardNavmesh.SetDestination(Player.transform.position);
    }
    void Flee()
    {
      
        Debug.Log("running");

        
        Timer += Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(transform.position - EnemyHazard.transform.position);

        Vector3 runTo = transform.position + transform.forward * multiplyBy;

        NavMeshHit hit;
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));

        GuardNavmesh.SetDestination(hit.position);

    }
    

}
