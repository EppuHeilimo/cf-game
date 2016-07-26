using UnityEngine;
using System.Collections;

public class Pulse : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // tween for mascot pulse animation
        var tween = Go.to(transform, 0.3f, new GoTweenConfig()
            .scale(new Vector3(1.2f, 1.2f, 1.2f))
            .setIterations(-1, GoLoopType.PingPong));
    }
   
}
