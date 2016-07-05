using UnityEngine;
using System.Collections;

public class SplitPillEnd : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pill")
        {
            if (other.gameObject.GetComponent<Pill>().canSplit != 0)
            {
                other.gameObject.GetComponent<Pill>().splitPill(false);
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
        }
    }
}
