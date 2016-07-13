using UnityEngine;
using System.Collections;

public class TutorialWalkZone : MonoBehaviour {

    bool reached;

    void OnTriggerEnter(Collider other)
    {
        if (!reached)
        { 
            if (other.gameObject.tag == "Player")
            {
                reached = true;
                GameObject.Find("Tutorial").GetComponent<Tutorial>().ReachedWalkZone();
            }
        }
    }
}
