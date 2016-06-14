using UnityEngine;
using System.Collections;

public class IiroAnimBehavior : MonoBehaviour {

    // Use this for initialization
    NavMeshAgent agent;
    Rigidbody rbody;
    public bool isWalking = false;
    public bool goToSleep = false;
    public bool sit = false;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        rbody = GetComponent<Rigidbody>();
    }   
	
	// Update is called once per frame
	void Update () {

        if(goToSleep || sit)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
        else
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
        transform.FindChild("Iiro").GetComponent<Animator>().SetBool("sleep", goToSleep);
        transform.FindChild("Iiro").GetComponent<Animator>().SetBool("IsWalking", isWalking);
        transform.FindChild("Iiro").GetComponent<Animator>().SetBool("sit", sit);
        if (agent.velocity.magnitude < 30.0f)
        {
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }
    }
}
