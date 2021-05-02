using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nav : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    public GameObject[] waypoints;

    private void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void NavMoveTo(int iwp)
    {
        agent.SetDestination(waypoints[iwp].transform.position);
    }

}