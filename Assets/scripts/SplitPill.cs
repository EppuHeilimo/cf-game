using UnityEngine;
using System.Collections;

public class SplitPill : MonoBehaviour {

    Pill pill;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pill")
        {
            if (other.gameObject.GetComponent<Pill>().canSplit != 0)
            {
                pill = other.gameObject.GetComponent<Pill>();
                if (pill != null)
                {
                    pill.splitPill(true);
                    pill.GetComponent<CircleCollider2D>().radius = Constants.PillColliderRadiusBig;
                }
                Time.timeScale = 0.2f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                Invoke("endSlowMo", 0.2f);
            }
        }
    }

    void endSlowMo()
    {
        if (pill != null)
        {
            pill.splitPill(false);
            pill.GetComponent<CircleCollider2D>().radius = Constants.PillColliderRadiusNormal;
        }
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }
}
