using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Pill")
            GetComponent<AudioSource>().Play();

    }
   
}
