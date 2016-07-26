using UnityEngine;
using System.Collections;

/* component used to play a sound when pill collides */
public class PlaySound : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Pill")
            GetComponent<AudioSource>().Play();

    }
   
}
