using UnityEngine;
using System.Collections;

public class TrashCanTriggers : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerControl>().atTrashCan = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerControl>().atTrashCan = true;
        }
    }

}
