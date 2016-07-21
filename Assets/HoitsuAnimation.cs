using UnityEngine;
using System.Collections;

public class HoitsuAnimation : MonoBehaviour {

    Animator anim;
    NavMeshAgent agent;
    bool walking = false;
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        anim = transform.FindChild("Iiro").GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
        if (agent.velocity.magnitude < 10.0f)
        {
            anim.SetBool("walk", false);
        }
        else
        {
            anim.SetBool("walk", true);
        }

    }
}
