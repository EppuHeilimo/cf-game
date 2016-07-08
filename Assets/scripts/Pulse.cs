using UnityEngine;
using System.Collections;

public class Pulse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var scaleNormal = new GoTween(transform, 0.3f, new GoTweenConfig().scale(1f));
        var scaleBig = new GoTween(transform, 0.3f, new GoTweenConfig().scale(1.2f));

        var chain = new GoTweenChain();
        chain.append(scaleBig).append(scaleNormal);
        chain.play();
        chain.setOnCompleteHandler((x) =>
        {
            x.restart();
        });
    }

   
}
