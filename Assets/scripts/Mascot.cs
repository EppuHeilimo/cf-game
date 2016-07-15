using UnityEngine;
using System.Collections;

public class Mascot : MonoBehaviour {

    public bool isTalking;
    public Animator anim;
    float delay = 3f;

    /*
    void Start()
    {
        var scaleBig = new GoTween(transform, 0.2f, new GoTweenConfig().scale(new Vector3(-1.2f, 1.2f, 1.2f)));
        var scaleNormal = new GoTween(transform, 0.2f, new GoTweenConfig().scale(new Vector3(-1f, 1f, 1f)));
        var chain = new GoTweenChain();
        chain.append(scaleBig).append(scaleNormal).appendDelay(4);
        chain.play();
        chain.setOnCompleteHandler(c => c.restart());
    }
    */

    void Update()
    {
        if (isTalking)
            anim.Play("Mascot");
        else
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                anim.Play("MascotPulse");
                delay = 3f;
                anim.Play("MascotPulse", -1, 0f);
            }
        }
            
    }
}
