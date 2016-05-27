using UnityEngine;
using System.Collections;

public class IiroAnimBehavior : MonoBehaviour {

    // Use this for initialization
    NavMeshAgent agent;
    bool isWalking = false;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
	}   
	
	// Update is called once per frame
	void Update () {
        transform.FindChild("Iiro").GetComponent<Animator>().SetBool("IsWalking", isWalking);
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
