using UnityEngine;
using System.Collections;

public class SplitPill : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pill")
        {
            if (other.gameObject.GetComponent<Pill>().canSplit != 0)
            { 
                other.gameObject.GetComponent<Pill>().splitPill(true);
                Time.timeScale = 0.2f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
        }
    }
}
